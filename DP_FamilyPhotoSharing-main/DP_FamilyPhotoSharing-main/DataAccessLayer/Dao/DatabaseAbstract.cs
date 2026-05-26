using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.SystemLog;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao
{
    public abstract class DatabaseAbstract
    {
        protected readonly IConfiguration _configuration;
        public DatabaseAbstract(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        protected string? GetConnectionString() => _configuration.GetConnectionString("DbConnection");
        protected string? GetConnectionMasterString() => _configuration.GetConnectionString("DbConnectionMaster");
    }
}
