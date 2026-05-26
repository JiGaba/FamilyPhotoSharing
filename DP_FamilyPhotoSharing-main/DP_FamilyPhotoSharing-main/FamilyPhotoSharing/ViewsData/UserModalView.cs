using ModelLayer.Data;
using ModelLayer.System;

namespace FamilyPhotoSharing.ViewsData
{
    public class UserModalView : UserModel
    {
        public bool HasAccess { get; set; } = false;
        public bool Owner { get; set; } = false;
        public string UserRoleString 
        { 
            get { return UserRoles.GetRoleById(RoleId); } 
        }
    }

    public static partial class ViewExtension
    {
        public static UserModalView ToUserModalView(this UserModel userModel, List<int> accessUserIds, int ownerId) => new UserModalView
        {
            Active = userModel.Active,
            UserDescription = userModel.UserDescription,
            UserLogin = userModel.UserLogin,
            UserName = userModel.UserName,
            UserPasswordPlain = "",
            UserSurname = userModel.UserSurname,
            CreateAuthor = userModel.CreateAuthor,
            CreateDateTime = userModel.CreateDateTime,
            Id = userModel.Id,
            RoleId = userModel.RoleId,
            SystemImagesId = userModel.SystemImagesId,
            UserPasswordHash = userModel.UserPasswordHash,
            GroupId = userModel.GroupId,
            HasAccess = accessUserIds?.Contains(userModel.Id) ?? false,
            Owner = ownerId == userModel.Id
        };
    }
}
