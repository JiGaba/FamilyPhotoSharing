using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ModelLayer.PhotoEnums
{
    public static class TextEnumHelper
    {
        public static readonly string[] AllowedImageMimeTypes =
        {
            MineTypesTextEnum.JPG,
            MineTypesTextEnum.JPEG,
            MineTypesTextEnum.PNG,
            MineTypesTextEnum.WEBP
        };

        public static readonly string[] AllowedImageExtensions =
        {
            FileExtensionTextEnum.JPG,
            FileExtensionTextEnum.JPEG,
            FileExtensionTextEnum.PNG,
            FileExtensionTextEnum.WEBP
        };

        public const string AcceptImages =
            ".jpg,.jpeg,.png,.svg,.webp," +
            "image/jpeg,image/png,image/svg+xml,image/webp";

        public static bool AllowedMimeTypeAndExtension(string mimeType, string extension)
        {
            return (extension, mimeType) switch
            {
                (FileExtensionTextEnum.JPG or FileExtensionTextEnum.JPEG, MineTypesTextEnum.JPG) => true,
                (FileExtensionTextEnum.PNG, MineTypesTextEnum.PNG) => true,
                (FileExtensionTextEnum.WEBP, MineTypesTextEnum.WEBP) => true,
                _ => false
            };
        }

        public static string GetMimeTypeByExtension(string extension)
        {
            return (extension) switch
            {
                (FileExtensionTextEnum.JPG or FileExtensionTextEnum.JPEG) => MineTypesTextEnum.JPG,
                (FileExtensionTextEnum.PNG) => MineTypesTextEnum.PNG,
                (FileExtensionTextEnum.WEBP) => MineTypesTextEnum.WEBP,
                _ => throw new Exception($"Nepodporovaný typ souboru s koncovkou {extension}.")
            };
        }
    }
}
