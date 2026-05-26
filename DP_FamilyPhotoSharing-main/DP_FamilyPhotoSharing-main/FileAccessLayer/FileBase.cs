using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessLayer
{
    public abstract class FileBase
    {
        protected static readonly string _dockerFolder = "/app/files";
        protected string GetFilePath(string fileName, string folder)
        {
            var fsFolder = string.IsNullOrEmpty(folder) ? _dockerFolder : Path.Combine(_dockerFolder, folder);
            Directory.CreateDirectory(fsFolder);

            return Path.Combine(fsFolder, fileName);
        }
    }
}
