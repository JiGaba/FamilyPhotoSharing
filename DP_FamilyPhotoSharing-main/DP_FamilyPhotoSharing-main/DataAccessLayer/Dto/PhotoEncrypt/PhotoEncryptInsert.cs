using DataAccessLayer.Dto.PhotoAlbum;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoEncrypt
{
    public class PhotoEncryptInsert : PhotoEncryptBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoEncryptInsert ToPhotoEncryptInsert(this PhotoEncryptModel photo) => new PhotoEncryptInsert
        {
            UserId = photo.UserId,
            Tag = photo.Tag,
            Nonce = photo.Nonce,
            Aes = photo.Aes,
            CreateAuthorId = photo.CreateAuthorId,
            FileId = photo.FileId,
            FileType = (Int16)photo.FileType, 
        };
    }
}
