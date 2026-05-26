using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Dto.PhotoEncrypt;
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

namespace DataAccessLayer.Dao.PhotoEncrypt
{
    public interface IPhotoEncryptDao : IInsertReturnIdentityTransaction<PhotoEncryptInsert>
    {
        Task<PhotoEncryptSelectByUserIdPhotoId> SelectByUserIdPhotoId(int userId, int fileId, Int16 fileType);
        Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int fileId, int fileType);
        Task Tr_DeleteByNotUserId(SqlConnection connection, IDbTransaction transaction, int fileId, int fileType, List<int> userIda);
    }
    public class PhotoEncryptDao : DbWithLoggerAbstract, IPhotoEncryptDao
    {
        private const string PhotoEncryptInsert = "PhotoEncryptInsert";
        private const string PhotoEncryptSelectByUserIdPhotoId = "PhotoEncryptSelectByUserIdPhotoId";
        private const string PhotoEncryptDelete = "PhotoEncryptDelete";
        private const string PhotoEncryptDeleteByNotUserId = "PhotoEncryptDeleteByNotUserId";
        public PhotoEncryptDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<PhotoEncryptSelectByUserIdPhotoId> SelectByUserIdPhotoId(int userId, int fileId, Int16 fileType)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@FileId", fileId);
                parameters.Add("@FileType", fileType);

                return await connection.QuerySingleOrDefaultAsync<PhotoEncryptSelectByUserIdPhotoId>(PhotoEncryptSelectByUserIdPhotoId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoEncrypt, $"{PhotoEncryptSelectByUserIdPhotoId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int fileId, int fileType)
            => await connection.ExecuteAsync(PhotoEncryptDelete, new { FileId = fileId, FIleType = fileType }, transaction, commandType: CommandType.StoredProcedure);

        public async Task Tr_DeleteByNotUserId(SqlConnection connection, IDbTransaction transaction, int fileId, int fileType, List<int> userIds)
            => await connection.ExecuteAsync(PhotoEncryptDeleteByNotUserId, new { FileId = fileId, FIleType = fileType, UserIds = string.Join(",", userIds) }, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, PhotoEncryptInsert data)
            => await connection.QuerySingleAsync<int>(PhotoEncryptInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
