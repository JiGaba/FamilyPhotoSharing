using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Dto.RelationAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccessLayer.Dao.RelationAlbumPhoto
{
    public interface IRelationAlbumPhotoDao : IInsertReturnIdentityTransaction<RelationAlbumPhotoInsert>
    {
        Task Delete(SqlConnection connection, IDbTransaction transaction, RelationAlbumPhotoDelete delete);
        Task<RelationAlbumPhotoSelectByPhotoId> SelectByPhotoId(int photoId);
    }
    public class RelationAlbumPhotoDao : DbWithLoggerAbstract, IRelationAlbumPhotoDao
    {
        private const string RelationAlbumPhotoInsert = "RelationAlbumPhotoInsert";
        private const string RelationAlbumPhotoDelete = "RelationAlbumPhotoDelete";
        private const string RelationAlbumPhotoSelectByPhotoId = "RelationAlbumPhotoSelectByPhotoId";
        public RelationAlbumPhotoDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task Delete(SqlConnection connection, IDbTransaction transaction, RelationAlbumPhotoDelete delete)
            => await connection.ExecuteAsync(RelationAlbumPhotoDelete, delete, transaction, commandType: CommandType.StoredProcedure);

        public async Task<RelationAlbumPhotoSelectByPhotoId> SelectByPhotoId(int photoId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@PhotoId", photoId);

                return await connection.QueryFirstOrDefaultAsync<RelationAlbumPhotoSelectByPhotoId>(RelationAlbumPhotoSelectByPhotoId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoAlbum, $"{RelationAlbumPhotoSelectByPhotoId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, RelationAlbumPhotoInsert data)
            => await connection.QuerySingleAsync<int>(RelationAlbumPhotoInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
