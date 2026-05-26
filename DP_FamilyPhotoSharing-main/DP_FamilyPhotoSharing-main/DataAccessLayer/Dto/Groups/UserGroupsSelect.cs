using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Groups
{
    public class UserGroupsSelect : GroupBase
    {
        public int Id { get; set; }
        public string FolderName { get; set; }
        public DateTime CreateDateTime { get; set; }
        public UserGroupModel ToUserGroupModel() => new UserGroupModel
        {
            CreateAuthor = CreateAuthor,
            GroupDescription = GroupDescription,
            GroupName = GroupName,
            FolderName = FolderName,
            CreateDateTime = CreateDateTime,
            Id = Id
        };
    }
}
