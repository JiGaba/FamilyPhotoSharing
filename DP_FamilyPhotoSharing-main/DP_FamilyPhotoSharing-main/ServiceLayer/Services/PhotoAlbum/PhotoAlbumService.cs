using DataAccessLayer.Dao.PhotoAlbum;
using DataAccessLayer.Dto.PhotoAlbum;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PhotoAlbum
{
    public interface IPhotoAlbumService : IGet<PhotoAlbumModel>, ISetValueReturnIdentity<PhotoAlbumModel>, IUpdateValue<PhotoAlbumModel>
    {
        Task<List<PhotoAlbumsModel>> GetByOwnerId(int id);
        Task<List<PhotoAlbumsModel>> GetByGroupsId(int id);
    }
    public class PhotoAlbumService : IPhotoAlbumService
    {
        private readonly IPhotoAlbumDao _photoAlbumDao;

        public PhotoAlbumService(IPhotoAlbumDao photoAlbumDao)
            => _photoAlbumDao = photoAlbumDao;

        public async Task<PhotoAlbumModel> Get(int id)
            => (await _photoAlbumDao.SelectById(id))?.ToPhotoAlbumModel();

        public async Task<List<PhotoAlbumsModel>> GetByGroupsId(int id)
            => (await _photoAlbumDao.SelectByGroupsId(id))?.Select(p => p?.ToPhotoAlbumsModel()).ToList();

        public async Task<List<PhotoAlbumsModel>> GetByOwnerId(int id)
            => (await _photoAlbumDao.SelectByOwnerId(id))?.Select(p => p?.ToPhotoAlbumsModel()).ToList();

        public async Task<int> Set(PhotoAlbumModel value)
            => await _photoAlbumDao.InsertReturnIdentity(value?.ToPhotoAlbumInsert());

        public async Task Update(PhotoAlbumModel value)
            => await _photoAlbumDao.Update(value?.ToPhotoAlbumUpdate());
    }
}
