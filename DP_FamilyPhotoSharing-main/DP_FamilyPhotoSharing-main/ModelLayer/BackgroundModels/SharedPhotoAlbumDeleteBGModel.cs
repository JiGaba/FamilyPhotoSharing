using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.BackgroundModels
{
    public class SharedPhotoAlbumDeleteBGModel
    {
        public int AlbumId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
