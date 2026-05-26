using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.PhotoThumbnail
{
    public abstract class PhotoThumbnailBase
    {
        public int PhotoId { get; set; }
        public int FileSize { get; set; }
        public string FSThumbnailName { get; set; } = string.Empty;
        public int CreateAuthor { get; set; }
    }
}
