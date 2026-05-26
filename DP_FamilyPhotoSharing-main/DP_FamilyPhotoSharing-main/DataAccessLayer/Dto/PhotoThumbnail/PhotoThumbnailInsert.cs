using DataAccessLayer.Dto.PhotoAlbum;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoThumbnail
{
    public class PhotoThumbnailInsert : PhotoThumbnailBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoThumbnailInsert ToPhotoThumbnailInsert(this PhotoThumbnailModel album) => new PhotoThumbnailInsert
        {
            CreateAuthor = album.CreateAuthor,
            FileSize = album.FileSize,
            FSThumbnailName = album.FSThumbnailName,
            PhotoId = album.PhotoId
        };
    }
}
