using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Accounts;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Dao.PhotoEncrypt;
using DataAccessLayer.Dao.PhotoThumbnailDao;
using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Dao.SharedPhotoAlbum;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Dto.PhotoEncrypt;
using DataAccessLayer.Dto.PhotoThumbnail;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.RelationUserSharedAlbum;
using DataAccessLayer.Dto.SharedPhotoAlbum;
using DataAccessLayer.Dto.SystemImages;
using EncryptionLayer.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Transactions
{
    public interface IPhotoTransaction
    {
        Task SetPhoto(PhotoModel photo, PhotoThumbnailModel thumbnail, PhotoEncryptModel photoEncrypt, PhotoEncryptModel thumbnailEncrypt);
        Task SetPhotoMultipleUser(PhotoModel photo, PhotoThumbnailModel thumbnail, List<PhotoEncryptModel> photoEncryptList, List<PhotoEncryptModel> thumbnailEncryptList);
        Task Delete(int photoId, RelationAlbumPhotoModel relationAlbumPhotoModel, List<RelationSharedAlbumPhotoModel> sharedRelation);
        Task MoveToPersonal(int photoId, int thumbnailId, List<int> ownerList, List<RelationAlbumPhotoModel> relationAlbumPhotoModel, PhotoModel photoModel, List<RelationSharedAlbumPhotoModel> relationShared);
        Task MoveToGroup(int photoId, int thumbnailId, List<int> ownerList, List<RelationSharedAlbumPhotoModel> relationSharedAlbumPhotoList,
            RelationAlbumPhotoModel relationAlbumPhotoModel, PhotoModel photoModel, List<PhotoEncryptModel> photoData, List<PhotoEncryptModel> thumbnailData);
        Task AddUserToGroup(List<PhotoEncryptModel> photoEncryptModels, UserModel user);
        Task AddPhotoToSharedAlbum(List<PhotoEncryptModel> photoEncryptModels, List<RelationSharedAlbumPhotoModel> sharedAlbumPhotoModels);
        Task RemoveFromSharedAlbum(List<PhotoEncryptDeleteStruct> photoEncryptModels, List<RelationSharedAlbumPhotoModel> sharedAlbumPhotoModels);
        Task ChangeUsersInAlbum(List<PhotoEncryptDeleteStruct> encryptDeleteList, List<PhotoEncryptModel> encryptAddList, List<RelationUserSharedAlbumModel> addRelList, List<RelationUserSharedAlbumModel> removeRelList);
    }
    public class PhotoTransaction : DbWithLoggerAbstract, IPhotoTransaction
    {
        private readonly IPhotoDao _photoDao;
        private readonly IPhotoThumbnailDao _photoThumbnailDao;
        private readonly IPhotoEncryptDao _photoEncryptDao;
        private readonly IRelationAlbumPhotoDao _relationAlbumPhotoDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        private readonly IUserDao _userDao;
        public PhotoTransaction(IConfiguration configuration, IPhotoDao photoDao, IPhotoThumbnailDao photoThumbnailDao, 
            IPhotoEncryptDao photoEncryptDao, ISystemLogDao logger, IRelationAlbumPhotoDao relationAlbumPhotoDao,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IRelationUserSharedAlbumDao relationUserSharedAlbumDao,
            IUserDao userDao) 
            : base(configuration, logger)
        {
            _photoDao = photoDao;
            _photoThumbnailDao = photoThumbnailDao;
            _photoEncryptDao = photoEncryptDao;
            _relationAlbumPhotoDao = relationAlbumPhotoDao;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
            _userDao = userDao;
        }

        public async Task AddPhotoToSharedAlbum(List<PhotoEncryptModel> photoEncryptModels, List<RelationSharedAlbumPhotoModel> sharedAlbumPhotoModels)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (var photo in photoEncryptModels)
                                await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photo.ToPhotoEncryptInsert());

                            foreach (var rel in sharedAlbumPhotoModels)
                                await _relationSharedAlbumPhotoDao.Tr_InsertReturnIdentity(connection, transaction, rel.ToRelationSharedAlbumPhotoInsert());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - AddPhotoToSharedAlbum -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task AddUserToGroup(List<PhotoEncryptModel> photoEncryptModels, UserModel user)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            if(photoEncryptModels != null)
                                foreach (var photo in photoEncryptModels)
                                    await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photo.ToPhotoEncryptInsert());

                            await _userDao.Tr_UpdateRowAttached(connection, transaction, user?.ToUserUpdate());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - AddUserToGroup -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task ChangeUsersInAlbum(List<PhotoEncryptDeleteStruct> encryptDeleteList, List<PhotoEncryptModel> encryptAddList, List<RelationUserSharedAlbumModel> addRelList, List<RelationUserSharedAlbumModel> removeRelList)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            if (encryptDeleteList != null && encryptDeleteList.Any())
                                foreach (var strct in encryptDeleteList)
                                    await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, strct.FileId, (int)strct.FileTypeEnum, strct.NotDeleteUsers);

                            if (encryptAddList != null && encryptAddList.Any())
                                foreach (var photoAdd in encryptAddList)
                                    await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photoAdd.ToPhotoEncryptInsert());

                            if (addRelList != null && addRelList.Any())
                                foreach (var rel in addRelList)
                                    await _relationUserSharedAlbumDao.Tr_InsertReturnIdentity(connection, transaction, rel.ToRelationUserSharedAlbumInsert());


                            if (removeRelList != null && removeRelList.Any())
                                foreach (var rel in removeRelList)
                                    await _relationUserSharedAlbumDao.Delete(connection, transaction, rel.ToRelationSharedAlbumDelete());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - SetPhoto -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task Delete(int photoId, RelationAlbumPhotoModel relationAlbumPhotoModel, List<RelationSharedAlbumPhotoModel> sharedRelation)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            await _photoDao.Tr_Delete(connection, transaction, photoId);
                            await _photoThumbnailDao.Tr_Delete(connection, transaction, photoId);
                            await _photoEncryptDao.Tr_Delete(connection, transaction, photoId, 1);
                            await _photoEncryptDao.Tr_Delete(connection, transaction, photoId, 2);

                            if(relationAlbumPhotoModel != null)
                                await _relationAlbumPhotoDao.Delete(connection, transaction, relationAlbumPhotoModel?.ToRelationAlbumPhotoDelete());

                            if (sharedRelation != null)
                                foreach (var rel in sharedRelation)
                                    await _relationSharedAlbumPhotoDao.Delete(connection, transaction, rel?.ToRelationSharedAlbumPhotoDelete());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - SetPhoto -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task MoveToGroup(int photoId, int thumbnailId, List<int> ownerList, List<RelationSharedAlbumPhotoModel> relationSharedAlbumPhotoList, RelationAlbumPhotoModel relationAlbumPhotoModel, PhotoModel photoModel, List<PhotoEncryptModel> photoData, List<PhotoEncryptModel> thumbnailData)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Změnit fotku na personal = false
                            await _photoDao.Tr_UpdateRowAttached(connection, transaction, photoModel?.ToPhotoUpdate());
                            // Odstanit všechny uživatele fotografie až na vlastníka kvůli případnému sdílení v sharedAlbum
                            await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, photoId, 1, ownerList);
                            await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, thumbnailId, 2, ownerList);

                            foreach (var photo in photoData)
                                await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photo.ToPhotoEncryptInsert());

                            foreach (var thumbnail in thumbnailData)
                                await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, thumbnail.ToPhotoEncryptInsert());

                            // Pokud je v albu odstranit relaci
                            if (relationAlbumPhotoModel != null)
                                await _relationAlbumPhotoDao.Delete(connection, transaction, relationAlbumPhotoModel?.ToRelationAlbumPhotoDelete());

                            // Pokud je ve sdíleném albu odstranit relaci na všechna alba
                            if (relationSharedAlbumPhotoList != null && relationSharedAlbumPhotoList.Any())
                                foreach (var rel in relationSharedAlbumPhotoList)
                                    await _relationSharedAlbumPhotoDao.Delete(connection, transaction, rel?.ToRelationSharedAlbumPhotoDelete());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - SetPhoto -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task MoveToPersonal(int photoId, int thumbnailId, List<int> ownerList, List<RelationAlbumPhotoModel> relationAlbumPhotoModel, PhotoModel photoModel, List<RelationSharedAlbumPhotoModel> relationShared)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Změnit fotku na personal
                            await _photoDao.Tr_UpdateRowAttached(connection, transaction, photoModel?.ToPhotoUpdate());
                            // Odstanit všechny uživatele fotografie až na vlastníka
                            await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, photoId, 1, ownerList);
                            await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, thumbnailId, 2, ownerList);

                            // Pokud je v albu odstranit relaci
                            if (relationAlbumPhotoModel != null && relationAlbumPhotoModel.Any())
                                foreach(var rel in relationAlbumPhotoModel)
                                    await _relationAlbumPhotoDao.Delete(connection, transaction, rel?.ToRelationAlbumPhotoDelete());

                            // Pokud je ve sdíleném albu odstranit relaci
                            if (relationShared != null && relationShared.Any())
                                foreach (var rel in relationShared)
                                    await _relationSharedAlbumPhotoDao.Delete(connection, transaction, rel?.ToRelationSharedAlbumPhotoDelete());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - SetPhoto -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveFromSharedAlbum(List<PhotoEncryptDeleteStruct> photoEncryptModels, List<RelationSharedAlbumPhotoModel> sharedAlbumPhotoModels)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (var strct in photoEncryptModels)
                                await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, strct.FileId, (int)strct.FileTypeEnum, strct.NotDeleteUsers);

                            foreach (var rel in sharedAlbumPhotoModels)
                                await _relationSharedAlbumPhotoDao.Delete(connection, transaction, rel.ToRelationSharedAlbumPhotoDelete());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - AddPhotoToSharedAlbum -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SetPhoto(PhotoModel photo, PhotoThumbnailModel thumbnail, PhotoEncryptModel photoEncrypt, PhotoEncryptModel thumbnailEncrypt)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var photoId = await _photoDao.Tr_InsertReturnIdentity(connection, transaction, photo?.ToPhotoInsert());

                            thumbnail.PhotoId = photoId;
                            var thumbnailId = await _photoThumbnailDao.Tr_InsertReturnIdentity(connection, transaction, thumbnail?.ToPhotoThumbnailInsert());

                            photoEncrypt.FileId = photoId;
                            thumbnailEncrypt.FileId = thumbnailId;

                            await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photoEncrypt?.ToPhotoEncryptInsert());
                            await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, thumbnailEncrypt?.ToPhotoEncryptInsert());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"Transakce - SetPhoto -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SetPhotoMultipleUser(PhotoModel photo, PhotoThumbnailModel thumbnail, List<PhotoEncryptModel> photoEncryptList, List<PhotoEncryptModel> thumbnailEncryptList)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var photoId = await _photoDao.Tr_InsertReturnIdentity(connection, transaction, photo?.ToPhotoInsert());

                            thumbnail.PhotoId = photoId;
                            var thumbnailId = await _photoThumbnailDao.Tr_InsertReturnIdentity(connection, transaction, thumbnail?.ToPhotoThumbnailInsert());

                            photoEncryptList.ForEach(p => p.FileId = photoId);
                            thumbnailEncryptList.ForEach(t => t.FileId = thumbnailId);

                            foreach(var photoEncrypt in photoEncryptList)
                                await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, photoEncrypt?.ToPhotoEncryptInsert());

                            foreach(var thumbnailEncrypt in thumbnailEncryptList)
                                await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, thumbnailEncrypt?.ToPhotoEncryptInsert());

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"PhotoTransaction - SetPhotoMultipleUser -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
