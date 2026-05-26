using DataAccessLayer.Dto.SystemLog;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.UserKeys
{
    public class UserKeysInsert : UserKeysBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static UserKeysInsert ToUserKeysInsert(this UserKeysModel userKeys) => new UserKeysInsert
        {
            CreateAuthorId = userKeys.CreateAuthorId,
            PrivateKeyNonce = userKeys.PrivateKeyNonce,
            PrivateKeyTag = userKeys.PrivateKeyTag,
            PublicKeyNonce = userKeys.PublicKeyNonce,
            PublicKeyTag = userKeys.PublicKeyTag,
            RSAPrivateKey = userKeys.RSAPrivateKey,
            RSAPublicKey = userKeys.RSAPublicKey,
            UserId = userKeys.UserId
        };
    }
}
