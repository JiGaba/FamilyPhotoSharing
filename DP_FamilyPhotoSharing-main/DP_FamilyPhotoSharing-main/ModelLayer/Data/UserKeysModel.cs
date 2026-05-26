using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class UserKeysModel
    {
		public int Id { get; set; }
		public int UserId { get; set; }
		public byte[] RSAPublicKey { get; set; }
        public byte[] RSAPrivateKey {  get; set; }
        public byte[] PublicKeyNonce { get; set; }
        public byte[] PublicKeyTag {  get; set; }
        public byte[] PrivateKeyNonce { get; set; }
        public byte[] PrivateKeyTag { get; set; }
        public DateTime CreateDateTime { get; set; }
		public int CreateAuthorId {  get; set; } 
    }
}
