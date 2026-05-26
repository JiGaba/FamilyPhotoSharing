using DataAccessLayer.Dao.UserKeys;
using DataAccessLayer.Dto.UserKeys;
using Microsoft.Data.SqlClient;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.UserKeys
{
    public interface IUserKeysService
    {
        Task<int> SetUserKeys(SqlConnection connection, IDbTransaction transaction, UserKeysModel model);
        Task<UserKeysModel> SelectByUserId(int id);
    }
    public class UserKeysService : IUserKeysService
    {
        private readonly IUserKeysDao _userKeysDao;

        public UserKeysService(IUserKeysDao userKeysDao)
        {
            _userKeysDao = userKeysDao;
        }

        public async Task<UserKeysModel> SelectByUserId(int id) =>
            (await _userKeysDao.SelectByUserId(id))?.ToUserKeysModel();

        public async Task<int> SetUserKeys(SqlConnection connection, IDbTransaction transaction, UserKeysModel model) =>
            (await _userKeysDao.Tr_InsertReturnIdentity(connection, transaction, model?.ToUserKeysInsert()));
    }
}
