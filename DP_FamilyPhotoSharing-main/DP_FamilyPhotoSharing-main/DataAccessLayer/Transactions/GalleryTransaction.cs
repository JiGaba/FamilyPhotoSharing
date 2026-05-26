using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.PhotoAlbum;
using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Transactions
{
    public interface IGalleryTransaction
    {
        Task MovePersonal(List<RelationAlbumPhotoModel> relationDelete, List<RelationAlbumPhotoModel> relationInsert, PhotoAlbumModel? photoAlbum);
    }
    public class GalleryTransaction : DbWithLoggerAbstract, IGalleryTransaction
    {
        private readonly IRelationAlbumPhotoDao _relationAlbumPhotoDao;
        private readonly IPhotoAlbumDao _photoAlbumDao;
        public GalleryTransaction(IConfiguration configuration, ISystemLogDao logger, IRelationAlbumPhotoDao relationAlbumPhotoDao,
            IPhotoAlbumDao photoAlbumDao) : base(configuration, logger)
        {
            _relationAlbumPhotoDao = relationAlbumPhotoDao;
            _photoAlbumDao = photoAlbumDao;
        }

        public async Task MovePersonal(List<RelationAlbumPhotoModel> relationDelete, List<RelationAlbumPhotoModel> relationInsert, PhotoAlbumModel? photoAlbum)
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
                            if(relationDelete != null && relationDelete.Any())
                                foreach (var relationAlbumPhotoModel in relationDelete)
                                    await _relationAlbumPhotoDao
                                        .Delete(connection, transaction, relationAlbumPhotoModel?.ToRelationAlbumPhotoDelete());

                            if (relationInsert != null && relationInsert.Any())
                                foreach (var relationAlbumPhotoModel in relationInsert)
                                    await _relationAlbumPhotoDao
                                        .Tr_InsertReturnIdentity(connection, transaction, relationAlbumPhotoModel?.ToRelationAlbumPhotoInsert());

                            if (photoAlbum != null)
                                await _photoAlbumDao.Tr_UpdateRowAttached(connection, transaction, photoAlbum?.ToPhotoAlbumUpdate());

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
