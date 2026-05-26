using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using DataAccessLayer.Dao.Logs;

namespace DataAccessLayer.Dao
{
    public interface ITestDao
    {
        Task<bool> TestDbConnection();
    }
    public class TestDao : DbWithLoggerAbstract, ITestDao
    {
        public TestDao(IConfiguration configuration, ISystemLogDao logger) : base(configuration, logger) { }

        public async Task<bool> TestDbConnection()
        {  
            try
            {
                using SqlConnection connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var result = await connection.ExecuteScalarAsync<int>("SELECT 1");
                return result == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
