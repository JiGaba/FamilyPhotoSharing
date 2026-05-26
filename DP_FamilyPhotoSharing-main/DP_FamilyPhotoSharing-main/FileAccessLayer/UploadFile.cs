
namespace FileAccessLayer
{
    public interface IUploadFile
    {
        Task Upload(Stream fileStream, string fileName, string folder = "");
        Task Upload(byte[] fileData, string fileName, string folder = "");
    }
    public class UploadFile : FileBase, IUploadFile
    {
        public async Task Upload(Stream fileStream, string fileName, string folder = "")
        {
            var filePath = GetFilePath(fileName, folder);

            await SaveStreamToFileAsync(fileStream, filePath);
        }

        public async Task Upload(byte[] fileData, string fileName, string folder = "")
        {
            var filePath = GetFilePath(fileName, folder);

            using (var ms = new MemoryStream(fileData))
            {
                await SaveStreamToFileAsync(ms, filePath);
            }
        }

        private async Task SaveStreamToFileAsync(Stream inputStream, string filePath)
        {
            try
            {
                using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await inputStream.CopyToAsync(outputStream);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
