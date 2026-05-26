using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Model
{
    public class UserKeyModel
    {
        /// <summary>
        /// Id uživatele, ke kterému jsou data vztažena
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// PEM klíč šifrovaný AES Master key a převedený do Base64
        /// </summary>
        public byte[] KeyPem { get; set; }
        /// <summary>
        /// Nonce daného souboru
        /// </summary>
        public byte[] Nonce { get; set; }
        /// <summary>
        /// Tag daného souboru
        /// </summary>
        public byte[] Tag { get; set; }
    }
}
