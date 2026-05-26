using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dto.SystemLog;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Logs
{
    public interface ISystemLogService : ISetValue<SystemLogModel> 
    {
        Task<List<SystemLogModel>> IGetListByParameters(int userId, int groupId, int actionType, int logType, int offset, int limit);
        Task<int> IGetCountByParameters(int userId, int groupId, int actionType, int logType);
    }
    public class SystemLogService : ISystemLogService
    {
        private readonly ISystemLogDao _systemLogDao;
        public SystemLogService(ISystemLogDao systemLogDao) => _systemLogDao = systemLogDao;

        public async Task<List<SystemLogModel?>?> IGetListByParameters(int userId, int groupId, int actionType, int logType, int offset, int limit)
            => (await _systemLogDao.ISelectListByParameters(userId, groupId, actionType, logType, offset, limit))?.Select(l => l?.ToSystemLogModel()).ToList();

        public async Task<int> IGetCountByParameters(int userId, int groupId, int actionType, int logType)
            => (await _systemLogDao.ISelectCountByParameters(userId, groupId, actionType, logType)).TotalCount;

        public async Task Set(SystemLogModel value)
            => await _systemLogDao.Insert(value?.ToSystemLogInsert());
    }
}
