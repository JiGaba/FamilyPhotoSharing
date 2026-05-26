using DataAccessLayer.Dao.PhotoEncrypt;
using DataAccessLayer.Dto.PhotoEncrypt;
using Microsoft.Data.SqlClient;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PhotoEncrypt
{
    public interface IPhotoEncryptService
    {
        Task<PhotoEncryptModel> GetByUserIdPhotoId(int userId, int fileId, FileTypeEnum fileType);
        Task<int> SetPhotoEncrypt(SqlConnection connection, IDbTransaction transaction, PhotoEncryptModel model);
    }
    public class PhotoEncryptService : IPhotoEncryptService
    {
        private readonly IPhotoEncryptDao _photoEncryptDao;

        public PhotoEncryptService(IPhotoEncryptDao photoEncryptDao)
        {
            _photoEncryptDao = photoEncryptDao;
        }

        public async Task<PhotoEncryptModel> GetByUserIdPhotoId(int userId, int fileId, FileTypeEnum fileType) =>
            (await _photoEncryptDao.SelectByUserIdPhotoId(userId, fileId, (short)fileType))?.ToPhotoEncryptModel();

        public async Task<int> SetPhotoEncrypt(SqlConnection connection, IDbTransaction transaction, PhotoEncryptModel model) =>
            (await _photoEncryptDao.Tr_InsertReturnIdentity(connection, transaction, model?.ToPhotoEncryptInsert()));
    }
}
