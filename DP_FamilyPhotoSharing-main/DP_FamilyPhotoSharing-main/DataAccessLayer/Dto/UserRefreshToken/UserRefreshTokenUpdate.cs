using ModelLayer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.UserRefreshToken
{
    public class UserRefreshTokenUpdate
    {
        public int UserId { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static UserRefreshTokenUpdate ToUserRefreshTokenUpdate(this RefreshTokenModel token) => new UserRefreshTokenUpdate
        {
            UserId = token.UserId
        };
    }
}
