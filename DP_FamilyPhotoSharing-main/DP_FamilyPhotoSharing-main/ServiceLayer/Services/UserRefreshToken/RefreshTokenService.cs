using DataAccessLayer.Dao.UserKeys;
using DataAccessLayer.Dao.UserRefreshToken;
using DataAccessLayer.Dto.UserRefreshToken;
using DataAccessLayer.Interfaces;
using ModelLayer.Data;
using ModelLayer.System;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.UserRefreshToken
{
    public interface IRefreshTokenService : ISetValue<RefreshTokenModel>, IUpdateValue<RefreshTokenModel>
    {
        Task<RefreshTokenModel> SelectByToken(string token);
    }
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUserRefreshTokenDao _tokenDao; 
        public RefreshTokenService(IUserRefreshTokenDao tokenDao)
        {
            _tokenDao = tokenDao;
        }

        public async Task<RefreshTokenModel> SelectByToken(string token)
            => (await _tokenDao.SelectByToken(token))?.ToRefreshTokenModel();

        public async Task Set(RefreshTokenModel value)
            => await _tokenDao.InsertReturnIdentity(value?.ToUserRefreshTokenInsert());
            
        public async Task Update(RefreshTokenModel value)
            => await _tokenDao.Update(value?.ToUserRefreshTokenUpdate());
    }
}
