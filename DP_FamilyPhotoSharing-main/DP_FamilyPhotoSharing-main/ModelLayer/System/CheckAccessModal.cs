using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class CheckAccessModal
    {
        public bool CheckDbConn { get; set; }
        public bool CheckTableConn { get; set; }
        public bool CheckKeyConn { get; set; }
    }
}
