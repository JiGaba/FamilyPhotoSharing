using EncryptionLayer.Password;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public class UserUpdate : UserBase 
    {
        public int Id { get; set; }
        public bool PasswordChange { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserUpdate ToUserUpdate(this UserModel user) => new UserUpdate
        {
            Active = user.Active,
            UserDescription = user.UserDescription,
            UserLogin = user.UserLogin,
            UserName = user.UserName,
            UserPassword = string.IsNullOrEmpty(user.UserPasswordPlain) ? "" : PasswordEncryption.HashPassword(user.UserPasswordPlain),
            UserSurname = user.UserSurname,
            CreateAuthor = user.CreateAuthor,
            Id = user.Id,
            RoleId = user.RoleId,
            SystemImagesId = user.SystemImagesId,
            PasswordChange = !string.IsNullOrEmpty(user.UserPasswordPlain),
            Activated = user.Activated,
        };
    }
}
