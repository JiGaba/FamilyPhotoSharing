using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Transactions;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using ModelLayer.Data;
using ModelLayer.Enums;
using ServiceLayer.BackgroundInterfaces;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.PhotoThumbnail;
using ServiceLayer.Services.SharedPhotoAlbum;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundProcessors
{
    public class AddPhotoToSharedAlbumProcessor : BaseProcessor, IJobProcessor
    {
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        public AddPhotoToSharedAlbumProcessor(IPhotoEncryptService photoEncryptService, IUserService userService, 
            ICryptoService cryptoService, ISystemLogService systemLogService, IUserKeysService userKeysService,
            ISharedPhotoAlbumService sharedPhotoAlbumService, IRelationUserSharedAlbumDao relationUserSharedAlbumDao,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IPhotoTransaction photoTransaction,
            IPhotoThumbnailService photoThumbnailService) 
            : base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _photoTransaction = photoTransaction;
            _photoThumbnailService = photoThumbnailService;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var addPhoto = job.AddPhotoToSharedAlbumBGModel;

            if (addPhoto == null || !addPhoto.PhotoIds.Any())
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            // Do soukromého sdíleného alba může přidávat jen vlastník
            // do rodinného sdíleného alba jen admin nebo groupAdmin - ale vkastník alba je vždy člen rodiny
            // má tedy již našifrované fotografie proto můžu dešifrovat vždy jeho klíčem
            try
            {
                job.Status = JobStatus.RUNNING;

                var sharedAlbum = await _sharedPhotoAlbumService.Get(addPhoto.SharedAlbumId);
                var albumOwner = await _userService.Get(sharedAlbum.OwnerUserId);
                var albumUsers = (await _relationUserSharedAlbumDao.SelectBySharedAlbumId(addPhoto.SharedAlbumId))
                                    .Select(a => a.ToRelationUserSharedAlbumModel())
                                    .ToList()
                                    ?? new List<RelationUserSharedAlbumModel>();
                var albumPhotos = await _relationSharedAlbumPhotoDao.SelectBySharedAlbumId(sharedAlbum.Id);

                var ownerKey = await _userKeysService.SelectByUserId(sharedAlbum.OwnerUserId);
                var decryptedKey = new UserKeyModel
                {
                    UserId = ownerKey.Id,
                    KeyPem = ownerKey.RSAPrivateKey,
                    Nonce = ownerKey.PrivateKeyNonce,
                    Tag = ownerKey.PrivateKeyTag,
                };

                var newEncryptedDataList = new List<PhotoEncryptModel>();
                var newRelationSharedAlbumPhotoList = new List<RelationSharedAlbumPhotoModel>();

                foreach (var fileId in addPhoto.PhotoIds.Except(albumPhotos.Select(p => p.PhotoId).ToList()).ToList())
                {
                    var thumbnail = await _photoThumbnailService.Get(fileId);
                    var photoEncryp = await _photoEncryptService.GetByUserIdPhotoId(sharedAlbum.OwnerUserId, fileId, FileTypeEnum.Photo);
                    var thumbnailEncryp = await _photoEncryptService.GetByUserIdPhotoId(sharedAlbum.OwnerUserId, thumbnail.Id, FileTypeEnum.Thumbnail);

                    var encryptedPhoto = GetEncryptedDataModel(photoEncryp);
                    var encryptedThumbnail = GetEncryptedDataModel(thumbnailEncryp);

                    foreach (var rel in albumUsers)
                    {
                        // zjisti zda user má už šifrování na danou fotku
                        var result =  await _photoEncryptService.GetByUserIdPhotoId(rel.UserId, fileId, FileTypeEnum.Photo);
                        if(result != null)
                            continue;
                        
                        var encryptedKeys = await _userKeysService.SelectByUserId(rel.UserId);
                        var encryptedKey = new UserKeyModel
                        {
                            UserId = encryptedKeys.Id,
                            KeyPem = encryptedKeys.RSAPublicKey,
                            Nonce = encryptedKeys.PublicKeyNonce,
                            Tag = encryptedKeys.PublicKeyTag,
                        };

                        var newEncryptedPhoto = _cryptoService.AddUserToEncryptedData(encryptedPhoto, decryptedKey, encryptedKey);
                        newEncryptedDataList.Add(new PhotoEncryptModel
                        {
                            FileType = FileTypeEnum.Photo,
                            Aes = newEncryptedPhoto.AesKeyEncrypted,
                            CreateAuthorId = addPhoto.UserId,
                            CreateDateTime = DateTime.UtcNow,
                            FileId = fileId,
                            Id = 0,
                            Nonce = newEncryptedPhoto.Nonce,
                            Tag = newEncryptedPhoto.Tag,
                            UserId = rel.UserId
                        });

                        var newEncryptedThumbnail = _cryptoService.AddUserToEncryptedData(encryptedThumbnail, decryptedKey, encryptedKey);
                        newEncryptedDataList.Add(new PhotoEncryptModel
                        {
                            FileType = FileTypeEnum.Thumbnail,
                            Aes = newEncryptedThumbnail.AesKeyEncrypted,
                            CreateAuthorId = addPhoto.UserId,
                            CreateDateTime = DateTime.UtcNow,
                            FileId = thumbnail.Id,
                            Id = 0,
                            Nonce = newEncryptedThumbnail.Nonce,
                            Tag = newEncryptedThumbnail.Tag,
                            UserId = rel.UserId
                        });
                    }

                    newRelationSharedAlbumPhotoList.Add(new RelationSharedAlbumPhotoModel
                    {
                        CreateAuthorId = addPhoto.UserId,
                        Id = 0,
                        CreateDateTime = DateTime.Now,
                        PhotoId = fileId,
                        SharedAlbumId = sharedAlbum.Id,
                    });

                    Interlocked.Increment(ref job.Processed);
                }

                await _photoTransaction.AddPhotoToSharedAlbum(newEncryptedDataList, newRelationSharedAlbumPhotoList);

                await LogInfo(ActionTypeEnum.Photo, $"Foto id {string.Join(",", addPhoto.PhotoIds)} byly sdíleny do alba {sharedAlbum.AlbumName}.", 
                    addPhoto.GroupId, (int)addPhoto.UserRoleEnum, addPhoto.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.Photo, e.Message, addPhoto.GroupId, (int)addPhoto.UserRoleEnum, addPhoto.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }
}
