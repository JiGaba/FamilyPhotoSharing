using ModelLayer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.UserRefreshToken
{
    public class UserRefreshTokenSelectByToken : UserRefreshTokenBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public RefreshTokenModel ToRefreshTokenModel() => new RefreshTokenModel
        {
            CreateAuthorId = CreateAuthorId,
            Token = Token,
            IsRevoked = IsRevoked,
            Expires = Expires,
            Id = Id,
            UserId = UserId
        };
    }
}
