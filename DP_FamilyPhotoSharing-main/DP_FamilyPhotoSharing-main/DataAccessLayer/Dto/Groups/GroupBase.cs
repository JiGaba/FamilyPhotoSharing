using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Groups
{
    public class GroupBase
    {
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int CreateAuthor { get; set; }
    }
}
