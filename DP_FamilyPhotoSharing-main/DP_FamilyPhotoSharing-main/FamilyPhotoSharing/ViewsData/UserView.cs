using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Data;
using ModelLayer.System;

namespace FamilyPhotoSharing.ViewsData
{
    public class UserView : UserModel
    {
        public int RedactorRoleId { get; set; } = 4;
        public int RedactorId { get; set; } = 0;
        public List<SelectListItem> RoleOptionsNew
        { 
            get
            {
                switch (RedactorRoleId)
                {
                    case 1:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "2", Text = "Administrátor (rodiny)" },
                            new SelectListItem { Value = "3", Text = "Uživatel", Selected = true },
                            new SelectListItem { Value = "4", Text = "Host" },
                        };
                    case 2:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "2", Text = "Administrátor (rodiny)" },
                            new SelectListItem { Value = "3", Text = "Uživatel", Selected = true },
                            new SelectListItem { Value = "4", Text = "Host" },
                        };
                    case 3:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "3", Text = "Uživatel" },
                        };
                    case 4:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "4", Text = "Host" },
                        };
                    default:
                        return new List<SelectListItem>();
                }
            }
        }
        public List<SelectListItem> RoleOptionsEdit
        {
            get
            {
                switch (RoleId)
                {
                    case 1:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "1", Text = "Administrátor (systému)" }
                        };
                    case 2:
                        return new List<SelectListItem>
                        {
                            new SelectListItem { Value = "2", Text = "Administrátor (rodiny)", Selected = true },
                            new SelectListItem { Value = "3", Text = "Uživatel" }
                        };
                    case 3:
                        if (RedactorRoleId is 1 or 2) // Admin a Group admin
                            return new List<SelectListItem>
                            {
                                new SelectListItem { Value = "2", Text = "Administrátor (rodiny)" },
                                new SelectListItem { Value = "3", Text = "Uživatel", Selected = true }
                            };
                        else 
                            return new List<SelectListItem>
                            {
                                new SelectListItem { Value = "3", Text = "Uživatel" },
                            };
                    case 4:
                        if (RedactorRoleId is 1 or 2) // Admin a Group admin
                            return new List<SelectListItem>
                            {
                                new SelectListItem { Value = "2", Text = "Administrátor (rodiny)" },
                                new SelectListItem { Value = "3", Text = "Uživatel" },
                                new SelectListItem { Value = "4", Text = "Host", Selected = true },
                            };
                        else
                            return new List<SelectListItem>
                            {
                                new SelectListItem { Value = "4", Text = "Host" },
                            };
                    default:
                        return new List<SelectListItem>();
                }
            }
        }
        public List<UserGroupModel> UserGroup { set { _userGroup = value; } }
        private List<UserGroupModel> _userGroup = new List<UserGroupModel>();
        public List<SelectListItem> GroupsOptionsNew
        {
            get
            {
                List<SelectListItem> retVal = new List<SelectListItem>();

                foreach (var item in _userGroup)
                {
                    retVal.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.GroupName });
                }

                return retVal;
            }
        }
        public List<SelectListItem> GroupsOptionsEdit
        {
            get
            {
                List<SelectListItem> retVal = new List<SelectListItem>();

                if (_userGroup == null || !_userGroup.Any()) return retVal;

                foreach (var item in _userGroup)
                {
                    if(item.Id == GroupId)
                        retVal.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.GroupName });
                }

                return retVal;
            }
        }
        public string CreateAuthorName { get; set; }
        public string RoleName { get => UserRoles.GetRoleById(RoleId); }
        public string GroupName { get => GetGroupName(); }
        public UserPasswordModel PasswordModel { get { return new UserPasswordModel { Login = UserLogin, Id = Id }; } }
        private string GetGroupName() => _userGroup?.Where(g => g.Id == GroupId)?.FirstOrDefault()?.GroupName ?? "";
    }

    public static partial class ViewExtension
    {
        public static UserView ToUserView(this UserModel userModel, int redactorId, string authorName, List<UserGroupModel> userGroups) => new UserView
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
            CreateAuthorName = authorName,
            UserGroup = userGroups,
            GroupId = userModel.GroupId,
            RedactorId = redactorId,
            Activated = userModel.Activated,
            BackupKey = string.IsNullOrWhiteSpace(userModel.BackupKey) ? "" : "exists",
        };
    }
}
