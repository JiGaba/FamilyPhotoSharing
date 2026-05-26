using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class RelationAlbumPhotoModel
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int AlbumId { get; set; }
        public int GroupId { get; set; }
        public int AuthorId { get; set; }
    }
}
