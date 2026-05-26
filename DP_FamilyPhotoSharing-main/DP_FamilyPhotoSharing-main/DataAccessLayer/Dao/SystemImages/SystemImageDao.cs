using Dapper;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.Photo;
using DataAccessLayer.Dto.PhotoEncrypt;
using DataAccessLayer.Dto.SystemImages;
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

namespace DataAccessLayer.Dao.SystemImages
{
    public interface ISystemImageDao : IInsert<SystemImagesInsert>, ISelect<SystemImagesSelect>, IInsertReturnIdentityTransaction<SystemImagesInsert>
    {
        Task<int> Tr_DeleteRowById(SqlConnection connection, IDbTransaction transaction, int id);
    }
    public class SystemImageDao : DbWithLoggerAbstract, ISystemImageDao
    {
        private const string SystemImagesSelect = "SystemImagesSelect";
        private const string SystemImagesInsert = "SystemImagesInsert";
        private const string SystemImagesDelete = "SystemImagesDelete";

        public SystemImageDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task Insert(SystemImagesInsert data)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters(data);

                await connection.ExecuteAsync(SystemImagesInsert, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.AddSystemImages, $"{SystemImagesInsert} -> {e.Message}", 0, 0);
                throw;
            }
        }

        public async Task<SystemImagesSelect> SelectById(int id)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                return await connection.QuerySingleAsync<SystemImagesSelect>(SystemImagesSelect, parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                await Log(LogTypeEnum.Error, ActionTypeEnum.SelectSystemImages, $"{SystemImagesSelect} -> {e.Message}", 0, 0);
                throw;
            }
        }
        public async Task<int> Tr_DeleteRowById(SqlConnection connection, IDbTransaction transaction, int id)
            => await connection.ExecuteAsync(SystemImagesDelete, new {Id = id}, transaction, commandType: CommandType.StoredProcedure);

        public async Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, SystemImagesInsert data)
            => await connection.QuerySingleAsync<int>(SystemImagesInsert, data, transaction, commandType: CommandType.StoredProcedure);
    }
}
