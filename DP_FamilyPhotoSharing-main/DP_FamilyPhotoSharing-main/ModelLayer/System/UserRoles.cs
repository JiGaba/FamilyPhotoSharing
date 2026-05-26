using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class UserRoles
    {
        public const string Admin = "Admin";
        public const string GroupAdmin = "GroupAdmin";
        public const string User = "User";
        public const string Host = "Host";

        public static string GetRoleById(int id)
        {
            switch (id)
            {
                case 1: return Admin;
                case 2: return GroupAdmin;
                case 3: return User;
                case 4: return Host;
                default: throw new Exception($"Uživatelská role s {id} není definována!");
            }
        }

        public static int GetRoleByName(string? name)
        {
            switch (name)
            {
                case Admin: return 1;
                case GroupAdmin: return 2;
                case User: return 3;
                case Host: return 4;
                default: return 0;
            }
        }

        public static bool CanCreateUser(int userRole)
        {
            var userRoleEnum = EnumHelper.GetEnum<UserRoleEnum>(userRole);

            return CanCreateUser(userRoleEnum);
        }

        public static bool CanCreateUser(UserRoleEnum userRole)
        {
            return userRole switch
            {
                UserRoleEnum.Admin or UserRoleEnum.GroupAdmin => true,
                _ => false 
            };
        }

        public static bool IsEditableByRoleSameGroup(int userRole, int redactorRole)
        {
            var userRoleEnum = EnumHelper.GetEnum<UserRoleEnum>(userRole);
            var redactorRoleEnum = EnumHelper.GetEnum<UserRoleEnum>(redactorRole);

            return IsEditableByRoleSameGroup(userRoleEnum, redactorRoleEnum);
        }

        public static bool IsEditableByRoleSameGroup(UserRoleEnum userRole, UserRoleEnum redactorRole)
        {
            return (redactorRole, userRole) switch
            {
                (UserRoleEnum.Admin, UserRoleEnum.GroupAdmin) => true,
                (UserRoleEnum.Admin, UserRoleEnum.User) => true,
                (UserRoleEnum.Admin, UserRoleEnum.Host) => true,
                (UserRoleEnum.GroupAdmin, UserRoleEnum.GroupAdmin) => true,
                (UserRoleEnum.GroupAdmin, UserRoleEnum.User) => true,
                (UserRoleEnum.GroupAdmin, UserRoleEnum.Host) => true,
                _ => false
            };
        }

        public static bool IsEditableByRoleOtherGroup(int userRole, int redactorRole)
        {
            var userRoleEnum = EnumHelper.GetEnum<UserRoleEnum>(userRole);
            var redactorRoleEnum = EnumHelper.GetEnum<UserRoleEnum>(redactorRole);

            return IsEditableByRoleOtherGroup(userRoleEnum, redactorRoleEnum);
        }

        public static bool IsEditableByRoleOtherGroup(UserRoleEnum userRole, UserRoleEnum redactorRole)
        {
            return (redactorRole, userRole) switch
            {
                (UserRoleEnum.Admin, UserRoleEnum.GroupAdmin) => true,
                (UserRoleEnum.Admin, UserRoleEnum.User) => true,
                (UserRoleEnum.Admin, UserRoleEnum.Host) => true,
                _ => false
            };
        }
    }
}
