using ModelLayer.Data;

namespace FamilyPhotoSharing.ViewsData
{
    public class UserGroupView : UserGroupModel
    {
        public int UsersActive { get; set; } = 0;
        public int UsersInactive { get; set; } = 0;
        public string IAutorName { get; set; } = string.Empty;
    }

    public static partial class ViewExtension
    {
        public static UserGroupView ToUserGroupView(this UserGroupModel userGroupModel, List<UserCountModel> userCountList, string iAutorName) => new UserGroupView
        {
            GroupDescription = userGroupModel.GroupDescription,
            GroupName = userGroupModel.GroupName,
            UsersActive = userCountList.Where(u => u.Active).Select(u => u.UserCount).First(),
            UsersInactive = userCountList.Where(u => !u.Active).Select(u => u.UserCount).First(),
            CreateAuthor = userGroupModel.CreateAuthor,
            CreateDateTime = userGroupModel.CreateDateTime,
            FolderName = userGroupModel.FolderName,
            Id = userGroupModel.Id,
            IAutorName = iAutorName
        };

        public static UserGroupView ToUserGroupView(this UserGroupModel userGroupModel, string iAutorName) => new UserGroupView
        {
            GroupDescription = userGroupModel.GroupDescription,
            GroupName = userGroupModel.GroupName,
            UsersActive = 0,
            UsersInactive = 0,
            CreateAuthor = userGroupModel.CreateAuthor,
            CreateDateTime = userGroupModel.CreateDateTime,
            FolderName = userGroupModel.FolderName,
            Id = userGroupModel.Id,
            IAutorName = iAutorName
        };
    }
}
