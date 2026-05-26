using DataAccessLayer.Dao.PhotoAlbum;
using DataAccessLayer.Dao.SharedPhotoAlbum;
using DataAccessLayer.Dto.PhotoAlbum;
using DataAccessLayer.Dto.SharedPhotoAlbum;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Transactions;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.SharedPhotoAlbum
{
    public interface ISharedPhotoAlbumService : IGet<SharedPhotoAlbumsModel>, ISetValueReturnIdentity<SharedPhotoAlbumModel>, IUpdateValue<SharedPhotoAlbumModel>
    {
        Task<List<SharedPhotoAlbumsModel>> GetByOwnerId(int id);
        Task<List<SharedPhotoAlbumsModel>> GetByHostId(int id);
    }
    public class SharedPhotoAlbumService : ISharedPhotoAlbumService
    {
        private readonly ISharedPhotoAlbumDao _sharedPhotoAlbumDao;
        public SharedPhotoAlbumService(ISharedPhotoAlbumDao sharedPhotoAlbumDao)
        {
            _sharedPhotoAlbumDao = sharedPhotoAlbumDao;
        }

        public async Task<SharedPhotoAlbumsModel> Get(int id)
            => (await _sharedPhotoAlbumDao.SelectById(id))?.ToSharedPhotoAlbumsModel();

        public async Task<List<SharedPhotoAlbumsModel>> GetByHostId(int id)
            => (await _sharedPhotoAlbumDao.SelectByHostUserId(id))?.Select(a => a?.ToSharedPhotoAlbumsModel()).ToList();

        public async Task<List<SharedPhotoAlbumsModel>> GetByOwnerId(int id)
            => (await _sharedPhotoAlbumDao.SelectByOwnerId(id))?.Select(a => a?.ToSharedPhotoAlbumsModel()).ToList();

        public async Task<int> Set(SharedPhotoAlbumModel value)
            => await _sharedPhotoAlbumDao.InsertReturnIdentity(value?.ToSharedPhotoAlbumInsert());

        public async Task Update(SharedPhotoAlbumModel value)
            => await _sharedPhotoAlbumDao.Update(value?.ToSharedPhotoAlbumUpdate());
    }
}
