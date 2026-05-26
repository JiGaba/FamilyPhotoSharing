using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao.Photo
{
    public interface IPhotoDao : IInsertReturnIdentityTransaction<PhotoInsert>, ISelect<PhotoSelect>, IDeleteRowByIdTransaction, IUpdateRowAttachedTransaction<PhotoUpdate>
    {
        Task<List<PhotoSelectByOwnerIdAndPersonal>> SelectByOwnerAndPersonal(int ownerId, bool personal, int fetch, int offset);
        Task<List<PhotoSelectByAlbumId>> SelectByAlbumId(int albumId, int fetch, int offset);
        Task<List<PhotoSelectBySharedAlbumId>> SelectBySharedAlbumId(int sharedAlbumId, int fetch, int offset);
        Task<List<PhotoSelectByGroupId>> SelectByGroupId(int groupId, int fetch, int offset);
        Task<PhotoSelectCountByOwnerAndPersonal> GetPhotoCount(int ownerId, bool personal);
        Task<PhotoSelectCountByAlbumId> GetPhotoCountByAlbumId(int albumId);
        Task<PhotoSelectCountByGroupId> GetPhotoCountByGroupId(int groupId);
        Task<PhotoSelectCountBySharedAlbumId> GetPhotoCountByShardAlbumId(int sharedAlbumId);
    }
    public class PhotoDao : DbWithLoggerAbstract, IPhotoDao
    {
        private const string PhotoSelectByOwnerIdAndPersonal = "PhotoSelectByOwnerIdAndPersonal";
        private const string PhotoInsert = "PhotoInsert";
        private const string PhotoSelect = "PhotoSelect";
        private const string PhotoSelectCountByOwnerAndPersonal = "PhotoSelectCountByOwnerAndPersonal";
        private const string PhotoSelectCountByAlbumId = "PhotoSelectCountByAlbumId";
        private const string PhotoSelectCountBySharedAlbumId = "PhotoSelectCountBySharedAlbumId";
        private const string PhotoSelectByAlbumId = "PhotoSelectByAlbumId";
        private const string PhotoSelectBySharedAlbumId = "PhotoSelectBySharedAlbumId";
        private const string PhotoSelectByGroupId = "PhotoSelectByGroupId";
        private const string PhotoDelete = "PhotoDelete";
        private const string PhotoSelectCountByGroupId = "PhotoSelectCountByGroupId";
        private const string PhotoUpdate = "PhotoUpdate";
        public PhotoDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }
        public async Task<List<PhotoSelectByOwnerIdAndPersonal>> SelectByOwnerAndPersonal(int ownerId, bool personal, int fetch, int offset)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@OwnerId", ownerId);
                parameters.Add("@Personal", personal);
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);

                var result = await connection.QueryAsync<PhotoSelectByOwnerIdAndPersonal>(PhotoSelectByOwnerIdAndPersonal, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectByOwnerIdAndPersonal} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<PhotoSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return await connection.QuerySingleAsync<PhotoSelect>(PhotoSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{SelectById} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, PhotoInsert data)
            => await connection.QuerySingleAsync<int>(PhotoInsert, data, transaction, commandType: CommandType.StoredProcedure);

        public async Task<PhotoSelectCountByOwnerAndPersonal> GetPhotoCount(int ownerId, bool personal)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@OwnerId", ownerId);
                parameters.Add("@Personal", personal);

                return await connection.QuerySingleAsync<PhotoSelectCountByOwnerAndPersonal>(PhotoSelectCountByOwnerAndPersonal, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectCountByOwnerAndPersonal} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<PhotoSelectCountByAlbumId> GetPhotoCountByAlbumId(int albumId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@AlbumId", albumId);

                return await connection.QuerySingleAsync<PhotoSelectCountByAlbumId>(PhotoSelectCountByAlbumId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectCountByAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<PhotoSelectCountBySharedAlbumId> GetPhotoCountByShardAlbumId(int sharedAlbumId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@SharedAlbumId", sharedAlbumId);

                return await connection.QuerySingleAsync<PhotoSelectCountBySharedAlbumId>(PhotoSelectCountBySharedAlbumId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectCountBySharedAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<PhotoSelectByAlbumId>> SelectByAlbumId(int albumId, int fetch, int offset)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@AlbumId", albumId);
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);

                var result = await connection.QueryAsync<PhotoSelectByAlbumId>(PhotoSelectByAlbumId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectByAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<PhotoSelectBySharedAlbumId>> SelectBySharedAlbumId(int sharedAlbumId, int fetch, int offset)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@SharedAlbumId", sharedAlbumId);
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);

                var result = await connection.QueryAsync<PhotoSelectBySharedAlbumId>(PhotoSelectBySharedAlbumId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectBySharedAlbumId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<PhotoSelectByGroupId>> SelectByGroupId(int groupId, int fetch, int offset)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);

                var result = await connection.QueryAsync<PhotoSelectByGroupId>(PhotoSelectByGroupId, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectByGroupId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int id)
            => await connection.ExecuteAsync(PhotoDelete, new { Id = id }, transaction, commandType: CommandType.StoredProcedure);

        public async Task<PhotoSelectCountByGroupId> GetPhotoCountByGroupId(int groupId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);

                return await connection.QuerySingleAsync<PhotoSelectCountByGroupId>(PhotoSelectCountByGroupId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectPhoto, $"{PhotoSelectCountByGroupId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_UpdateRowAttached(SqlConnection connection, IDbTransaction transaction, PhotoUpdate data)
            => await connection.ExecuteAsync(PhotoUpdate, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
