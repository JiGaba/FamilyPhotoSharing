using DataAccessLayer.Dto.Photo;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Groups
{
    public class UserGroupsUpdate : GroupBase 
    { 
        public int GroupId { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserGroupsUpdate ToUserGroupsUpdate(this UserGroupModel group) => new UserGroupsUpdate
        {
            CreateAuthor = group.CreateAuthor,
            GroupName = group.GroupName,
            GroupDescription = group.GroupDescription,
            GroupId = group.Id
        };
    }
}
