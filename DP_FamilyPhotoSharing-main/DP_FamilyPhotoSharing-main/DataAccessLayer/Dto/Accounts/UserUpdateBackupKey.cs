using EncryptionLayer.Password;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public class UserUpdateBackupKey
    {
        public int Id { get; set; }
        public string BackupKey { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserUpdateBackupKey ToUserUpdateBackupKey(this UserBackupKeyModel user) => new UserUpdateBackupKey
        {
            BackupKey = string.IsNullOrEmpty(user.BackupKey) ? "" : PasswordEncryption.HashPassword(user.BackupKey),
            Id = user.Id,
        };
    }
}
