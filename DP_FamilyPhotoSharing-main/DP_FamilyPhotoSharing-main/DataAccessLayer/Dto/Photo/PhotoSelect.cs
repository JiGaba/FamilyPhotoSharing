using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Photo
{
    public class PhotoSelect : PhotoBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public PhotoModel ToPhotoModel() => new PhotoModel
        {
            PhotoName = PhotoName,
            PhotoDescription = PhotoDescription,
            Personal = Personal,
            OwnerId = OwnerId,
            GroupsId = GroupsId,
            FSFileName = FSFileName,
            CreateAuthor = CreateAuthor,
            CreateDateTime = CreateDateTime,
            FileSize = FileSize,
            Id = Id
        };
    }
}
