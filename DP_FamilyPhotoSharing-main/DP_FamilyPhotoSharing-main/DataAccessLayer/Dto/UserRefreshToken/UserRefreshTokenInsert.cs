using DataAccessLayer.Dto.UserKeys;
using ModelLayer.Data;
using ModelLayer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.UserRefreshToken
{
    public class UserRefreshTokenInsert : UserRefreshTokenBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static UserRefreshTokenInsert ToUserRefreshTokenInsert(this RefreshTokenModel token) => new UserRefreshTokenInsert
        {
            CreateAuthorId = token.CreateAuthorId,
            UserId = token.UserId,
            Expires = token.Expires,
            IsRevoked = token.IsRevoked,
            Token = token.Token
        };
    }
}
