using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class RelationSharedAlbumPhotoModel
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int SharedAlbumId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int CreateAuthorId { get; set; }
    }
}
