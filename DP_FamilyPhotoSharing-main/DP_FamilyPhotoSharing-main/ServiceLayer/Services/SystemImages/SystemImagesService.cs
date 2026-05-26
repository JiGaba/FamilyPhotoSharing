using DataAccessLayer.Dao.Photo;
using DataAccessLayer.Dao.SystemImages;
using DataAccessLayer.Dto.SystemImages;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.SystemImages
{
    public interface ISystemImagesService : IGet<SystemImagesModel>, ISetValue<SystemImagesModel> { }
    public class SystemImagesService : ISystemImagesService
    {
        private readonly ISystemImageDao _systemImageDao;

        public SystemImagesService(ISystemImageDao systemImageDao)
        {
            _systemImageDao = systemImageDao;
        }

        public async Task<SystemImagesModel> Get(int id)
            => (await _systemImageDao.SelectById(id))?.ToSystemImagesModel();

        public async Task Set(SystemImagesModel value)
            => await _systemImageDao.Insert(value?.ToSystemImagesInsert());
    }
}
