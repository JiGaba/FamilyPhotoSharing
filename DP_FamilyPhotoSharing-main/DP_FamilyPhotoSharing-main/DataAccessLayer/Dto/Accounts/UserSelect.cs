using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    /// <summary>
    /// Input parameter int UserId
    /// </summary>
    public class UserSelect : UserBase
    {
        public DateTime CreateDateTime { get; set; }
        public int Id { get; set; }
        public int GroupId { get; set; }
        public required string BackupKey { get; set; }
        public UserModel ToUserModel() => new UserModel
        {
            UserDescription = UserDescription,
            UserLogin = UserLogin,
            UserName = UserName,
            UserPasswordHash = UserPassword,
            UserSurname = UserSurname,
            Active = Active,
            CreateAuthor = CreateAuthor,
            CreateDateTime = CreateDateTime,
            Id = Id,
            RoleId = RoleId,
            SystemImagesId = SystemImagesId,
            UserPasswordPlain = "",
            GroupId = GroupId,
            Activated = Activated,
            BackupKey = BackupKey,
        };
    }
}
