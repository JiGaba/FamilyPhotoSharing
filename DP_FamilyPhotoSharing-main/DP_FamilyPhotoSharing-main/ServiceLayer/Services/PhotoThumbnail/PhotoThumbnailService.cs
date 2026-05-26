using DataAccessLayer.Dao.PhotoThumbnailDao;
using DataAccessLayer.Dto.PhotoThumbnail;
using Microsoft.Data.SqlClient;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PhotoThumbnail
{
    public interface IPhotoThumbnailService
    {
        Task<int> SetPhotoThumbnail(SqlConnection connection, IDbTransaction transaction, PhotoThumbnailModel model);
        Task<PhotoThumbnailModel> Get(int id);
    }
    public class PhotoThumbnailService : IPhotoThumbnailService
    {
        private readonly IPhotoThumbnailDao _photoThumbnailDao;

        public PhotoThumbnailService(IPhotoThumbnailDao photoThumbnailDao)
        {
            _photoThumbnailDao = photoThumbnailDao;
        }

        public async Task<PhotoThumbnailModel> Get(int id)
            => (await _photoThumbnailDao.SelectByPhotoId(id))?.ToPhotoThumbnailModel();

        public async Task<int> SetPhotoThumbnail(SqlConnection connection, IDbTransaction transaction, PhotoThumbnailModel model) =>
            (await _photoThumbnailDao.Tr_InsertReturnIdentity(connection, transaction, model?.ToPhotoThumbnailInsert()));
    }
}
