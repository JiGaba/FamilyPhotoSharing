using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Dto.Photo;
using Microsoft.Data.SqlClient;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Photos
{
    public interface IPhotoService : IGet<PhotoModel> 
    {
        Task<List<PhotoModel>> SelectByOwnerAndPersonal(int ownerId, bool personal, int fetch, int offset);
        Task<List<PhotoModel>> SelectByAlbumId(int albumId, int fetch, int offset);
        Task<List<PhotoModel>> SelectBySharedAlbumId(int sharedAlbumId, int fetch, int offset);
        Task<List<PhotoModel>> SelectByGroupId(int groupId, int fetch, int offset);
        Task<int> GetPhotoCount(int ownerId, bool personal);
        Task<int> GetPhotoCountByAlbumId(int albumId);
        Task<int> GetPhotoCountByGroupId(int groupId);
        Task<int> GetPhotoCountByShardAlbumId(int sharedAlbumId);
    }
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoDao _photoDao;
        public PhotoService(IPhotoDao photoDao) => _photoDao = photoDao;

        public async Task<PhotoModel> Get(int id)
            => (await _photoDao.SelectById(id))?.ToPhotoModel();

        public async Task<int> GetPhotoCount(int ownerId, bool personal)
            => (await _photoDao.GetPhotoCount(ownerId, personal)).TotalCount;

        public async Task<int> GetPhotoCountByAlbumId(int albumId)
            => (await _photoDao.GetPhotoCountByAlbumId(albumId)).TotalCount;

        public async Task<int> GetPhotoCountByGroupId(int groupId)
            => (await _photoDao.GetPhotoCountByGroupId(groupId)).TotalCount;

        public async Task<int> GetPhotoCountByShardAlbumId(int sharedAlbumId)
            => (await _photoDao.GetPhotoCountByShardAlbumId(sharedAlbumId)).TotalCount;

        public async Task<List<PhotoModel>> SelectByAlbumId(int albumId, int fetch, int offset)
            => (await _photoDao.SelectByAlbumId(albumId, fetch, offset))?.Select(p => p?.ToPhotoModel()).ToList();

        public async Task<List<PhotoModel>> SelectByGroupId(int groupId, int fetch, int offset)
            => (await _photoDao.SelectByGroupId(groupId, fetch, offset))?.Select(p => p?.ToPhotoModel()).ToList();

        public async Task<List<PhotoModel>> SelectByOwnerAndPersonal(int ownerId, bool personal, int fetch, int offset)
            => (await _photoDao.SelectByOwnerAndPersonal(ownerId, personal, fetch, offset))?.Select(p => p?.ToPhotoModel()).ToList();

        public async Task<List<PhotoModel>> SelectBySharedAlbumId(int sharedAlbumId, int fetch, int offset)
            => (await _photoDao.SelectBySharedAlbumId(sharedAlbumId, fetch, offset))?.Select(p => p?.ToPhotoModel()).ToList();

    }
}
