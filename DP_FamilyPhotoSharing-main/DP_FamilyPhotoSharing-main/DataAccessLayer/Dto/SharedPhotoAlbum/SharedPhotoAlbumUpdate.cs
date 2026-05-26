using DataAccessLayer.Dto.PhotoAlbum;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SharedPhotoAlbum
{
    public class SharedPhotoAlbumUpdate : SharedPhotoAlbumBase
    {
        public int Id { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static SharedPhotoAlbumUpdate ToSharedPhotoAlbumUpdate(this SharedPhotoAlbumModel album) => new SharedPhotoAlbumUpdate
        {
            CreateAuthor = album.CreateAuthor,
            UserGroupsId = album.UserGroupsId,
            OwnerUserId = album.OwnerUserId,
            AlbumDescription = album.AlbumDescription,
            AlbumName = album.AlbumName,
            TitlePhotoId = album.TitlePhotoId,
            Id = album.Id,
        };
    }
}
