using DataAccessLayer.Dto.PhotoAlbum;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SharedPhotoAlbum
{
    public class SharedPhotoAlbumInsert : SharedPhotoAlbumBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static SharedPhotoAlbumInsert ToSharedPhotoAlbumInsert(this SharedPhotoAlbumModel album) => new SharedPhotoAlbumInsert
        {
            CreateAuthor = album.CreateAuthor,
            UserGroupsId = album.UserGroupsId,
            OwnerUserId = album.OwnerUserId,
            AlbumDescription = album.AlbumDescription,
            AlbumName = album.AlbumName,
            TitlePhotoId = album.TitlePhotoId,
        };
    }
}
