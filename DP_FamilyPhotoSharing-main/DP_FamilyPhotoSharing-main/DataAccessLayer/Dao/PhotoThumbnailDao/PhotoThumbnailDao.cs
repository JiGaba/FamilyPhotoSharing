using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Dto.PhotoThumbnail;
using DataAccessLayer.Dto.SharedPhotoAlbum;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao.PhotoThumbnailDao
{
    public interface IPhotoThumbnailDao : IInsertReturnIdentityTransaction<PhotoThumbnailInsert>, IDeleteRowByIdTransaction
    {
        Task<PhotoThumbnailSelectByPhotoId> SelectByPhotoId(int photoId);
    }
    public class PhotoThumbnailDao : DbWithLoggerAbstract, IPhotoThumbnailDao
    {
        private const string PhotoThumbnailInsert = "PhotoThumbnailInsert";
        private const string PhotoThumbnailSelectByPhotoId = "PhotoThumbnailSelectByPhotoId";
        private const string PhotoThumbnailDelete = "PhotoThumbnailDelete";
        public PhotoThumbnailDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<PhotoThumbnailSelectByPhotoId> SelectByPhotoId(int photoId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@PhotoId", photoId);

                return await connection.QuerySingleAsync<PhotoThumbnailSelectByPhotoId>(PhotoThumbnailSelectByPhotoId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoThumbnailSelectByPhotoId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int id)
            => await connection.ExecuteAsync(PhotoThumbnailDelete, new { Id = id }, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, PhotoThumbnailInsert data)
            => await connection.QuerySingleAsync<int>(PhotoThumbnailInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
