using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoAlbum
{
    public abstract class PhotoAlbumBase
    {
        public string AlbumName { get; set; } = string.Empty;
        public string AlbumDescription { get; set; } = string.Empty;
        public bool Personal { get; set; }
        public int TitlePhotoId { get; set; }
        public int CreateAuthor { get; set; }
        public int OwnerUserId { get; set; }
        public int UserGroupsId { get; set; }
    }
}
