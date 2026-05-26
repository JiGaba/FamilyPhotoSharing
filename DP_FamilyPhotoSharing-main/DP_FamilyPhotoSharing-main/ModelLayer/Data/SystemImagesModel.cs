using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class SystemImagesModel
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public string PhotoNameOriginal { get; set; }
        public string FSPhotoName { get; set; }
        public int CreateAuthorId { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
