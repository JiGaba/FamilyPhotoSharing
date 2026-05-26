using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public abstract class UserBase
    {
        public int RoleId { get; set; }
        public required string UserLogin { get; set; }
        public required string UserName { get; set; }
        public required string UserSurname { get; set; }
        public required string UserPassword { get; set; }
        public required string UserDescription { get; set; }
        public int SystemImagesId { get; set; }
        public int CreateAuthor {  get; set; }
        public bool Active { get; set; }
        public bool Activated { get; set; }
    }
}
