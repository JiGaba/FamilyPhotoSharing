using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class SystemUsersDataModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public int ProfileImageId {  get; set; } = 0;
        public UserRoleEnum RoleEnum {  get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
    }
}
