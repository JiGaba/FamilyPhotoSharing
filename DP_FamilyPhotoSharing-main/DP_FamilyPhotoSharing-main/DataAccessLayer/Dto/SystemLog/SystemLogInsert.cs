using DataAccessLayer.Dto.Photo;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SystemLog
{
    public class SystemLogInsert : SystemLogBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static SystemLogInsert ToSystemLogInsert(this SystemLogModel log) => new SystemLogInsert
        {
            ActionType = (int)log.ActionType,
            LogType = (int)log.LogType,
            LogDescription = log.LogDescription,
            GroupsId = log.GroupsId,
            CreateAuthorId = log.CreateAuthorId
        };
    }
}
