using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SharedPhotoAlbum
{
    public class SharedPhotoAlbumSelect : SharedPhotoAlbumSelectByHostUserId 
    { 
        public string HostUserIds { get; set; }
        public override SharedPhotoAlbumsModel ToSharedPhotoAlbumsModel() => new SharedPhotoAlbumsModel
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
            HostUserCount = 0,
            HostUserIds = HostUserIds,
        };
    }
}
