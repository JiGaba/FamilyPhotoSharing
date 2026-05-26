using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.SystemLog;
using EncryptionLayer.Photo;
using ModelLayer.Data;
using ModelLayer.Enums;
using ServiceLayer.BackgroundInterfaces;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.SharedPhotoAlbum;
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundProcessors
{
    public class PhotoDeleteProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoAlbumService _photoAlbumService;
        private readonly IDeleteFileService _fileService;
        private readonly IPhotoService _photoService;
        private readonly IRelationAlbumPhotoDao _relationAlbumPhotoDao;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        public PhotoDeleteProcessor(ICryptoService cryptoService, IPhotoAlbumService photoAlbumService, 
            IDeleteFileService fileService, ISystemLogService logService, IPhotoEncryptService photoEncryptService, 
            IUserService userService, IUserKeysService userKeysService, IPhotoService photoService,
            IRelationAlbumPhotoDao relationAlbumPhotoDao, IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao,
            ISharedPhotoAlbumService sharedPhotoAlbumService) 
            : base(photoEncryptService, userService, cryptoService, logService, userKeysService)
        {
            _photoAlbumService = photoAlbumService;
            _fileService = fileService;
            _photoService = photoService;
            _relationAlbumPhotoDao = relationAlbumPhotoDao;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
        }
        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var photoDelete = job.PhotoDeleteBGModel;

            if (photoDelete == null || !photoDelete.PhotoIds.Any())
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }

            try
            {
                job.Status = JobStatus.RUNNING;

                if (photoDelete.AlbumId != 0)
                {
                    var album = await _photoAlbumService.Get(photoDelete.AlbumId);

                    if (photoDelete.PhotoIds.Contains(album.TitlePhotoId))
                    {
                        album.TitlePhotoId = 0;
                        await _photoAlbumService.Update(album);
                    }
                }    

                foreach (var fileId in photoDelete.PhotoIds)
                {
                    // 1) vyjímečný případ, pokud maže vlastník svou fotku v rodinné galerii (pak jsou zobrazeny fotky mimo album)
                    var photo = await _photoService.Get(fileId);
                    
                    if (photoDelete.AlbumId == 0 && !photo.Personal)
                    {
                        // Relace se mažou automaticky v transakci, potřebuji zjisti zda není nastavena titulní ftotka v album/sdíleném albu
                        
                        var relationAlbumPhoto = (await _relationAlbumPhotoDao.SelectByPhotoId(photo.Id))?.ToRelationAlbumPhotoModel(); // získej vazbu fotky na album pokud existuje

                        if(relationAlbumPhoto != null)
                        {
                            var album = await _photoAlbumService.Get(relationAlbumPhoto.AlbumId);

                            if(album != null && album.TitlePhotoId == fileId)
                            {
                                album.TitlePhotoId = 0;
                                await _photoAlbumService.Update(album);
                            }
                        }    
                    }

                    // 2 vyjímečný případ, smazat titulní fotky ve sadílených albech pokud existují
                    var sharedAlbumRelation = (await _relationSharedAlbumPhotoDao.SelectByPhotoId(photo.Id))?
                    .Select(r => r?.ToRelationSharedAlbumPhotoModel())
                    .ToList();

                    if(sharedAlbumRelation != null && sharedAlbumRelation.Any())
                    {
                        foreach(var sharedAlbum in sharedAlbumRelation)
                        {
                            var album = await _sharedPhotoAlbumService.Get(sharedAlbum.SharedAlbumId);

                            if(album != null && album.TitlePhotoId == fileId)
                            {
                                album.TitlePhotoId = 0;
                                await _sharedPhotoAlbumService.Update(album);
                            }
                        }
                    }

                    // vyjímečný případ - konec

                    await _fileService.DeleteFile(fileId, photoDelete.UserId, photoDelete.GroupId, photoDelete.AlbumId, photoDelete.Folder);

                    Interlocked.Increment(ref job.Processed);
                }

                await LogInfo(ActionTypeEnum.RemovePhoto, $"Foto id {string.Join(",", photoDelete.PhotoIds)}", photoDelete.GroupId, 0, photoDelete.UserId);
                job.Status = JobStatus.COMPLETED;
            }
            catch(Exception e)
            {
                await LogError(ActionTypeEnum.RemovePhoto, e.Message, photoDelete.GroupId, 0, photoDelete.UserId);
                
                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }
}
