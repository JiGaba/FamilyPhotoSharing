using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Dto.UserKeys;
using DataAccessLayer.Dto.UserRefreshToken;
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

namespace DataAccessLayer.Dao.UserRefreshToken
{
    public interface IUserRefreshTokenDao : IInsertReturnIdentity<UserRefreshTokenInsert>, IUpdate<UserRefreshTokenUpdate>
    {
        Task<UserRefreshTokenSelectByToken> SelectByToken(string token);
    }
    public class UserRefreshTokenDao : DbWithLoggerAbstract, IUserRefreshTokenDao
    {
        private const string UserRefreshTokenInsert = "UserRefreshTokenInsert";
        private const string UserRefreshTokenUpdate = "UserRefreshTokenUpdate";
        private const string UserRefreshTokenSelectByToken = "UserRefreshTokenSelectByToken";
        public UserRefreshTokenDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<int> InsertReturnIdentity(UserRefreshTokenInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                return await connection.QuerySingleAsync<int>(UserRefreshTokenInsert, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.AddRefreshToken, $"{UserRefreshTokenInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<UserRefreshTokenSelectByToken> SelectByToken(string token)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Token", token);

                return await connection.QuerySingleAsync<UserRefreshTokenSelectByToken>(UserRefreshTokenSelectByToken, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.ShowRefreshToken, $"{UserRefreshTokenSelectByToken} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Update(UserRefreshTokenUpdate data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(UserRefreshTokenUpdate, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.UpdateRefreshToken, $"{UserRefreshTokenUpdate} -> {e.Message}", 0, 0);
                throw;
            }
        }
    }
}
