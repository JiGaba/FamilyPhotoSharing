using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoThumbnail
{
    public class PhotoThumbnailSelectByPhotoId : PhotoThumbnailBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime {  get; set; }
        public PhotoThumbnailModel ToPhotoThumbnailModel() => new PhotoThumbnailModel
        {
            CreateAuthor = CreateAuthor,
            CreateDateTime = CreateDateTime,
            FileSize = FileSize,
            FSThumbnailName = FSThumbnailName,
            Id = Id,
            PhotoId = PhotoId
        };
    }
}
