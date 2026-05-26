using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Photo
{
    public class CryptoBase
    {
        protected string BytesToText(byte[] data) => Encoding.UTF8.GetString(data);
        protected byte[] TextToBytes(string data) => Encoding.UTF8.GetBytes(data);
        public static string BytesToBase64(byte[] data) => Convert.ToBase64String(data);
        protected byte[] Base64ToBytes(string data) => Convert.FromBase64String(data);
    }
}
