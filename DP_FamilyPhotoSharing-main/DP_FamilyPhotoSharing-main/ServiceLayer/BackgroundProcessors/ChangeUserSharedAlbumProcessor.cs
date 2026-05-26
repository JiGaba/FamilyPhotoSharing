using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Transactions;
using EncryptionLayer.Model;
using EncryptionLayer.Photo;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.Structs;
using ServiceLayer.BackgroundInterfaces;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.PhotoThumbnail;
using ServiceLayer.Services.SharedPhotoAlbum;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ServiceLayer.BackgroundProcessors
{
    public class ChangeUserSharedAlbumProcessor : BaseProcessor, IJobProcessor
    {
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoTransaction _photoTransaction;
        public ChangeUserSharedAlbumProcessor(IPhotoEncryptService photoEncryptService, IUserService userService, 
            ICryptoService cryptoService, ISystemLogService systemLogService, IUserKeysService userKeysService,
            ISharedPhotoAlbumService sharedPhotoAlbumService, IRelationUserSharedAlbumDao relationUserSharedAlbumDao,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IPhotoService photoService,
            IPhotoThumbnailService photoThumbnailService, IPhotoTransaction photoTransaction) : 
            base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
            _photoService = photoService;
            _photoThumbnailService = photoThumbnailService;
            _photoTransaction = photoTransaction;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var sharedAlbumChange = job.ChangeUserSharedAlbumBGModel;

            if (sharedAlbumChange == null || (!sharedAlbumChange.UserIdRemove.Any() && !sharedAlbumChange.UserIdAdd.Any()))
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            try
            {
                job.Status = JobStatus.RUNNING;

                var usersAll = await GetAllUsers(sharedAlbumChange.GroupId);
                var groupUsers = usersAll.Where(u => u.RoleId != (int)UserRoleEnum.Host).ToList();
                var album = await _sharedPhotoAlbumService.Get(sharedAlbumChange.AlbumId);
                var owner = usersAll.Where(u => u.Id == album.OwnerUserId).First();
                var photos = (await _relationSharedAlbumPhotoDao.SelectBySharedAlbumId(sharedAlbumChange.AlbumId))
                                .Select(p => p?.ToRelationSharedAlbumPhotoModel())
                                .ToList();
                var albumUsers = (await _relationUserSharedAlbumDao.SelectBySharedAlbumId(sharedAlbumChange.AlbumId))
                                .Select(u => u?.ToRelationUserSharedAlbumModel())
                                .ToList();

                job.Total = photos.Count;

                var removeUsersIsAllIn = sharedAlbumChange.UserIdRemove
                    .All(u => albumUsers.Select(au => au.UserId).ToList().Contains(u));

                var addUsersNotInAny = !sharedAlbumChange.UserIdAdd
                    .Any(u => albumUsers.Select(au => au.UserId).ToList().Contains(u));

                if (!addUsersNotInAny || !removeUsersIsAllIn)
                    throw new Exception("Uživatel, kterého jste chtěli přidat/odebrat se již nachází/nenachází v albu.");

                var ownerKey = await _userKeysService.SelectByUserId(owner.Id);
                var decryptedKey = new UserKeyModel
                {
                    UserId = ownerKey.Id,
                    KeyPem = ownerKey.RSAPrivateKey,
                    Nonce = ownerKey.PrivateKeyNonce,
                    Tag = ownerKey.PrivateKeyTag,
                };

                var encryptListDelete = new List<PhotoEncryptDeleteStruct>();
                var encryptListAdd = new List<PhotoEncryptModel>();

                var addUsers = new List<UserModel>();
                foreach(var id in sharedAlbumChange.UserIdAdd)
                    addUsers.Add(await _userService.Get(id));

                foreach (var photoRel in photos)
                {
                    // 1) Add users to shared photo album
                    var photo = await _photoService.Get(photoRel.PhotoId);
                    var thumbnail = await _photoThumbnailService.Get(photo.Id);
                    var photoEncryp = await _photoEncryptService.GetByUserIdPhotoId(owner.Id, photo.Id, FileTypeEnum.Photo);
                    var thumbnailEncryp = await _photoEncryptService.GetByUserIdPhotoId(owner.Id, thumbnail.Id, FileTypeEnum.Thumbnail);

                    var encryptedPhoto = GetEncryptedDataModel(photoEncryp);
                    var encryptedThumbnail = GetEncryptedDataModel(thumbnailEncryp);

                    foreach (var user in addUsers)
                    {
                        // zjisti zda user má už šifrování na danou fotku
                        var result = await _photoEncryptService.GetByUserIdPhotoId(user.Id, photo.Id, FileTypeEnum.Photo);
                        if (result != null)
                            continue;

                        var encryptedKeys = await _userKeysService.SelectByUserId(user.Id);
                        var encryptedKey = new UserKeyModel
                        {
                            UserId = encryptedKeys.Id,
                            KeyPem = encryptedKeys.RSAPublicKey,
                            Nonce = encryptedKeys.PublicKeyNonce,
                            Tag = encryptedKeys.PublicKeyTag,
                        };

                        var newEncryptedPhoto = _cryptoService.AddUserToEncryptedData(encryptedPhoto, decryptedKey, encryptedKey);
                        encryptListAdd.Add(new PhotoEncryptModel
                        {
                            FileType = FileTypeEnum.Photo,
                            Aes = newEncryptedPhoto.AesKeyEncrypted,
                            CreateAuthorId = sharedAlbumChange.UserId,
                            CreateDateTime = DateTime.UtcNow,
                            FileId = photo.Id,
                            Id = 0,
                            Nonce = newEncryptedPhoto.Nonce,
                            Tag = newEncryptedPhoto.Tag,
                            UserId = user.Id
                        });

                        var newEncryptedThumbnail = _cryptoService.AddUserToEncryptedData(encryptedThumbnail, decryptedKey, encryptedKey);
                        encryptListAdd.Add(new PhotoEncryptModel
                        {
                            FileType = FileTypeEnum.Thumbnail,
                            Aes = newEncryptedThumbnail.AesKeyEncrypted,
                            CreateAuthorId = sharedAlbumChange.UserId,
                            CreateDateTime = DateTime.UtcNow,
                            FileId = thumbnail.Id,
                            Id = 0,
                            Nonce = newEncryptedThumbnail.Nonce,
                            Tag = newEncryptedThumbnail.Tag,
                            UserId = user.Id
                        });
                    }

                    // 2) Remove users from shared photo album
                    if(sharedAlbumChange.UserIdRemove.Count > 0)
                    {
                        var usersNotRemoveList = new List<int>();

                        // původní uživatelé bez těch mazaných
                        usersNotRemoveList.AddRange(albumUsers
                            .Select(u => u.UserId)
                            .ToList()
                            .Except(sharedAlbumChange.UserIdRemove)
                            .ToList());

                        // nově přidávaní uživatelé
                        if (sharedAlbumChange.UserIdAdd.Count > 0)
                            usersNotRemoveList.AddRange(sharedAlbumChange.UserIdAdd);

                        // Načte uživatele co mají přístup k fotografii v jiných sdílenných albech
                        var albumsWithPhoto = await _relationSharedAlbumPhotoDao.SelectByPhotoId(photo.Id);

                        if (albumsWithPhoto.Count > 1)
                        {
                            var userIdsOtherAlbums = new List<int>();

                            foreach (var albumTemp in albumsWithPhoto.Where(a => a.SharedAlbumId != sharedAlbumChange.AlbumId).ToList())
                            {
                                var rel = await _relationUserSharedAlbumDao.SelectBySharedAlbumId(albumTemp.Id);

                                if (rel != null && rel.Any())
                                    userIdsOtherAlbums.AddRange(rel.Select(u => u.Id).ToList());
                            }

                            // Přidám do seznamu uživatele co mají přístup k fotografie v jiném sdílenném albu
                            usersNotRemoveList.AddRange(userIdsOtherAlbums);
                        }

                        if (photo.Personal) // odstranit všechny uživatele mimo ownera
                            usersNotRemoveList.Add(owner.Id);
                        else // odstranit všechny encryption mimo uživatele group
                            usersNotRemoveList.AddRange(groupUsers.Select(u => u.Id).ToList());

                        encryptListDelete.Add(new PhotoEncryptDeleteStruct
                        {
                            FileId = photo.Id,
                            FileTypeEnum = FileTypeEnum.Photo,
                            NotDeleteUsers = usersNotRemoveList.Distinct().ToList(),
                        });
                        encryptListDelete.Add(new PhotoEncryptDeleteStruct
                        {
                            FileId = thumbnail.Id,
                            FileTypeEnum = FileTypeEnum.Thumbnail,
                            NotDeleteUsers = usersNotRemoveList.Distinct().ToList(),
                        });
                    }              

                    Interlocked.Increment(ref job.Processed);
                }

                // relace s usery na přidání
                var ralationUserAdd = new List<RelationUserSharedAlbumModel>();
                if (sharedAlbumChange.UserIdAdd != null && sharedAlbumChange.UserIdAdd.Any())
                    foreach (var userId in sharedAlbumChange.UserIdAdd)
                        ralationUserAdd.Add(new RelationUserSharedAlbumModel
                        {
                            CreateAuthorId = sharedAlbumChange.UserId,
                            UserId = userId,
                            SharedAlbumId = sharedAlbumChange.AlbumId,
                        });          

                // Relace s usery na smazání
                var ralationUserDel = new List<RelationUserSharedAlbumModel>();
                if(sharedAlbumChange.UserIdRemove != null && sharedAlbumChange.UserIdRemove.Any())
                    ralationUserDel = albumUsers.Where(u => sharedAlbumChange.UserIdRemove.Contains(u.UserId)).ToList();

                await _photoTransaction.ChangeUsersInAlbum(encryptListDelete, encryptListAdd, ralationUserAdd, ralationUserDel);

                await LogInfo(ActionTypeEnum.SharedPhotoAlbum, $"Z alba byli odstraněni uživatelé s id {string.Join(",", sharedAlbumChange.UserIdRemove)} a byli přidání uživatelé s id " +
                    $"{string.Join(",", sharedAlbumChange.UserIdRemove)}.", sharedAlbumChange.GroupId, (int)sharedAlbumChange.UserRoleEnum, sharedAlbumChange.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message, sharedAlbumChange.GroupId, (int)sharedAlbumChange.UserRoleEnum, sharedAlbumChange.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }
}
