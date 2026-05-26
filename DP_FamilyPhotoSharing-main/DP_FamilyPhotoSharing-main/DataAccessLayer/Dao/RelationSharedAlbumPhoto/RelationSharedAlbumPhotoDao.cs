using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.PhotoEncrypt;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
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

namespace DataAccessLayer.Dao.RelationSharedAlbumPhoto
{
    public interface IRelationSharedAlbumPhotoDao : IInsertReturnIdentityTransaction<RelationSharedAlbumPhotoInsert>
    {
        Task Delete(SqlConnection connection, IDbTransaction transaction, RelationSharedAlbumPhotoDelete delete);
        Task<List<RelationSharedAlbumPhotoSelectByPhotoId>> SelectByPhotoId(int photoId);
        Task<List<RelationSharedAlbumPhotoSelectBySharedAlbumId>> SelectBySharedAlbumId(int albumId);
    }
    public class RelationSharedAlbumPhotoDao : DbWithLoggerAbstract, IRelationSharedAlbumPhotoDao
    {
        private const string RelationSharedAlbumPhotoInsert = "RelationSharedAlbumPhotoInsert";
        private const string RelationSharedAlbumPhotoDelete = "RelationSharedAlbumPhotoDelete";
        private const string RelationSharedAlbumPhotoSelectByPhotoId = "RelationSharedAlbumPhotoSelectByPhotoId";
        private const string RelationSharedAlbumPhotoSelectBySharedAlbumId = "RelationSharedAlbumPhotoSelectBySharedAlbumId";
        public RelationSharedAlbumPhotoDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<List<RelationSharedAlbumPhotoSelectByPhotoId>> SelectByPhotoId(int photoId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@PhotoId", photoId);

                var result = await connection.QueryAsync<RelationSharedAlbumPhotoSelectByPhotoId>(RelationSharedAlbumPhotoSelectByPhotoId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSharedPhotoAlbum, $"{RelationSharedAlbumPhotoSelectByPhotoId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<RelationSharedAlbumPhotoSelectBySharedAlbumId>> SelectBySharedAlbumId(int albumId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@SharedAlbumId", albumId);

                var result = await connection.QueryAsync<RelationSharedAlbumPhotoSelectBySharedAlbumId>(RelationSharedAlbumPhotoSelectBySharedAlbumId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSharedPhotoAlbum, $"{RelationSharedAlbumPhotoSelectBySharedAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Delete(SqlConnection connection, IDbTransaction transaction, RelationSharedAlbumPhotoDelete delete)
            => await connection.ExecuteAsync(RelationSharedAlbumPhotoDelete, delete, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, RelationSharedAlbumPhotoInsert data)
            => await connection.QuerySingleAsync<int>(RelationSharedAlbumPhotoInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
