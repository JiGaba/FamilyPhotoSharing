using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Accounts
{
    public class UserSelectCountByGroupId
    {
        public bool Active { get; set; }
        public int UserCount { get; set; } = 0;
        public UserCountModel ToUserCountModel() => new UserCountModel
        {
            Active = Active,
            UserCount = UserCount
        };
    }
}
