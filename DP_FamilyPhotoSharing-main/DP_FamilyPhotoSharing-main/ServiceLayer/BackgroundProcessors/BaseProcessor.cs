using DataAccessLayer.Dto.PhotoAlbum;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using ModelLayer.Data;
using ModelLayer.Enums;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundProcessors
{
    public abstract class BaseProcessor
    {
        protected readonly ISystemLogService _systemLogService;
        protected readonly IUserService _userService;
        protected readonly IPhotoEncryptService _photoEncryptService;
        protected readonly IUserKeysService _userKeysService;
        protected readonly ICryptoService _cryptoService;
        public BaseProcessor(IPhotoEncryptService photoEncryptService, IUserService userService, ICryptoService cryptoService, 
            ISystemLogService systemLogService, IUserKeysService userKeysService)
        {
            _systemLogService = systemLogService;
            _cryptoService = cryptoService;
            _photoEncryptService = photoEncryptService;
            _userService = userService;
            _userKeysService = userKeysService;
        }
        protected async Task LogInfo(ActionTypeEnum actionType, string message, int groupId, int roleId, int userId)
        {
            var log = SystemLogModel.GetLog(LogTypeEnum.Ok, actionType, message, groupId, roleId, userId);
            await _systemLogService.Set(log);
        }

        protected async Task LogError(ActionTypeEnum actionType, string message, int groupId, int roleId, int userId)
        {
            var log = SystemLogModel.GetLog(LogTypeEnum.Error, actionType, message, groupId, roleId, userId);
            await _systemLogService.Set(log);
        }

        protected EncryptedDataModel GetEncryptedDataModel(PhotoEncryptModel model) => new EncryptedDataModel
        {
            AesKeyEncrypted = model.Aes,
            Nonce = model.Nonce,
            Tag = model.Tag,
            UserId = model.UserId,
            AesKeyPlain = null,
            EncrptData = null
        };

        protected List<PhotoEncryptModel> AddEncryptedForNewUsers(List<UserModel> users, List<UserKeysModel> keys, EncryptedDataModel encryptedData, UserKeyModel decryptedKey, int fileId, FileTypeEnum fileType)
        {
            var newEncryptedDataList = new List<PhotoEncryptModel>();

            foreach (var user in users)
            {
                var userKey = keys.Where(k => k.UserId == user.Id).First();

                var encryptedKey = new UserKeyModel
                {
                    UserId = user.Id,
                    KeyPem = userKey.RSAPublicKey,
                    Nonce = userKey.PublicKeyNonce,
                    Tag = userKey.PublicKeyTag,
                };

                var newEncryptedData = _cryptoService.AddUserToEncryptedData(encryptedData, decryptedKey, encryptedKey);
                newEncryptedDataList.Add(new PhotoEncryptModel
                {
                    FileType = fileType,
                    Aes = newEncryptedData.AesKeyEncrypted,
                    CreateAuthorId = user.Id,
                    CreateDateTime = DateTime.UtcNow,
                    FileId = fileId,
                    Id = 0,
                    Nonce = newEncryptedData.Nonce,
                    Tag = newEncryptedData.Tag,
                    UserId = newEncryptedData.UserId
                });

                /* Test decryption 
                var file = await _downloadFile.Download(photo.FSFileName, "main_group");
                var decryptData = _cryptoService.DecryptData(file, newEncryptedPhoto, new UserKeyModel
                {
                    KeyPem = userKey.RSAPrivateKey,
                    Nonce = userKey.PrivateKeyNonce,
                    Tag= userKey.PrivateKeyTag,
                    UserId = userKey.UserId,
                });
                */
            }

            return newEncryptedDataList;
        }

        protected async Task<List<UserModel>> GetAllUsers(int groupId, bool activated = true)
        {
            var userRoleList = new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.GroupAdmin, UserRoleEnum.User, UserRoleEnum.Host };
            var users = await _userService.SelectUserByGroupIdActiveRoleId(groupId, true, userRoleList, activated);
            var usersInactive = await _userService.SelectUserByGroupIdActiveRoleId(groupId, false, userRoleList, activated);
            users ??= new List<UserModel>();
            users.AddRange(usersInactive);

            return users;
        }

        protected async Task<List<UserKeysModel>> GetAllKeys(List<UserModel> users)
        {
            var keys = new List<UserKeysModel>();
            foreach (var user in users)
            {
                var key = await _userKeysService.SelectByUserId(user.Id);
                keys.Add(key);
            }

            return keys;
        }
    }

    
}
