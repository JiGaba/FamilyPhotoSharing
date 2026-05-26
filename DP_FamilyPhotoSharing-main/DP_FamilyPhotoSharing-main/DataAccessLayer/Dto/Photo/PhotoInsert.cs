using DataAccessLayer.Dto.Accounts;
using EncryptionLayer.Password;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Photo
{
    public class PhotoInsert : PhotoBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoInsert ToPhotoInsert(this PhotoModel photo) => new PhotoInsert
        {
            CreateAuthor = photo.CreateAuthor,
            FileSize = photo.FileSize,
            FSFileName = photo.FSFileName,
            GroupsId = photo.GroupsId,
            OwnerId = photo.OwnerId,
            Personal = photo.Personal,
            PhotoDescription = photo.PhotoDescription,
            PhotoName = photo.PhotoName
        };
    }
}
