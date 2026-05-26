using ModelLayer.PhotoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.BackgroundModels
{
    public abstract class BaseBGModel
    {
        public List<int> PhotoIds { get; set; }
        public int AlbumId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public ThumbnailData ThumbnailData { get; set; }
    }
}
