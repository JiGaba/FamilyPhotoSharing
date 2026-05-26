using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class PhotoAlbumsModel : PhotoAlbumModel
    {
        public string CreateAuthorName { get; set; }
        public int PhotoCount { get; set; }
    }
}
