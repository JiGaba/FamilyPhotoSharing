using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class SystemLogModel
    {
        public int Id {  get; set; }
        public LogTypeEnum LogType { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public string LogDescription { get; set; } = string.Empty;
        public int CreateAuthorId { get; set; }
        public int GroupsId { get; set; }
        public DateTime CreateDate { get; set; }
        public SystemLogModel() { }
        public SystemLogModel(LogTypeEnum logType, ActionTypeEnum actionType, string logDescription, int createAuthorId, int groupsId)
        {
            LogType = logType;
            ActionType = actionType;
            LogDescription = logDescription;
            CreateAuthorId = createAuthorId;
            GroupsId = groupsId;
            CreateDate = DateTime.Now;
        }
        public static SystemLogModel GetLog(LogTypeEnum logType, ActionTypeEnum actionType, string description, int groupId, int roleId, int userId)
        => new SystemLogModel
        {
        ActionType = actionType,
        LogType = logType,
        LogDescription = $"{description} [role id: {roleId}]",
        CreateAuthorId = userId,
        GroupsId = groupId,
        };
    }
}
