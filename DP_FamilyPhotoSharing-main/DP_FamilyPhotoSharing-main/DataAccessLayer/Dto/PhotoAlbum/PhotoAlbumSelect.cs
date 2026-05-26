using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoAlbum
{
    public class PhotoAlbumSelect : PhotoAlbumBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public PhotoAlbumModel ToPhotoAlbumModel() => new PhotoAlbumModel
        {
            AlbumDescription = AlbumDescription,
            AlbumName = AlbumName,
            CreateAuthor = CreateAuthor,
            CreateDateTime = CreateDateTime,
            TitlePhotoId = TitlePhotoId,
            Id = Id,
            OwnerUserId = OwnerUserId,
            Personal = Personal,
            UserGroupsId = UserGroupsId
        };
    }
}
