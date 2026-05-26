using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.RelationUserSharedAlbum;
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

namespace DataAccessLayer.Dao.RelationUserSharedAlbum
{
    public interface IRelationUserSharedAlbumDao : IInsertReturnIdentityTransaction<RelationUserSharedAlbumInsert>
    {
        Task Delete(SqlConnection connection, IDbTransaction transaction, RelationSharedAlbumDelete delete);
        Task<List<RelationUserSharedAlbumSelectByAlbumId>> SelectBySharedAlbumId(int sharedAlbumId);
    }
    public class RelationUserSharedAlbumDao : DbWithLoggerAbstract, IRelationUserSharedAlbumDao
    {
        private const string RelationUserSharedAlbumInsert = "RelationUserSharedAlbumInsert";
        private const string RelationSharedAlbumDelete = "RelationUserSharedAlbumDelete";
        private const string RelationUserSharedAlbumSelectByAlbumId = "RelationUserSharedAlbumSelectByAlbumId";
        public RelationUserSharedAlbumDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task Delete(SqlConnection connection, IDbTransaction transaction, RelationSharedAlbumDelete delete)
            => await connection.ExecuteAsync(RelationSharedAlbumDelete, delete, transaction, commandType: CommandType.StoredProcedure);

        public  async Task<List<RelationUserSharedAlbumSelectByAlbumId>> SelectBySharedAlbumId(int sharedAlbumId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@SharedAlbumId", sharedAlbumId);

                var result = await connection.QueryAsync<RelationUserSharedAlbumSelectByAlbumId>(RelationUserSharedAlbumSelectByAlbumId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoEncrypt, $"{RelationUserSharedAlbumSelectByAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, RelationUserSharedAlbumInsert data)
            => await connection.QuerySingleAsync<int>(RelationUserSharedAlbumInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
