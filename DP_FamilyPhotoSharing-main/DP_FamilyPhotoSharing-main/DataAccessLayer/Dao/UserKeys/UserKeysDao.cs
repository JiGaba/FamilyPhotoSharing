using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.SystemImages;
using DataAccessLayer.Dto.UserKeys;
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

namespace DataAccessLayer.Dao.UserKeys
{
    public interface IUserKeysDao : IInsertReturnIdentityTransaction<UserKeysInsert>
    {
        Task<UserKeysSelectByUserId> SelectByUserId(int id);
    }
    public class UserKeysDao : DbWithLoggerAbstract, IUserKeysDao
    {
        private const string UserKeysInsert = "UserKeysInsert";
        private const string UserKeysSelectByUserId = "UserKeysSelectByUserId";
        public UserKeysDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<UserKeysSelectByUserId> SelectByUserId(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", id);

                return await connection.QuerySingleAsync<UserKeysSelectByUserId>(UserKeysSelectByUserId, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUserKeys, $"{UserKeysSelectByUserId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, UserKeysInsert data)
            => await connection.QuerySingleAsync<int>(UserKeysInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
