using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using FileAccessLayer;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.PhotoThumbnail;
using ServiceLayer.Services.SystemImages;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Files
{
    public interface IDownloadFileService
    {
        Task<(byte[] data, string mimeType)> GetPhoto(int id, int userId, string folder = "");
        Task<byte[]> GetThumbnail(int id, int userId, string folder = "");
        Task<byte[]> GetProfileImage(int systemImageId, string folder);
    }
    public class DownloadFileService : IDownloadFileService
    {
        private readonly IUserKeysService _userKeysService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoEncryptService _photoEncryptService;
        private readonly IDownloadFile _downloadFile;
        private readonly ICryptoService _cryptoService;
        private readonly IPhotoService _photoService;
        private readonly ISystemImagesService _systemImagesService;

        public DownloadFileService(IUserKeysService userKeysService, IPhotoThumbnailService photoThumbnailService,
            IPhotoEncryptService photoEncryptService, IDownloadFile downloadFile, ICryptoService cryptoService, 
            IPhotoService photoService, ISystemImagesService systemImagesService)
        {
            _userKeysService = userKeysService;
            _photoThumbnailService = photoThumbnailService;
            _photoEncryptService = photoEncryptService;
            _downloadFile = downloadFile;
            _cryptoService = cryptoService;
            _photoService = photoService;
            _systemImagesService = systemImagesService;
        }

        public async Task<(byte[] data, string mimeType)> GetPhoto(int id, int userId, string folder = "")
        {
            try
            {
                var userKey = await _userKeysService.SelectByUserId(userId);
                var photo = await _photoService.Get(id);
                var photoEncrypt = await _photoEncryptService.GetByUserIdPhotoId(userId, photo.Id, FileTypeEnum.Photo);
                var fileData = await _downloadFile.Download(photo.FSFileName, folder);
                var mimeType = TextEnumHelper.GetMimeTypeByExtension(Path.GetExtension(photo.PhotoName));

                var encryptedDataModel = EncryptedDataModel.GetEncryptedDataFromPhotoEncryptModel(photoEncrypt);

                UserKeyModel userKeyModel = new UserKeyModel
                {
                    KeyPem = userKey.RSAPrivateKey,
                    Nonce = userKey.PrivateKeyNonce,
                    Tag = userKey.PrivateKeyTag,
                    UserId = userId,
                };

                var data = _cryptoService.DecryptData(fileData, encryptedDataModel, userKeyModel);

                return (data, mimeType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<byte[]> GetProfileImage(int systemImageId, string folder)
        {
            try
            {
                var image = await _systemImagesService.Get(systemImageId);
                var fileData = await _downloadFile.Download(image.FSPhotoName, folder);

                return fileData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<byte[]> GetThumbnail(int id, int userId, string folder = "")
        {
            try
            {
                var userKey = await _userKeysService.SelectByUserId(userId);
                var photoThumbnail = await _photoThumbnailService.Get(id);
                var photoEncrypt = await _photoEncryptService.GetByUserIdPhotoId(userId, photoThumbnail.Id, FileTypeEnum.Thumbnail);
                var fileData = await _downloadFile.Download(photoThumbnail.FSThumbnailName, folder);

                var encryptedDataModel = EncryptedDataModel.GetEncryptedDataFromPhotoEncryptModel(photoEncrypt);

                UserKeyModel userKeyModel = new UserKeyModel
                {
                    KeyPem = userKey.RSAPrivateKey,
                    Nonce = userKey.PrivateKeyNonce,
                    Tag = userKey.PrivateKeyTag,
                    UserId = userId,
                };

                return _cryptoService.DecryptData(fileData, encryptedDataModel, userKeyModel);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
