using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class RelationUserSharedAlbumModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SharedAlbumId { get; set; }
        public int CreateAuthorId { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
