using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Transactions;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using FileAccessLayer;
using Microsoft.VisualBasic.FileIO;
using ModelLayer.Data;
using ModelLayer.Enums;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.SystemImages;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceLayer.Services.Files
{
    public interface IUploadFileService
    {
        Task CreateTestFile();
        Task UploadFileAsync(PhotoModel photoModel, Stream data, string folder = "");
        Task UploadFileForFamilyAsync(PhotoModel photoModel, Stream data, string folder = "");
        Task<int> UploadProfileImageAsync(SystemImagesModel imagesModel, byte[] data, int userId, string folder);
    }
    public class UploadFileService : IUploadFileService
    {
        private const string _THUMBNAIL = "thumbnail";
        private readonly IUploadFile _uploadFile;
        private readonly IDeleteFile _deleteFile;
        private readonly ICryptoService _cryptoService;
        private readonly IUserKeysService _userKeysService;
        private readonly IUserService _userService;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly ISystemImagesService _systemImagesService;
        private readonly IUserTransaction _userTransaction;
        public UploadFileService(IUploadFile uploadFile, IDeleteFile deleteFile, 
            ICryptoService cryptoService, IUserKeysService userKeysService,
            IUserService userService, IPhotoTransaction photoTransaction, ISystemImagesService systemImagesService,
            IUserTransaction userTransaction) 
        { 
            _uploadFile = uploadFile;
            _deleteFile = deleteFile;
            _cryptoService = cryptoService;
            _userKeysService = userKeysService;
            _userService = userService;
            _photoTransaction = photoTransaction;
            _systemImagesService = systemImagesService;
            _userTransaction = userTransaction;
        }

        public async Task CreateTestFile()
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes("Hello file!");

                // uloži do FS
                await _uploadFile.Upload(data, "HelloFile.txt");
            }
            catch (Exception e)
            {
                throw new Exception($"Nastala chyba při ukládání souboru.");
            }
        }

        // Použití v prípadě, kdy si uživatel ukládá fotku sám pro sebe tedy photoModel.Personal == true
        public async Task UploadFileAsync(PhotoModel photoModel, Stream data, string folder = "")
        {
            string fSThumbnailName = string.Empty;

            try
            {
                byte[] originalData = await GetByteFromStream(data);

                // Příprava náhledu
                var thumbnailData = FileHelper.CreateThumbnail150x150(originalData);
                var photoThumbnail = GetPhotoThumbnail(ref fSThumbnailName, thumbnailData.Length, photoModel);

                // příprava fotografie
                photoModel.FSFileName = FileHelper.GetFileName(photoModel.PhotoName);

                var photoDataEncrypted = Array.Empty<byte>();
                var thumbnailDataEncrypted = Array.Empty<byte>();
                var userKeyList = new List<UserKeyModel>();
                userKeyList.Add(await GetUserKey(photoModel.OwnerId));

                // šifrování souborů
                var encryptedDataPhotoList = _cryptoService.EncryptData(originalData, ref photoDataEncrypted, userKeyList);
                var encryptedDataThumbnailList = _cryptoService.EncryptData(thumbnailData, ref thumbnailDataEncrypted, userKeyList);

                var photoEncrypt = GetFileEncrypt(encryptedDataPhotoList.ElementAt(0), photoModel.OwnerId, FileTypeEnum.Photo);
                var thumbnailEncrypt = GetFileEncrypt(encryptedDataThumbnailList.ElementAt(0), photoModel.OwnerId, FileTypeEnum.Thumbnail);

                // uloži do FS
                await _uploadFile.Upload(thumbnailDataEncrypted, fSThumbnailName, folder);
                await _uploadFile.Upload(photoDataEncrypted, photoModel.FSFileName, folder);

                // uložit do DB
                await _photoTransaction.SetPhoto(photoModel, photoThumbnail, photoEncrypt, thumbnailEncrypt);
            }
            catch (Exception e)
            {
                // Pokud se již vytvořily soubory fotografií => smazat
                if (!string.IsNullOrEmpty(photoModel.FSFileName)) 
                    await _deleteFile.Delete(photoModel.FSFileName, folder);

                if (!string.IsNullOrEmpty(fSThumbnailName))
                    await _deleteFile.Delete(fSThumbnailName, folder);

                throw new Exception($"Nastala chyba při ukládání souboru.");
            }
        }

        // Použití v případě, kdy uživatel ukládá fotku do adíleného rodinného alba photoModel.Perosnal == false 
        public async Task UploadFileForFamilyAsync(PhotoModel photoModel, Stream data, string folder = "")
        {
            string fSThumbnailName = string.Empty;

            try
            {
                byte[] originalData = await GetByteFromStream(data);

                // Načtení šifrovacích klíčů celé skupiny uživatelů
                var userKeyList = await GetUserKeyList(photoModel.GroupsId);

                // příprava náhledu
                var thumbnailData = FileHelper.CreateThumbnail150x150(originalData);
                var photoThumbnail = GetPhotoThumbnail(ref fSThumbnailName, thumbnailData.Length, photoModel);

                // Příparava fotografie
                photoModel.FSFileName = FileHelper.GetFileName(photoModel.PhotoName);

                // šifrování fotografie a náhledu
                var photoDataEncrypted = Array.Empty<byte>();
                var thumbnailDataEncrypted = Array.Empty<byte>();
                
                var encryptedDataPhotoList = _cryptoService.EncryptData(originalData, ref photoDataEncrypted, userKeyList);
                var encryptedDataThumbnailList = _cryptoService.EncryptData(thumbnailData, ref thumbnailDataEncrypted, userKeyList);

                // příparav Encrypt modelu pro fotografii a náhled
                var photoEncryptList = GetFileEncryptList(encryptedDataPhotoList, photoModel.OwnerId, FileTypeEnum.Photo);
                var thumbnailEncryptList = GetFileEncryptList(encryptedDataThumbnailList, photoModel.OwnerId, FileTypeEnum.Thumbnail);

                // ulož do FS
                await _uploadFile.Upload(thumbnailDataEncrypted, fSThumbnailName, folder);
                await _uploadFile.Upload(photoDataEncrypted, photoModel.FSFileName, folder);
                
                // ulož do DB
                await _photoTransaction.SetPhotoMultipleUser(photoModel, photoThumbnail, photoEncryptList, thumbnailEncryptList);
            }
            catch (Exception)
            {
                // Pokud se již vytvořily soubory fotografií => smazat
                if (!string.IsNullOrEmpty(photoModel.FSFileName))
                    await _deleteFile.Delete(photoModel.FSFileName, folder);

                if (!string.IsNullOrEmpty(fSThumbnailName))
                    await _deleteFile.Delete(fSThumbnailName, folder);

                throw new Exception($"Nastala chyba při ukládání souboru.");
            }
        }

        public async Task<int> UploadProfileImageAsync(SystemImagesModel imagesModel, byte[] data, int userId, string folder)
        {
            try
            {
                // Příprava náhledu
                int oldImageId = 0;
                string oldImageName = string.Empty;
                var ext = Path.GetExtension(imagesModel.PhotoNameOriginal);
                var extWithoutDot = ext.TrimStart('.');
                var thumbnailData = FileHelper.CreateThumbnail150x150(data);
                imagesModel.FSPhotoName = FileHelper.GetFileName(imagesModel.PhotoNameOriginal, "", extWithoutDot);

                var user = await _userService.Get(userId);

                if (user != null && user.SystemImagesId != 0) {
                    var oldImage = await _systemImagesService.Get(user.SystemImagesId);
                    oldImageId = oldImage.Id;
                    oldImageName = oldImage.FSPhotoName;
                }

                // uložit do FS
                await _uploadFile.Upload(data, imagesModel.FSPhotoName, folder);

                // uložit do DB
                var imageId = await _userTransaction.SetUser(user, imagesModel);

                if (oldImageName != string.Empty)
                    await _deleteFile.Delete(oldImageName, folder);

                return imageId;
            }
            catch (Exception e)
            {
                // Pokud se již vytvořily soubory fotografií => smazat
                if (!string.IsNullOrEmpty(imagesModel.FSPhotoName))
                    await _deleteFile.Delete(imagesModel.FSPhotoName, folder);

                throw new Exception($"Nastala chyba při ukládání souboru.");
            }
        }

        private List<PhotoEncryptModel> GetFileEncryptList(List<EncryptedDataModel> encryptedDataList, int ownerId, FileTypeEnum fileType)
        {
            var fileEncryptList = new List<PhotoEncryptModel>();

            foreach (var encryptedDataEncrypted in encryptedDataList)
                fileEncryptList.Add(new PhotoEncryptModel
                {
                    Tag = encryptedDataEncrypted.Tag,
                    Aes = encryptedDataEncrypted.AesKeyEncrypted,
                    CreateAuthorId = ownerId,
                    CreateDateTime = DateTime.Now,
                    FileType = fileType,
                    Nonce = encryptedDataEncrypted.Nonce,
                    UserId = encryptedDataEncrypted.UserId,
                });

            return fileEncryptList;
        }

        private PhotoEncryptModel GetFileEncrypt(EncryptedDataModel encryptedData, int ownerId, FileTypeEnum fileType) => new PhotoEncryptModel
        {
            Tag = encryptedData.Tag,
            Aes = encryptedData.AesKeyEncrypted,
            CreateAuthorId = ownerId,
            CreateDateTime = DateTime.Now,
            FileType = fileType,
            Nonce = encryptedData.Nonce,
            UserId = encryptedData.UserId,
        };

        private async Task<List<UserKeyModel>> GetUserKeyList(int groupId)
        {
            var userKeyList = new List<UserKeyModel>();
            // Načtení uživatelů sdílejícíh knihovnu
            // role Host je pouze pro zobrazení určitých fotografií nikoli člen rodiny 
            var roleList = new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.GroupAdmin, UserRoleEnum.User };

            // foto je přidáno pouze aktivním uživatelům
            var userList = await _userService.SelectUserByGroupIdActiveRoleId(groupId, true, roleList) ?? throw new Exception($"GroupId {groupId} neobsahuje žádné uživatele.");

            foreach (var user in userList)
                userKeyList.Add(await GetUserKey(user.Id));

            return userKeyList;
        }

        private async Task<UserKeyModel> GetUserKey(int userId)
        {
            var userKeys = await _userKeysService.SelectByUserId(userId) ?? throw new Exception($"Uživatele {userId} nemá přidělen žádné šifrovací klíče.");

            return new UserKeyModel
            {
                UserId = userId,
                KeyPem = userKeys.RSAPublicKey,
                Nonce = userKeys.PublicKeyNonce,
                Tag = userKeys.PublicKeyTag,
            };
        }

        private PhotoThumbnailModel GetPhotoThumbnail(ref string fSThumbnailName, int thumbnailDataLengt, PhotoModel photoModel)
        {
            fSThumbnailName = FileHelper.GetFileName(photoModel.PhotoName, _THUMBNAIL);

            return new PhotoThumbnailModel
            {
                CreateAuthor = photoModel.CreateAuthor,
                CreateDateTime = photoModel.CreateDateTime,
                FileSize = thumbnailDataLengt,
                FSThumbnailName = fSThumbnailName,
            };
        }

        private async Task<byte[]> GetByteFromStream(Stream data)
        {
            using (var ms = new MemoryStream())
            {
                await data.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
