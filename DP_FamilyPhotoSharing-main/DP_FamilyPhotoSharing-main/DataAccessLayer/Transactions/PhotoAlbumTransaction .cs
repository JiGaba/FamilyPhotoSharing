using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.PhotoAlbum;
using DataAccessLayer.Dao.PhotoEncrypt;
using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Dao.SharedPhotoAlbum;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.RelationUserSharedAlbum;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Transactions
{
    public interface IPhotoAlbumTransaction
    {
        Task DeletePhotoAlbum(List<RelationAlbumPhotoModel> relation, int albumId);
        Task DeleteSharedPhotoAlbum(int albumId, List<PhotoEncryptDeleteStruct> photoEncrypt, 
            List<RelationSharedAlbumPhotoDelete> relAlbumPhoto, List<RelationSharedAlbumDelete> relAlbumUser);
    }
    public class PhotoAlbumTransaction : DbWithLoggerAbstract, IPhotoAlbumTransaction
    {
        private readonly IRelationAlbumPhotoDao _relationAlbumPhotoDao;
        private readonly IPhotoAlbumDao _photoAlbumDao;
        private readonly ISharedPhotoAlbumDao _sharedPhotoAlbumDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedPhotoDao;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        private readonly IPhotoEncryptDao _photoEncryptDao;
        public PhotoAlbumTransaction(IConfiguration configuration, ISystemLogDao logger, ISharedPhotoAlbumDao sharedPhotoAlbumDao,
            IRelationAlbumPhotoDao relationAlbumPhotoDao, IPhotoAlbumDao photoAlbumDao, IRelationUserSharedAlbumDao relationUserSharedAlbumDao,
            IRelationSharedAlbumPhotoDao relationSharedPhotoDao, IPhotoEncryptDao photoEncryptDao) : base(configuration, logger)
        {
            _relationAlbumPhotoDao = relationAlbumPhotoDao;
            _photoAlbumDao = photoAlbumDao;
            _relationSharedPhotoDao = relationSharedPhotoDao;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
            _photoEncryptDao = photoEncryptDao;
            _sharedPhotoAlbumDao = sharedPhotoAlbumDao;
        }

        public async Task DeletePhotoAlbum(List<RelationAlbumPhotoModel> relation, int albumId)
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
                            if (relation != null && relation.Any())
                                foreach (var rel in relation)
                                    await _relationAlbumPhotoDao
                                        .Delete(connection, transaction, rel?.ToRelationAlbumPhotoDelete());

                            await _photoAlbumDao.Tr_Delete(connection, transaction, albumId);

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"GalleryTransaction - MovePersonal -> {e.Message}", 0, 0);
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

        public async Task DeleteSharedPhotoAlbum(int albumId, List<PhotoEncryptDeleteStruct> photoEncrypt, List<RelationSharedAlbumPhotoDelete> relAlbumPhoto, List<RelationSharedAlbumDelete> relAlbumUser)
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
                            if (photoEncrypt != null && photoEncrypt.Any())
                                foreach (var rel in photoEncrypt)
                                    await _photoEncryptDao.Tr_DeleteByNotUserId(connection, transaction, 
                                        rel.FileId, (int)rel.FileTypeEnum, rel.NotDeleteUsers);

                            if (relAlbumPhoto != null && relAlbumPhoto.Any())
                                foreach (var rel in relAlbumPhoto)
                                    await _relationSharedPhotoDao.Delete(connection, transaction, rel);

                            if (relAlbumUser != null && relAlbumUser.Any())
                                foreach (var rel in relAlbumUser)
                                    await _relationUserSharedAlbumDao.Delete(connection, transaction, rel);

                            await _sharedPhotoAlbumDao.Tr_Delete(connection, transaction, albumId);

                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"GalleryTransaction - MovePersonal -> {e.Message}", 0, 0);
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
