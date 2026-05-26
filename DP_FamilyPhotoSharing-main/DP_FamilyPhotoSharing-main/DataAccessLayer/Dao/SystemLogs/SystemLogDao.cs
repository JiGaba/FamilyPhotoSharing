using Dapper;
using DataAccessLayer.Dto.Groups;
using DataAccessLayer.Dto.SystemLog;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao.Logs
{
    public interface ISystemLogDao : IInsert<SystemLogInsert>
    {
        Task<List<SystemLogSelectByParameters>> ISelectListByParameters(int userId, int groupId, int actionType, int logType, int offset, int limit);
        Task<SystemLogSelectCountByParameters> ISelectCountByParameters(int userId, int groupId, int actionType, int logType);
    }
    public class SystemLogDao : DatabaseAbstract, ISystemLogDao
    {
        private const string SystemLogInsert = "SystemLogInsert";
        private const string SystemsLogSelectByParameters = "SystemsLogSelectByParameters";
        private const string SystemLogSelectCountByParameters = "SystemLogSelectCountByParameters";
        public SystemLogDao(IConfiguration configuration) : base(configuration) { }

        public async Task Insert(SystemLogInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(SystemLogInsert, data, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<SystemLogSelectCountByParameters> ISelectCountByParameters(int userId, int groupId, int actionType, int logType)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);
                parameters.Add("@FilterGroup", groupId == 0 ? null : "true");
                parameters.Add("@UserId", userId);
                parameters.Add("@FilterUser", userId == 0 ? null : "true");
                parameters.Add("@LogType", logType);
                parameters.Add("@FilterLogType", logType == 0 ? null : "true");
                parameters.Add("@ActionType", actionType);
                parameters.Add("@FilterActionType", actionType == 0 ? null : "true");

                return await connection.QuerySingleAsync<SystemLogSelectCountByParameters>(SystemLogSelectCountByParameters, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<SystemLogSelectByParameters>> ISelectListByParameters(int userId, int groupId, int actionType, int logType, int offset, int limit)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@GroupId", groupId);
                parameters.Add("@FilterGroup", groupId == 0 ? null : "true");
                parameters.Add("@UserId", userId);
                parameters.Add("@FilterUser", userId == 0 ? null : "true");
                parameters.Add("@LogType", logType);
                parameters.Add("@FilterLogType", logType == 0 ? null : "true");
                parameters.Add("@ActionType", actionType);
                parameters.Add("@FilterActionType", actionType == 0 ? null : "true");
                parameters.Add("@Offset", offset);
                parameters.Add("@Limit", limit);

                var result = await connection.QueryAsync<SystemLogSelectByParameters>(SystemsLogSelectByParameters, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
