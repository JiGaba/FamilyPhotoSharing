using DataAccessLayer.Dto.Photo;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.SystemImages
{
    public class SystemImagesInsert : SystemImagesBase { }

    public static partial class ProcedureMapperExtensions
    {
        public static SystemImagesInsert ToSystemImagesInsert(this SystemImagesModel systemImage) => new SystemImagesInsert
        {
            CreateAuthorId = systemImage.CreateAuthorId,
            PhotoNameOriginal = systemImage.PhotoNameOriginal,
            FSPhotoName = systemImage.FSPhotoName,
            Size = systemImage.Size
        };
    }
}
