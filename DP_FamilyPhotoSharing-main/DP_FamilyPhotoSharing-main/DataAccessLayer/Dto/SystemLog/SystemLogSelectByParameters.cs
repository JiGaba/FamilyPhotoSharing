using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SystemLog
{
    public class SystemLogSelectByParameters : SystemLogBase
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public SystemLogModel ToSystemLogModel() => new SystemLogModel
        {
            Id = Id,
            ActionType = EnumHelper.GetEnum<ActionTypeEnum>(ActionType),
            CreateAuthorId = CreateAuthorId,
            CreateDate = CreateDate,
            GroupsId = GroupsId,
            LogDescription = LogDescription,
            LogType = EnumHelper.GetEnum<LogTypeEnum>(LogType),
        };
    }
}
