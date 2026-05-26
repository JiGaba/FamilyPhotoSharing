using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.UserKeys
{
    public class UserKeysSelectByUserId : UserKeysBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public UserKeysModel ToUserKeysModel() => new UserKeysModel
        {
            Id = Id,
            CreateDateTime = CreateDateTime,
            UserId = UserId,
            RSAPublicKey = RSAPublicKey,
            RSAPrivateKey = RSAPrivateKey,
            PublicKeyTag = PublicKeyTag,
            PublicKeyNonce = PublicKeyNonce,
            PrivateKeyTag = PrivateKeyTag,
            CreateAuthorId = CreateAuthorId,
            PrivateKeyNonce = PrivateKeyNonce
        };
    }
}
