using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.Groups;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao.Groups
{
    public interface IUserGroupDao : IInsertReturnIdentity<UserGroupsInsert>,
        ISelectList<UserGroupsSelectAll>, ISelect<UserGroupsSelect>, IUpdate<UserGroupsUpdate>
    {
        Task DeleteGroupById(int id);
    }

    public class UserGroupDao : DbWithLoggerAbstract, IUserGroupDao
    {
        private const string UserGroupsInsert = "UserGroupsInsert";
        private const string UserGroupsSelectAll = "UserGroupsSelectAll";
        private const string UserGroupsSelect = "UserGroupsSelect";
        private const string UserGroupsUpdate = "UserGroupsUpdate";
        private const string UserGroupsDelete = "UserGroupsDelete";
        public UserGroupDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task DeleteGroupById(int id)
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                await connection.ExecuteAsync(UserGroupsDelete, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51001) // Skupina nelze smazat, obsahuje uživatele!
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.RemoveGroup, $"{UserGroupsInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<int> InsertReturnIdentity(UserGroupsInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                return await connection.QuerySingleAsync<int>(UserGroupsInsert, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.AddGroup, $"{UserGroupsInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<UserGroupsSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", id);

                return await connection.QuerySingleAsync<UserGroupsSelect>(UserGroupsSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectGroup, $"{SelectById} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<List<UserGroupsSelectAll>> SelectList(int id = 0)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var result = await connection.QueryAsync<UserGroupsSelectAll>(UserGroupsSelectAll, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectGroup, $"{SelectList} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task Update(UserGroupsUpdate data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(UserGroupsUpdate, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.EditGroup, $"{UserGroupsUpdate} -> {e.Message}", 0, 0);
                throw;
            }
        }
    }
}
