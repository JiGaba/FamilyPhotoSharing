using ModelLayer.Data;
using ModelLayer.Enums;

namespace FamilyPhotoSharing.ViewsData
{
    public class SystemLogView : SystemLogModel
    {
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public string ActionTypeName { get; set; }
        public string LogTypeName { get; set; }
        public string CreateDateWithFormat { get; set; }
    }

    public static partial class ViewExtension
    {
        public static SystemLogView ToSystemLogView(this SystemLogModel log, List<UserGroupModel>? groupList, 
            List<UserModel>? userList) => new SystemLogView
        {
             ActionType = log.ActionType,
             ActionTypeName = EnumHelper.GetDescription(log.ActionType),
             CreateAuthorId = log.CreateAuthorId,
             CreateDate = log.CreateDate,
             GroupsId = log.GroupsId,
             GroupName = groupList.Where(g => g.Id == log.GroupsId).FirstOrDefault()?.GroupName ?? "",
             Id = log.Id,
             LogDescription = log.LogDescription,
             LogType = log.LogType,
             LogTypeName = EnumHelper.GetDescription(log.LogType),
             UserName = userList.Where(u => u.Id == log.CreateAuthorId).FirstOrDefault()?.GetName() ?? "",
             CreateDateWithFormat = log.CreateDate.ToString("dd.MM.yyyy HH:mm")
        };
    }
}
