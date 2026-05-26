using DataAccessLayer.Transactions;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using Microsoft.VisualBasic.FileIO;
using ModelLayer.Data;
using ModelLayer.Enums;
using ServiceLayer.BackgroundInterfaces;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.PhotoThumbnail;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ServiceLayer.BackgroundProcessors
{
    public class AddUserToGroupProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoService _photoService;
        private readonly IPhotoAlbumService _photoAlbumService;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        public AddUserToGroupProcessor(ICryptoService cryptoService, ISystemLogService systemLogService, 
            IPhotoService photoService, IUserService userService, IPhotoEncryptService photoEncryptService,
            IPhotoTransaction photoTransaction, IPhotoAlbumService photoAlbumService, 
            IPhotoThumbnailService photoThumbnailService, IUserKeysService userKeysService) : base(photoEncryptService, 
                userService, cryptoService, systemLogService, userKeysService)
        {
            _photoService = photoService;
            _photoAlbumService = photoAlbumService;
            _photoTransaction = photoTransaction;
            _photoThumbnailService = photoThumbnailService;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var addUserModel = job.AddUserToGroupBGModel;

            if (addUserModel == null)
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }

            try
            {
                job.Status = JobStatus.RUNNING;

                var usersAll = await GetAllUsers(addUserModel.GroupId);
                var users = usersAll.Where(u => u.Id != addUserModel.UserId || u.RoleId != (int)UserRoleEnum.Host).ToList();
                var owner = await _userService.Get(addUserModel.UserId);
                var decryptUser = usersAll?.ElementAt(0);

                if (owner == null)
                    throw new Exception($"Nový uživatel s id {addUserModel.UserId} nebyl nalezen v databázi.");

                if(owner.RoleId == (int)UserRoleEnum.Host)
                {
                    owner.Activated = true;
                    await _photoTransaction.AddUserToGroup(null, owner);

                    job.Processed = job.Total;
                    job.Status = JobStatus.COMPLETED;

                    await LogInfo(ActionTypeEnum.AddUser, $"Přidání uživatele {owner.GetName()} [{owner.Id}] do rodiny [{addUserModel.GroupId}]. Jedné se pouze u roli HOST.", addUserModel.GroupId, 0, addUserModel.UserId);
                    return;
                }

                if (users == null || !users.Any())
                {
                    job.Processed = job.Total;
                    job.Status = JobStatus.COMPLETED;

                    await LogInfo(ActionTypeEnum.AddUser, $"Přidání uživatele {owner.GetName()} [{owner.Id}] do rodiny [{addUserModel.GroupId}]. Jedné se o prvotního uživatele v rodině.", addUserModel.GroupId, 0, addUserModel.UserId);
                    return;
                }

                var photoList = await GetAllPhotoInGroup(addUserModel.GroupId);
                job.Total = photoList.Count;

                var userDecryptKey = await _userKeysService.SelectByUserId(decryptUser.Id);
                var decryptedKey = new UserKeyModel
                {
                    KeyPem = userDecryptKey.RSAPrivateKey,
                    Nonce = userDecryptKey.PrivateKeyNonce,
                    Tag = userDecryptKey.PrivateKeyTag,
                    UserId = userDecryptKey.UserId,
                };

                var ownerKey = await _userKeysService.SelectByUserId(owner.Id);
                var encryptedKey = new UserKeyModel
                {
                    UserId = ownerKey.Id,
                    KeyPem = ownerKey.RSAPublicKey,
                    Nonce = ownerKey.PublicKeyNonce,
                    Tag = ownerKey.PublicKeyTag,
                };

                var newEncryptedDataList = new List<PhotoEncryptModel>();

                foreach (var file in photoList)
                {
                    var thumbnail = await _photoThumbnailService.Get(file.Id);
                    var photoEncryp = await _photoEncryptService.GetByUserIdPhotoId(decryptUser.Id, file.Id, FileTypeEnum.Photo);
                    var thumbnailEncryp = await _photoEncryptService.GetByUserIdPhotoId(decryptUser.Id, thumbnail.Id, FileTypeEnum.Thumbnail);

                    var encryptedPhoto = GetEncryptedDataModel(photoEncryp);
                    var encryptedThumbnail = GetEncryptedDataModel(thumbnailEncryp);

                    var newEncryptedPhoto = _cryptoService.AddUserToEncryptedData(encryptedPhoto, decryptedKey, encryptedKey);
                    newEncryptedDataList.Add(new PhotoEncryptModel
                    {
                        FileType = FileTypeEnum.Photo,
                        Aes = newEncryptedPhoto.AesKeyEncrypted,
                        CreateAuthorId = addUserModel.AuthorId,
                        CreateDateTime = DateTime.UtcNow,
                        FileId = file.Id,
                        Id = 0,
                        Nonce = newEncryptedPhoto.Nonce,
                        Tag = newEncryptedPhoto.Tag,
                        UserId = owner.Id,
                    });

                    var newEncryptedThumbnail = _cryptoService.AddUserToEncryptedData(encryptedThumbnail, decryptedKey, encryptedKey);
                    newEncryptedDataList.Add(new PhotoEncryptModel
                    {
                        FileType = FileTypeEnum.Thumbnail,
                        Aes = newEncryptedThumbnail.AesKeyEncrypted,
                        CreateAuthorId = addUserModel.AuthorId,
                        CreateDateTime = DateTime.UtcNow,
                        FileId = thumbnail.Id,
                        Id = 0,
                        Nonce = newEncryptedThumbnail.Nonce,
                        Tag = newEncryptedThumbnail.Tag,
                        UserId = owner.Id
                    });

                    Interlocked.Increment(ref job.Processed);
                }

                owner.Activated = true;
                await _photoTransaction.AddUserToGroup(newEncryptedDataList, owner);
                
                await LogInfo(ActionTypeEnum.AddUser, $"Přidání uživatele {owner.GetName()} [{owner.Id}] do rodiny [{addUserModel.GroupId}].", addUserModel.GroupId, 0, addUserModel.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.AddUser, e.Message, addUserModel.GroupId, 0, addUserModel.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }

        private async Task<List<PhotoModel>> GetAllPhotoInGroup(int groupId)
        {
            var photoList = new List<PhotoModel>();
            var photoCount = await _photoService.GetPhotoCountByGroupId(groupId);

            if (photoCount != 0)
                photoList = await _photoService.SelectByGroupId(groupId, photoCount, 0);
            var photoAlbums = await _photoAlbumService.GetByGroupsId(groupId);

            if(photoAlbums != null)
                foreach (var photoAlbum in photoAlbums)
                {
                    var photoAlbumCount = await _photoService.GetPhotoCountByAlbumId(photoAlbum.Id);

                    if (photoAlbumCount == 0)
                        continue;

                    var photos = await _photoService.SelectByAlbumId(photoAlbum.Id, photoAlbumCount, 0);

                    photoList ??= new List<PhotoModel>();
                    photoList.AddRange(photos);
                }

            return photoList;
        }
    }
}
