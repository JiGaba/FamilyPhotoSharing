using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class RefreshTokenModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public int CreateAuthorId { get; set; }
    }

}
