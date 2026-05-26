using DataAccessLayer.Dto.Photo;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoAlbum
{
    public class PhotoAlbumInsert : PhotoAlbumBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static PhotoAlbumInsert ToPhotoAlbumInsert(this PhotoAlbumModel album) => new PhotoAlbumInsert
        {
            CreateAuthor = album.CreateAuthor,
            UserGroupsId = album.UserGroupsId,
            Personal = album.Personal,
            OwnerUserId = album.OwnerUserId,
            AlbumDescription = album.AlbumDescription,
            AlbumName = album.AlbumName,
            TitlePhotoId = album.TitlePhotoId,
        };
    }
}
