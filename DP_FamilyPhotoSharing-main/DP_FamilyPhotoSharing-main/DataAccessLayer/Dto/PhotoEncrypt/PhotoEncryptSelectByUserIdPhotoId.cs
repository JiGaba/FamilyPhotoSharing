using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoEncrypt
{
    public class PhotoEncryptSelectByUserIdPhotoId : PhotoEncryptBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public PhotoEncryptModel ToPhotoEncryptModel() => new PhotoEncryptModel
        {
            CreateAuthorId = CreateAuthorId,
            Aes = Aes,
            CreateDateTime = CreateDateTime,
            Id = Id,
            Nonce = Nonce,
            Tag = Tag,
            UserId = UserId,
            FileId = FileId,
            FileType = EnumHelper.GetEnum<FileTypeEnum>(FileType)
        };
    }
}
