using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Transactions;
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
using ServiceLayer.Services.UserKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ServiceLayer.BackgroundProcessors
{
    public class PhotoAlbumDeleteProcessor : BaseProcessor, IJobProcessor
    {
        private readonly IPhotoAlbumService _photoAlbumService;
        private readonly IPhotoService _photoService;
        private readonly IDeleteFileService _deleteFileService;
        private readonly IPhotoAlbumTransaction _photoAlbumTransaction;
        public PhotoAlbumDeleteProcessor(ICryptoService cryptoService, ISystemLogService systemLogService, 
            IPhotoAlbumService photoAlbumService, IPhotoService photoService, 
            IDeleteFileService deleteFileService, IPhotoAlbumTransaction photoAlbumTransaction,
            IPhotoEncryptService photoEncryptService, IUserService userService, IUserKeysService userKeysService) 
            : base(photoEncryptService, userService, cryptoService, systemLogService, userKeysService)
        {
            _photoAlbumService = photoAlbumService;
            _photoService = photoService;
            _deleteFileService = deleteFileService;
            _photoAlbumTransaction = photoAlbumTransaction;
        }

        public async Task ProcessAsync(BackgroundJob job, CancellationToken token)
        {
            var albumDelete = job.PhotoAlbumDeleteBGModel;

            if (albumDelete == null)
            {
                job.Processed = job.Total;
                job.Status = JobStatus.COMPLETED;
                return;
            }
            
            try
            {
                job.Status = JobStatus.RUNNING;

                var album = await _photoAlbumService.Get(albumDelete.AlbumId);
                var photoCount = await _photoService.GetPhotoCountByAlbumId(albumDelete.AlbumId);
                var photoList = await _photoService.SelectByAlbumId(albumDelete.AlbumId, photoCount, 0);

                if ((album.Personal && album.OwnerUserId != albumDelete.UserId) || 
                    (!album.Personal && (albumDelete.UserRoleEnum.Equals(UserRoleEnum.Admin) || albumDelete.UserRoleEnum.Equals(UserRoleEnum.GroupAdmin)))) // může smazat jen vlastník alba!
                {
                    job.Processed = job.Total;
                    job.Status = JobStatus.COMPLETED;
                    return;
                }

                job.Total = photoList.Count;

                if (album.Personal) // Smazat všechny fotky a případně jejich relace ve sdílené galerie
                {
                    foreach (var photo in photoList)
                    {
                        await _deleteFileService.DeleteFile(photo.Id, albumDelete.UserId, albumDelete.GroupId, album.Id, albumDelete.Folder);
                        
                        Interlocked.Increment(ref job.Processed);
                    }

                    await _photoAlbumTransaction.DeletePhotoAlbum(null, album.Id); 
                }
                else // Smazat jen album a relace na album
                {
                    var relation = new List<RelationAlbumPhotoModel>();
                    foreach(var photo in photoList)
                    {
                        relation.Add(new RelationAlbumPhotoModel
                        {
                            AlbumId = album.Id,
                            AuthorId = 0,
                            GroupId = albumDelete.GroupId,
                            PhotoId = photo.Id,
                        });

                        Interlocked.Increment(ref job.Processed);
                    }
                    
                    await _photoAlbumTransaction.DeletePhotoAlbum(relation, album.Id);

                    job.Processed = job.Total;
                }

                await LogInfo(ActionTypeEnum.RemovePhotoAlbum, $"Smazání alba {album.AlbumName} [{album.Id}].", albumDelete.GroupId, (int)albumDelete.UserRoleEnum, albumDelete.UserId);

                job.Status = JobStatus.COMPLETED;
            }
            catch (Exception e)
            {
                await LogError(ActionTypeEnum.RemovePhotoAlbum, e.Message, albumDelete.GroupId, (int)albumDelete.UserRoleEnum, albumDelete.UserId);

                job.Processed = job.Total;
                job.Status = JobStatus.FAILED;
                job.Error = e.Message;
            }
        }
    }
}
