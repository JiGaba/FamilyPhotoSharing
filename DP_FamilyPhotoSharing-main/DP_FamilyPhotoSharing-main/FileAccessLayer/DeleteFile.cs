using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessLayer
{
    public interface IDeleteFile
    {
        Task Delete(string fileName, string folder = "");
    }
    public class DeleteFile : FileBase, IDeleteFile
    {
        public async Task Delete(string fileName, string folder = "")
        {
            var filePath = GetFilePath(fileName, folder);

            FsDelete(filePath);
        }

        private void FsDelete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
