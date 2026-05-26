using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoEncrypt
{
    public abstract class PhotoEncryptBase
    {
        public int UserId { get; set; }
        public int FileId { get; set; }
        public Int16 FileType { get; set; }
        public byte[] Aes { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] Tag { get; set; }
        public int CreateAuthorId { get; set; }
    }
}
