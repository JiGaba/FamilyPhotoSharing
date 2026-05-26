using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationSharedAlbumPhoto
{
    public abstract class RelationSharedAlbumPhotoBase
    {
        public int SharedAlbumId { get; set; }
        public int PhotoId { get; set; }
    }
}
