using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoAlbum
{
    public class PhotoAlbumUpdate : PhotoAlbumBase
    {
        public int Id { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoAlbumUpdate ToPhotoAlbumUpdate(this PhotoAlbumModel album) => new PhotoAlbumUpdate
        {
            CreateAuthor = album.CreateAuthor,
            UserGroupsId = album.UserGroupsId,
            Personal = album.Personal,
            OwnerUserId = album.OwnerUserId,
            AlbumDescription = album.AlbumDescription,
            AlbumName = album.AlbumName,
            TitlePhotoId = album.TitlePhotoId,
            Id = album.Id,
        };
    }
}
