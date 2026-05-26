using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationUserSharedAlbum
{
    public class RelationUserSharedAlbumBase
    {
        public int UserId { get; set; }
        public int SharedAlbumId { get; set; }
    }
}
