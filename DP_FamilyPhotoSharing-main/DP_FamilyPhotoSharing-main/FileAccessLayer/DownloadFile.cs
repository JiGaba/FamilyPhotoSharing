using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessLayer
{
    public interface IDownloadFile
    {
        Task<byte[]> Download(string fileName, string folder = "");
    }
    public class DownloadFile : FileBase, IDownloadFile
    {
        public async Task<byte[]> Download(string fileName, string folder = "")
        {
            var filePath = GetFilePath(fileName, folder);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Soubor nebyl nalezen: {filePath}");

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
