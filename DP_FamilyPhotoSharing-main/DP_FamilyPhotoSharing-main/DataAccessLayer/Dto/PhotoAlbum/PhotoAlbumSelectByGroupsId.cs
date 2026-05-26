using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoAlbum
{
    public class PhotoAlbumSelectByGroupsId : PhotoAlbumSelect 
    {
        public string CreateAuthorName { get; set; } = string.Empty;
        public int PhotoCount { get;set; }
        public PhotoAlbumsModel ToPhotoAlbumsModel() => new PhotoAlbumsModel
        {
            AlbumDescription = AlbumDescription,
            AlbumName = AlbumName,
            CreateAuthorName = CreateAuthorName,
            PhotoCount = PhotoCount,
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
