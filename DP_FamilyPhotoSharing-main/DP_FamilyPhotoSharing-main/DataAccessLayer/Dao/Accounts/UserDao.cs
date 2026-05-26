using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Extensions;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao.Accounts
{
    public interface IUserDao : ISelect<UserSelect>, IInsertReturnIdentityTransaction<UserInsert>, ISelectList<UserSelectAll>, IUpdateRowAttachedTransaction<UserUpdate>
    {
        Task<List<UserSelectByGroupIdActiveRoleId>> SelectUserByGroupIdActiveRoleId(int groupId, bool active, List<int> roleId, bool activated = true);
        Task<UserSelectByLogin> SelectByLogin(string userName);
        Task<List<UserSelectCountByGroupId>> SelectUserCountByGroupId(int groupId);
        Task UpdatePassword(UserUpdatePassword data);
        Task UpdateBackupKey(UserUpdateBackupKey data);
    }
    public class UserDao : DbWithLoggerAbstract, IUserDao
    {
        private const string UserInsert = "UserInsert";
        private const string UserUpdate = "UserUpdate";
        private const string UserSelectByLogin = "UserSelectByLogin";
        private const string UserSelect = "UserSelect";
        private const string UserSelectAll = "UserSelectAll";
        private const string UserSelectByGroupIdActiveRoleId = "UserSelectByGroupIdActiveRoleId";
        private const string UserSelectCountByGroupId = "UserSelectCountByGroupId";
        private const string UserUpdatePassword = "UserUpdatePassword";
        private const string UserUpdateBackupKey = "UserUpdateBackupKey";
        public UserDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<UserSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", id);

                return await connection.QuerySingleOrDefaultAsync<UserSelect>(UserSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUser, $"{UserSelect} -> UserId {id} {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<UserSelectByLogin?> SelectByLogin(string userLogin)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@UserLogin", userLogin);

                return await connection.QuerySingleOrDefaultAsync<UserSelectByLogin>(UserSelectByLogin, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUser, $"{UserSelectByLogin} -> UserLogin {userLogin} {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<UserSelectAll>> SelectList(int id = 0)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", id);
                parameters.Add("@Filter", id == 0 ? null : "true");

                var result = await connection.QueryAsync<UserSelectAll>(UserSelectAll, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUser, $"{UserSelectAll} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, UserInsert data)
            => await connection.QuerySingleAsync<int>(UserInsert, data, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_UpdateRowAttached(SqlConnection connection, IDbTransaction transaction, UserUpdate data)
            => await connection.ExecuteAsync(UserUpdate, data, transaction, commandType: CommandType.StoredProcedure);

        public async Task<List<UserSelectByGroupIdActiveRoleId>> SelectUserByGroupIdActiveRoleId(int groupId, bool active, List<int> roleId, bool activated = true)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();


                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);
                parameters.Add("@Active", active ? 1 : 0);
                parameters.Add("@RoleIds", string.Join(",", roleId));
                parameters.Add("@Activated", activated ? 1 : 0);

                var result = await connection.QueryAsync<UserSelectByGroupIdActiveRoleId>(UserSelectByGroupIdActiveRoleId, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUser, $"{UserSelectByGroupIdActiveRoleId} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<UserSelectCountByGroupId>> SelectUserCountByGroupId(int groupId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);

                var tesult = await connection.QueryAsync<UserSelectCountByGroupId>(UserSelectCountByGroupId, parameters, commandType: CommandType.StoredProcedure);
                
                return tesult.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectUser, $"{UserSelectCountByGroupId} -> GroupId {groupId} {e.Message}", 0, 0);
                throw;
            }
        }
        public async Task UpdatePassword(UserUpdatePassword data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(UserUpdatePassword, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.EditUser, $"{UserUpdatePassword} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task UpdateBackupKey(UserUpdateBackupKey data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(UserUpdateBackupKey, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.EditUser, $"{UserUpdateBackupKey} -> {e.Message}", 0, 0);
                throw;
            }
        }
    }
}
