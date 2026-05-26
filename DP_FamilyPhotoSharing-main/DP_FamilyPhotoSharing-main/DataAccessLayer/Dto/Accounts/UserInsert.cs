using EncryptionLayer.Password;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public class UserInsert : UserBase 
    {
        public int GroupId { get; set; }
        public required string BackupKey { get; set; }
    }

    public static partial class ProcedureMapperExtensions
    {
        public static UserInsert ToUserInsert(this UserModel user) => new UserInsert
        {
            Active = user.Active,
            UserDescription = user.UserDescription,
            UserLogin = user.UserLogin,
            UserName = user.UserName,
            UserPassword = PasswordEncryption.HashPassword(user.UserPasswordPlain),
            UserSurname = user.UserSurname,
            CreateAuthor = user.CreateAuthor,
            RoleId = user.RoleId,
            SystemImagesId = user.SystemImagesId,
            GroupId = user.GroupId,
            BackupKey = "",
            Activated = user.Activated,
        };
    }
}
