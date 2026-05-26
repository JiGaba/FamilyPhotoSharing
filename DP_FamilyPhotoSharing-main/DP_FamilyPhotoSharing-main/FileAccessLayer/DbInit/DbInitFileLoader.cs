using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessLayer.DbInit
{
    public interface IDbInitFileLoader
    {
        Task<List<string>> GetDbInitFileByFolder(FoldersEnum folder);
    }
    public class DbInitFileLoader : IDbInitFileLoader
    {
        private const string _BASE_FOLDER = "SQLInitialize";
        public async Task<List<string>> GetDbInitFileByFolder(FoldersEnum folder)
        {
            var assembly = Assembly.GetEntryAssembly();
            var folderName = folder.GetDescription(); 
            var retVal = new List<string>();

            var resourceNames = assembly
                .GetManifestResourceNames()
                .Where(r => r.Contains($"{_BASE_FOLDER}.{folderName}") && r.EndsWith(".sql"))
                .ToList();

            foreach (var resource in resourceNames.OrderBy(n => n).ToList())
            {
                using var stream = assembly.GetManifestResourceStream(resource);
                using var reader = new StreamReader(stream);

                var sql = await reader.ReadToEndAsync();
                retVal.Add(sql);
            }

            return retVal;
        }
    }
}
