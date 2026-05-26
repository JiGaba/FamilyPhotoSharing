using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.PhotoAlbum;
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

namespace DataAccessLayer.Dao.SharedPhotoAlbum
{
    public interface ISharedPhotoAlbumDao : IInsertReturnIdentity<SharedPhotoAlbumInsert>, 
        IUpdate<SharedPhotoAlbumUpdate>, ISelect<SharedPhotoAlbumSelect>, IDeleteRowByIdTransaction
    {
        Task<List<SharedPhotoAlbumSelectByOwnerUserId>> SelectByOwnerId(int id);
        Task<List<SharedPhotoAlbumSelectByHostUserId>> SelectByHostUserId(int id);
    }
    public class SharedPhotoAlbumDao : DbWithLoggerAbstract, ISharedPhotoAlbumDao
    {
        private const string SharedPhotoAlbumSelectByHostUserId = "SharedPhotoAlbumSelectByHostUserId";
        private const string SharedPhotoAlbumSelectByOwnerUserId = "SharedPhotoAlbumSelectByOwnerUserId";
        private const string SharedPhotoAlbumInsert = "SharedPhotoAlbumInsert";
        private const string SharedPhotoAlbumUpdate = "SharedPhotoAlbumUpdate";
        private const string SharedPhotoAlbumSelect = "SharedPhotoAlbumSelect";
        private const string SharedPhotoAlbumDelete = "SharedPhotoAlbumDelete";
        public SharedPhotoAlbumDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<int> InsertReturnIdentity(SharedPhotoAlbumInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                return await connection.QuerySingleAsync<int>(SharedPhotoAlbumInsert, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.AddSharedPhotoAlbum, $"{SharedPhotoAlbumInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<SharedPhotoAlbumSelectByHostUserId>> SelectByHostUserId(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@HostUserId", id);

                var result = await connection.QueryAsync<SharedPhotoAlbumSelectByHostUserId>(SharedPhotoAlbumSelectByHostUserId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSharedPhotoAlbum, $"{SharedPhotoAlbumSelect} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<SharedPhotoAlbumSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return await connection.QuerySingleOrDefaultAsync<SharedPhotoAlbumSelect>(SharedPhotoAlbumSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSharedPhotoAlbum, $"{SharedPhotoAlbumSelect} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<SharedPhotoAlbumSelectByOwnerUserId>> SelectByOwnerId(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@OwnerUserId", id);

                var result = await connection.QueryAsync<SharedPhotoAlbumSelectByOwnerUserId>(SharedPhotoAlbumSelectByOwnerUserId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSharedPhotoAlbum, $"{SharedPhotoAlbumSelectByOwnerUserId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int id)
            => await connection.ExecuteAsync(SharedPhotoAlbumDelete, new { Id = id }, transaction, commandType: CommandType.StoredProcedure);

        public async Task Update(SharedPhotoAlbumUpdate data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(SharedPhotoAlbumUpdate, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.EditSharedPhotoAlbum, $"{SharedPhotoAlbumUpdate} -> {e.Message}", 0, 0);
                throw;
            }
        }
    }
}
