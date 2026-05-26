using EncryptionLayer.Password;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public class UserUpdatePassword
    {
        public int Id { get; set; }
        public string UserPassword { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserUpdatePassword ToUserUpdatePassword(this UserPasswordModel user) => new UserUpdatePassword
        {
            UserPassword = string.IsNullOrEmpty(user.NewPassword) ? "" : PasswordEncryption.HashPassword(user.NewPassword),
            Id = user.Id,
        };
    }
}
