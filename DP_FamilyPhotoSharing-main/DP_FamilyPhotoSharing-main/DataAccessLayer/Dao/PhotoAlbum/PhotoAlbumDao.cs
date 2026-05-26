using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Groups;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Dto.PhotoAlbum;
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

namespace DataAccessLayer.Dao.PhotoAlbum
{
    public interface IPhotoAlbumDao : IInsertReturnIdentity<PhotoAlbumInsert>, IUpdate<PhotoAlbumUpdate>, 
        ISelect<PhotoAlbumSelect>, IDeleteRowByIdTransaction, IUpdateRowAttachedTransaction<PhotoAlbumUpdate>
    {
        Task<List<PhotoAlbumSelectByOwnerUserId>> SelectByOwnerId(int id);
        Task<List<PhotoAlbumSelectByGroupsId>> SelectByGroupsId(int id);
    }
    public class PhotoAlbumDao : DbWithLoggerAbstract, IPhotoAlbumDao
    {
        private const string PhotoAlbumSelectByOwnerUserId = "PhotoAlbumSelectByOwnerUserId";
        private const string PhotoAlbumSelectByUserGroupsId = "PhotoAlbumSelectByUserGroupsId";
        private const string PhotoAlbumInsert = "PhotoAlbumInsert";
        private const string PhotoAlbumUpdate = "PhotoAlbumUpdate";
        private const string PhotoAlbumSelect = "PhotoAlbumSelect";
        private const string PhotoAlbumDelete = "PhotoAlbumDelete";
        public PhotoAlbumDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<int> InsertReturnIdentity(PhotoAlbumInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                return await connection.QuerySingleAsync<int>(PhotoAlbumInsert, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.AddPhotoAlbum, $"{PhotoAlbumInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<PhotoAlbumSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return await connection.QuerySingleAsync<PhotoAlbumSelect>(PhotoAlbumSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoAlbum, $"{PhotoAlbumSelect} -> {e.Message}", 0, 0);
                throw;
            }
        }
        public async Task<List<PhotoAlbumSelectByOwnerUserId>> SelectByOwnerId(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@OwnerUserId", id);

                var result = await connection.QueryAsync<PhotoAlbumSelectByOwnerUserId>(PhotoAlbumSelectByOwnerUserId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoAlbum, $"{PhotoAlbumSelectByOwnerUserId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Update(PhotoAlbumUpdate data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(PhotoAlbumUpdate, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.EditPhotoAlbum, $"{PhotoAlbumUpdate} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<PhotoAlbumSelectByGroupsId>> SelectByGroupsId(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserGroupsId", id);

                var result = await connection.QueryAsync<PhotoAlbumSelectByGroupsId>(PhotoAlbumSelectByUserGroupsId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhotoAlbum, $"{PhotoAlbumSelectByUserGroupsId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int id)
            => await connection.ExecuteAsync(PhotoAlbumDelete, new { Id = id }, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_UpdateRowAttached(SqlConnection connection, IDbTransaction transaction, PhotoAlbumUpdate data)
            => await connection.ExecuteAsync(PhotoAlbumUpdate, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
