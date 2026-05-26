using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SystemLog
{
    public abstract class SystemLogBase
    {
        public int LogType { get; set; }
        public int ActionType { get; set; }
        public string LogDescription { get; set; } = string.Empty;
        public int CreateAuthorId { get; set; }
        public int GroupsId { get; set; }
    }
}
