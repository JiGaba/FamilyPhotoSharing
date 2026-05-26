using DataAccessLayer.Dto.Photo;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Groups
{
    public class UserGroupsInsert : GroupBase
    {
        public string FolderName { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserGroupsInsert ToUserGroupsInsert(this UserGroupModel group) => new UserGroupsInsert
        {
            CreateAuthor = group.CreateAuthor,
            FolderName = group.FolderName,
            GroupDescription = group.GroupDescription,
            GroupName = group.GroupName
        };
    }
}
