using DataAccessLayer.Dao.RelationAlbumPhoto;
using DataAccessLayer.Dao.RelationSharedAlbumPhoto;
using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using DataAccessLayer.Transactions;
using FileAccessLayer;
using ModelLayer.Data;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.PhotoEncrypt;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.PhotoThumbnail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Files
{
    public interface IDeleteFileService
    {
        Task DeleteFile(int fileId, int userId, int groupId, int albumId, string folder);
    }
    public class DeleteFileService : IDeleteFileService
    {
        private readonly IDeleteFile _fileService;
        private readonly IPhotoService _photoService;
        private readonly IPhotoThumbnailService _photoThumbnailService;
        private readonly IPhotoTransaction _photoTransaction;
        private readonly IRelationSharedAlbumPhotoDao _relationSharedAlbumPhotoDao;
        private readonly IRelationAlbumPhotoDao _relationAlbumPhotoDao;

        public DeleteFileService(IDeleteFile fileService, IPhotoService photoService,
            IPhotoThumbnailService photoThumbnailService, IPhotoTransaction photoTransaction,
            IRelationSharedAlbumPhotoDao relationSharedAlbumPhotoDao, IRelationAlbumPhotoDao relationAlbumPhotoDao)
        {
            _fileService = fileService;
            _photoService = photoService;
            _photoThumbnailService = photoThumbnailService;
            _photoTransaction = photoTransaction;
            _relationSharedAlbumPhotoDao = relationSharedAlbumPhotoDao;
            _relationAlbumPhotoDao = relationAlbumPhotoDao;
        }

        public async Task DeleteFile(int fileId, int userId, int groupId, int albumId, string folder)
        {
            RelationAlbumPhotoModel relationAlbumPhoto = null;
            try
            {
                var photo = await _photoService.Get(fileId);
                var thumbnail = await _photoThumbnailService.Get(fileId);

                if (albumId != 0)
                    relationAlbumPhoto = new RelationAlbumPhotoModel
                    {
                        AlbumId = albumId,
                        AuthorId = userId,
                        GroupId = groupId,
                        PhotoId = fileId
                    };
                else if(albumId == 0 && !photo.Personal)// vyjímečný případ, pokud maže vlastník svou fotku v rodinné galerii (pak jsou zobrazeny fotky mimo album)
                    relationAlbumPhoto = (await _relationAlbumPhotoDao.SelectByPhotoId(photo.Id))?.ToRelationAlbumPhotoModel(); // získej vazbu fotky na album pokud existuje

                var sharedAlbumRelation = (await _relationSharedAlbumPhotoDao.SelectByPhotoId(photo.Id))?
                    .Select(r => r?.ToRelationSharedAlbumPhotoModel())
                    .ToList();

                // odstranit titulní foto album/sdílené album

                await _photoTransaction.Delete(fileId, relationAlbumPhoto, sharedAlbumRelation);

                var photoTask = _fileService.Delete(photo.FSFileName, folder);
                var thumbnailTask = _fileService.Delete(thumbnail.FSThumbnailName, folder);

                await Task.WhenAll(photoTask, thumbnailTask);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
