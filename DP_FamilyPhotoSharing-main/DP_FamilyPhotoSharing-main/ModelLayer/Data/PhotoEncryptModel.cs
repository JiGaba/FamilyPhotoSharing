using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class PhotoEncryptModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FileId { get; set; }
        public FileTypeEnum FileType { get; set; }
        public byte[] Aes { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] Tag { get; set; }
        public DateTime CreateDateTime { get; set; }
		public int CreateAuthorId { get; set; }
    }
}
