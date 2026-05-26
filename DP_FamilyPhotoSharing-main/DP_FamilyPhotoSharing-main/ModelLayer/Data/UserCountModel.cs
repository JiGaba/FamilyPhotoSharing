using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserCountModel
    {
        public bool Active { get; set; }
        public int UserCount { get; set; } = 0;
    }
}
