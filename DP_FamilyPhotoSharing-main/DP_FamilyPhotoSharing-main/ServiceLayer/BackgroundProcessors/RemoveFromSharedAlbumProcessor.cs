using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Transactions;
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

namespace ServiceLayer.BackgroundProcessors
{
    public class RemoveFromSharedAlbumProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        public RemoveFromSharedAlbumProcessor(IPhotoEncryptService photoEncryptService, IUserService userService, 
            ICryptoService cryptoService, ISystemLogService systemLogService, IUserKeysService userKeysService,
            IPhotoService photoService, IPhotoThumbnailService photoThumbnailService, IPhotoTransaction photoTransaction,
            ISharedPhotoAlbumService sharedPhotoAlbumService, IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao,
            IRelationUserSharedAlbumDao relationUserSharedAlbumDao) 
            : base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _photoService = photoService;
            _photoThumbnailService = photoThumbnailService;
            _photoTransaction = photoTransaction;
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var photoRemove = job.RemoveFromSharedAlbumBGModel;
            var sharedAlbumName = "";

            if (photoRemove == null || !photoRemove.PhotoIds.Any())
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            try
            {
                job.Status = JobStatus.RUNNING;

                var groupUsers = await GetAllUsers(photoRemove.GroupId);
                groupUsers = groupUsers.Where(u => u.RoleId != (int)UserRoleEnum.Host).ToList();
                var relationSharedAlbumPhoto = new List<RelationSharedAlbumPhotoModel>();
                var encryptListDelete = new List<PhotoEncryptDeleteStruct>();
                var sharedAlbum = await _sharedPhotoAlbumService.Get(photoRemove.AlbumId);
                sharedAlbumName = sharedAlbum.AlbumName;

                foreach (var fileId in photoRemove.PhotoIds)
                {
                    var photo = await _photoService.Get(fileId);
                    var thumbnail = _photoThumbnailService.Get(fileId);
                    var owner = groupUsers.Where(u => u.Id == photo.OwnerId).First();
                    var usersNotRemoveList = new List<int>();

                    if (photo == null)
                        throw new Exception($"Fotografie s id: {fileId} nebyla nalezena.");

                    // Načte uživatele co mají přístup k fotografii v jiných sdílenných albech
                    var albumsWithPhoto = await _relationSharedAlbumPhotoDao.SelectByPhotoId(fileId);

                    if (albumsWithPhoto.Count > 1)
                    {
                        var userIdsOtherAlbums = new List<int>();

                        foreach (var album in albumsWithPhoto.Where(a => a.SharedAlbumId != photoRemove.AlbumId).ToList())
                        {
                            var rel = await _relationUserSharedAlbumDao.SelectBySharedAlbumId(album.Id);

                            if(rel != null && rel.Any())
                                userIdsOtherAlbums.AddRange(rel.Select(u => u.Id).ToList());
                        }
                        
                        // Přidám do seznamu uživatele co mají přístup k fotografie v jiném sdílenném albu
                        if (photo.Personal)
                            usersNotRemoveList.AddRange(userIdsOtherAlbums);
                        else
                            usersNotRemoveList.AddRange(userIdsOtherAlbums);
                    }

                    if (photo.Personal) // odstranit všechny uživatele mimo ownera
                        usersNotRemoveList.Add(owner.Id);
                    else // odstranit všechny encryption mimo uživatele group
                        usersNotRemoveList.AddRange(groupUsers.Select(u => u.Id).ToList());

                    encryptListDelete.Add(new PhotoEncryptDeleteStruct
                    {
                        FileId = fileId,
                        FileTypeEnum = FileTypeEnum.Photo,
                        NotDeleteUsers = usersNotRemoveList.Distinct().ToList(),
                    });
                    encryptListDelete.Add(new PhotoEncryptDeleteStruct
                    {
                        FileId = thumbnail.Id,
                        FileTypeEnum = FileTypeEnum.Thumbnail,
                        NotDeleteUsers = usersNotRemoveList.Distinct().ToList(),
                    });

                    relationSharedAlbumPhoto.Add(new RelationSharedAlbumPhotoModel
                    {
                        PhotoId = fileId,
                        SharedAlbumId = photoRemove.AlbumId
                    });

                    // 1 vyjímečný případ, smazat titulní fotky ve sdílených albech pokud existují
                    if (sharedAlbum != null && sharedAlbum.TitlePhotoId == fileId)
                    {
                        sharedAlbum.TitlePhotoId = 0;
                        await _sharedPhotoAlbumService.Update(sharedAlbum);
                    }

                    Interlocked.Increment(ref job.Processed);
                }

                await _photoTransaction.RemoveFromSharedAlbum(encryptListDelete, relationSharedAlbumPhoto);
                
                await LogInfo(ActionTypeEnum.SharedPhotoAlbum, $"Foto id {string.Join(",", photoRemove.PhotoIds)} byly smazány ze sdíleného alba {sharedAlbumName}.", photoRemove.GroupId, 0, photoRemove.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message, photoRemove.GroupId, 0, photoRemove.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }

    
}
