using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SystemImages
{
    public class SystemImagesSelect : SystemImagesBase
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public SystemImagesModel ToSystemImagesModel() => new SystemImagesModel
        {
            Size = Size,
            FSPhotoName = FSPhotoName,
            CreateDateTime = CreateDateTime,
            PhotoNameOriginal = PhotoNameOriginal,
            CreateAuthorId = CreateAuthorId,
            Id = Id
        };
    }
}
