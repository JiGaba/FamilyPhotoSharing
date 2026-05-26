using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Dao.PhotoThumbnailDao;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Transactions;
using EncryptionLayer.Photo;
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

namespace ServiceLayer.BackgroundProcessors
{
    public class MoveToPersonalProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoTransaction _photoTransaction;
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IPhotoAlbumService _photoAlbumService;
        public MoveToPersonalProcessor(ISystemLogService logService, IPhotoTransaction photoTransaction,
            IPhotoService photoService, IPhotoEncryptService photoEncryptService, IPhotoThumbnailService photoThumbnailService,
            ICryptoService cryptoService, IUserService userService, IUserKeysService userKeysService,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IPhotoAlbumService photoAlbumService) 
            : base(photoEncryptService, userService, cryptoService, logService, userKeysService)
        {
            _photoService = photoService;
            _photoTransaction = photoTransaction;
            _photoThumbnailService = photoThumbnailService;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _photoAlbumService = photoAlbumService;
        }
        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var photoMove = job.MoveToPersonalBGModel;

            if (photoMove == null || !photoMove.PhotoIds.Any())
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }

            try
            {
                job.Status = JobStatus.RUNNING;

                PhotoAlbumModel photoAlbum = null;
                if (photoMove.AlbumId != 0)
                    photoAlbum = await _photoAlbumService.Get(photoMove.AlbumId);

                foreach (var fileId in photoMove.PhotoIds)
                {
                    var relation = new List<RelationAlbumPhotoModel>();
                    var relationShared = new List<RelationSharedAlbumPhotoModel>();

                    if (photoMove.AlbumId != 0) 
                    {
                        relation.Add(new RelationAlbumPhotoModel
                        {
                            AlbumId = photoMove.AlbumId,
                            GroupId = photoMove.GroupId,
                            PhotoId = fileId
                        });

                        if(photoAlbum != null && photoAlbum.TitlePhotoId == fileId)
                        {
                            photoAlbum.TitlePhotoId = 0;
                            await _photoAlbumService.Update(photoAlbum);
                        }
                    }

                    var sharedAlbums = await _relationSharedAlbumPhotoDao.SelectByPhotoId(fileId);
                    if (sharedAlbums != null && sharedAlbums.Any())
                        foreach(var sharedAlbum in sharedAlbums)
                        relationShared.Add(new RelationSharedAlbumPhotoModel
                        {
                            PhotoId = fileId,
                            SharedAlbumId = sharedAlbum.SharedAlbumId
                        });

                    var photo = await _photoService.Get(fileId);
                    var thumbnail = await _photoThumbnailService.Get(fileId);
                    var ownerList = new List<int> { photo.OwnerId };
                    photo.Personal = true;

                    await _photoTransaction.MoveToPersonal(fileId, thumbnail.Id, ownerList, relation, photo, relationShared);

                    Interlocked.Increment(ref job.Processed);
                }

                await LogInfo(ActionTypeEnum.Photo, $"Foto id {string.Join(",", photoMove.PhotoIds)} byly přesunuty z rodinné galerie do soukromých", photoMove.GroupId, 0, photoMove.UserId);
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
