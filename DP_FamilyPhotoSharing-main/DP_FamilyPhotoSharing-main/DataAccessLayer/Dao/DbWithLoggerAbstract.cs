using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.SystemLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dao
{
    public abstract class DbWithLoggerAbstract : DatabaseAbstract
    {
        private readonly ISystemLogDao _logger;
        public DbWithLoggerAbstract(IConfiguration configuration, ISystemLogDao logger) : base(configuration)
        {
            _logger = logger;
        }
        protected async Task Log(LogTypeEnum logType, ActionTypeEnum actionType, string description, int autorId, int groupId)
        {
            var log = new SystemLogModel(logType, actionType, description, autorId, groupId).ToSystemLogInsert();
            await _logger.Insert(log);
        }
    }
}
