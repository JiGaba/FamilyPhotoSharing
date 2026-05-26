using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SharedPhotoAlbum
{
    public class SharedPhotoAlbumSelectByOwnerUserId : SharedPhotoAlbumBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateAuthorName { get; set; } = string.Empty;
        public int HostUserId { get; set; }
        public int HostUserCount { get; set; }
        public int PhotoCount { get; set; }
        public SharedPhotoAlbumsModel ToSharedPhotoAlbumsModel() => new SharedPhotoAlbumsModel
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
            UserGroupsId = UserGroupsId,
            HostUserId = HostUserId,
            HostUserCount = HostUserCount,
        };
    }
}
