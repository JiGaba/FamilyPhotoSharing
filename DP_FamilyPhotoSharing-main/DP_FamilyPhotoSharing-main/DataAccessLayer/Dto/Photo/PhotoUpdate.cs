using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.Photo
{
    public class PhotoUpdate : PhotoBase
    {
        public int Id { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoUpdate ToPhotoUpdate(this PhotoModel photo) => new PhotoUpdate
        {
            CreateAuthor = photo.CreateAuthor,
            FileSize = photo.FileSize,
            FSFileName = photo.FSFileName,
            GroupsId = photo.GroupsId,
            OwnerId = photo.OwnerId,
            Personal = photo.Personal,
            PhotoDescription = photo.PhotoDescription,
            PhotoName = photo.PhotoName,
            Id = photo.Id,
        };
    }
}
