using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dao.RelationUserSharedAlbum;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.RelationUserSharedAlbum;
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
using static System.Net.WebRequestMethods;

namespace ServiceLayer.BackgroundProcessors
{
    public class SharedPhotoAlbumDeleteProcessor : BaseProcessor, IJobProcessor
    {
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IRelationUserSharedAlbumDao _relationUserSharedAlbumDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoAlbumTransaction _photoAlbumTransaction;
        public SharedPhotoAlbumDeleteProcessor(IPhotoEncryptService photoEncryptService, IUserService userService,
            ICryptoService cryptoService, ISystemLogService systemLogService, IUserKeysService userKeysService,
            ISharedPhotoAlbumService sharedPhotoAlbumService, IRelationUserSharedAlbumDao relationUserSharedAlbumDao,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IPhotoService photoService,
            IPhotoThumbnailService photoThumbnailService, IPhotoAlbumTransaction photoAlbumTransaction) :
            base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _relationUserSharedAlbumDao = relationUserSharedAlbumDao;
            _photoService = photoService;
            _photoThumbnailService = photoThumbnailService;
            _photoAlbumTransaction = photoAlbumTransaction;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var albumDelete = job.SharedPhotoAlbumDeleteBGModel;

            if (albumDelete == null)
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            try
            {
                job.Status = JobStatus.RUNNING;

                var album = await _sharedPhotoAlbumService.Get(albumDelete.AlbumId);
                var albumUsers = (await _relationUserSharedAlbumDao.SelectBySharedAlbumId(albumDelete.AlbumId))
                                    .Select(u => u?.ToRelationUserSharedAlbumModel())
                                    .ToList();
                var albumPhotos = (await _relationSharedAlbumPhotoDao.SelectBySharedAlbumId(albumDelete.AlbumId))
                                    .Select(p => p?.ToRelationSharedAlbumPhotoModel())
                                    .ToList();
                var groupUsers = await GetAllUsers(albumDelete.GroupId);
                var encryptListDelete = new List<PhotoEncryptDeleteStruct>();

                if (album.OwnerUserId != albumDelete.UserId) // Mazat alba smí jen vlastník alba
                {
                    job.Processed = job.Total;
                    job.Status = JobStatus.COMPLETED;
                    return;
                }

                job.Total = albumPhotos.Count;

                foreach (var rel in albumPhotos)
                {
                    var photo = await _photoService.Get(rel.PhotoId);
                    var thumbnail = _photoThumbnailService.Get(rel.PhotoId);
                    var owner = groupUsers.Where(u => u.Id == photo.OwnerId).First();
                    var usersNotRemoveList = new List<int>();

                    if (photo == null)
                        throw new Exception($"Fotografie s id: {rel.PhotoId} nebyla nalezena.");

                    var albumsWithPhoto = await _relationSharedAlbumPhotoDao.SelectByPhotoId(rel.PhotoId);
                    if (albumsWithPhoto.Count > 1)
                    {
                        var userIdsOtherAlbums = new List<int>();

                        foreach (var albumTemp in albumsWithPhoto.Where(a => a.SharedAlbumId != albumDelete.AlbumId).ToList())
                        {
                            var relTemp = await _relationUserSharedAlbumDao.SelectBySharedAlbumId(albumTemp.Id);

                            if (relTemp != null && relTemp.Any())
                                userIdsOtherAlbums.AddRange(relTemp.Select(u => u.Id).ToList());
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
                        FileId = rel.PhotoId,
                        FileTypeEnum = FileTypeEnum.Photo,
                        NotDeleteUsers = usersNotRemoveList.Distinct().ToList(),
                    });
                    encryptListDelete.Add(new PhotoEncryptDeleteStruct
                    {
                        FileId = thumbnail.Id,
                        FileTypeEnum = FileTypeEnum.Thumbnail,
                        NotDeleteUsers = usersNotRemoveList.Distinct().ToList()
                    });

                    Interlocked.Increment(ref job.Processed);
                }

                // relace na delete, shared album ID
                var ralationPhotosDel = albumPhotos.Select(p => p?.ToRelationSharedAlbumPhotoDelete()).ToList();
                var relationUserDel = albumUsers.Select(p => p?.ToRelationSharedAlbumDelete()).ToList();

                await _photoAlbumTransaction.DeleteSharedPhotoAlbum(albumDelete.AlbumId, encryptListDelete, ralationPhotosDel, relationUserDel);

                await LogInfo(ActionTypeEnum.RemoveSharedPhotoAlbum, $"Foto id {string.Join(",", albumPhotos.Select(p => p.Id).ToList())} " +
                    $"byly odstraněny ze sdíleného alba {album.AlbumName} a album bylo následně smazáno.", albumDelete.GroupId, 0, albumDelete.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.RemoveSharedPhotoAlbum, e.Message, albumDelete.GroupId, 0, albumDelete.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }
}
