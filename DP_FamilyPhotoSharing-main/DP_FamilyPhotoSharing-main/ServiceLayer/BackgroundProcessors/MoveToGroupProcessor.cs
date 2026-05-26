using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Transactions;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using FileAccessLayer;
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
    public class MoveToGroupProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IPhotoAlbumService _photoAlbumService;
        public MoveToGroupProcessor(ISystemLogService systemLogService, IPhotoService photoService, IPhotoThumbnailService photoThumbnailService,
            ICryptoService cryptoService, IUserService userService, IUserKeysService userKeysService, IPhotoEncryptService photoEncryptService,
            IPhotoTransaction photoTransaction, IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IPhotoAlbumService photoAlbumService) : 
            base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _photoService = photoService;
            _photoThumbnailService  = photoThumbnailService;
            _photoTransaction = photoTransaction;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _photoAlbumService = photoAlbumService;
        }   

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var photoMove = job.MoveToGroupBGModel;

            if (photoMove == null || !photoMove.PhotoIds.Any())
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            try
            {
                job.Status = JobStatus.RUNNING;

                RelationAlbumPhotoModel relation = null;

                if (photoMove.AlbumId != 0)
                    relation = new RelationAlbumPhotoModel
                    {
                        AlbumId = photoMove.AlbumId,
                        GroupId = photoMove.GroupId,
                        PhotoId = 0,
                    };

                PhotoAlbumModel photoAlbum = null;
                if(photoMove.AlbumId != 0)
                    photoAlbum = await _photoAlbumService.Get(photoMove.AlbumId);

                // Načtení všech uživatelů ze skupiny
                var owner = await _userService.Get(photoMove.UserId);
                var users = await GetAllUsers(photoMove.GroupId);

                // Načtení klíčů
                var keys = await GetAllKeys(users);

                // Filtrace uživatelů
                users = users.Where(u => u.Id != owner.Id && u.RoleId != (int)UserRoleEnum.Host).ToList();

                var ownerKey = keys.Where(k => k.UserId == photoMove.UserId).First();
                var decryptedKey = new UserKeyModel
                {
                    KeyPem = ownerKey.RSAPrivateKey,
                    Nonce = ownerKey.PrivateKeyNonce,
                    Tag = ownerKey.PrivateKeyTag,
                    UserId = ownerKey.UserId,
                };

                foreach (var fileId in photoMove.PhotoIds)
                {
                    var ownerList = new List<int> { owner.Id };

                    if(photoAlbum != null && photoAlbum.TitlePhotoId == fileId)
                    {
                        photoAlbum.TitlePhotoId = 0;
                        await _photoAlbumService.Update(photoAlbum);
                    }

                    // Příparava zrušení relace pokud se nacházíme v albu
                    if (relation != null)
                        relation.PhotoId = fileId;

                    // Načtení sdílených alb, ve kterých se fotografie nachází
                    var sharedAlbums = (await _relationSharedAlbumPhotoDao.SelectByPhotoId(fileId))?
                        .Select(sa => sa?.ToRelationSharedAlbumPhotoModel())
                        .ToList();

                    // Načtení souborů
                    var photo = await _photoService.Get(fileId);
                    var thumbnail = await _photoThumbnailService.Get(fileId);
                    var photoEncryp = await _photoEncryptService.GetByUserIdPhotoId(photoMove.UserId, photo.Id, FileTypeEnum.Photo);
                    var thumbnailEncryp = await _photoEncryptService.GetByUserIdPhotoId(photoMove.UserId, thumbnail.Id, FileTypeEnum.Thumbnail);
                    photo.Personal = false;

                    var encryptedPhoto = GetEncryptedDataModel(photoEncryp);
                    var encryptedThumbnail = GetEncryptedDataModel(thumbnailEncryp);
                    var photoEncryptModel = AddEncryptedForNewUsers(users, keys, encryptedPhoto, decryptedKey, fileId, FileTypeEnum.Photo);
                    var thumbnailEncryptModel = AddEncryptedForNewUsers(users, keys, encryptedThumbnail, decryptedKey, thumbnail.Id, FileTypeEnum.Thumbnail);

                    await _photoTransaction.MoveToGroup(fileId, thumbnail.Id, ownerList, sharedAlbums, relation, photo, photoEncryptModel, thumbnailEncryptModel);

                    Interlocked.Increment(ref job.Processed);
                }                

                await LogInfo(ActionTypeEnum.Photo, $"Foto id {string.Join(",", photoMove.PhotoIds)} byly přesunuty ze soukromé galerie do rodinné group id {photoMove.GroupId}", photoMove.GroupId, 0, photoMove.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.Photo, e.Message, photoMove.GroupId, 0, photoMove.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }

        
    }
}