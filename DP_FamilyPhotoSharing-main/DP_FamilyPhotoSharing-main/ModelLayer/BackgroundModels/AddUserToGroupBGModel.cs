using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.BackgroundModels
{
    public class AddUserToGroupBGModel
    {
        public int UserId { get; set; }
        public int AuthorId { get; set; }
        public int GroupId { get; set; }
        public UserRoleEnum RoleEnum { get; set; }
    }
}
