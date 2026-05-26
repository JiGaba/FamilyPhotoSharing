using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationAlbumPhoto
{
    public abstract class RalationAlbumPhotoBase
    {
        public int AlbumId { get; set; }
        public int GroupId { get; set; }
        public int PhotoId { get; set; }
    }
}
