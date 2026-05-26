using DataAccessLayer.Dao.Accounts;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Transactions;
using EncryptionLayer.Photo;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Accounts
{
    public interface IUserService : IGet<UserModel?>, IGetList<UserModel>
    {
        Task<UserModel?> GetUserByLogin(string login);
        Task<int> SetNewUser(UserModel userModel);
        Task SetUser(UserModel user);
        Task<List<UserModel?>> SelectUserByGroupIdActiveRoleId(int groupId, bool active, List<UserRoleEnum> roleId, bool activated = true);
        Task<List<UserCountModel>> GetUserCountByGroupId(int groupId);
        Task UpdatePassword(UserPasswordModel data);
        Task UpdateBackupKey(UserBackupKeyModel data);
        UserKeysModel PrepareUserKeysModel(UserModel userModel);
    }
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly ICryptoService _cryptoService;
        private readonly IUserTransaction _userTransaction;
        public UserService(IUserDao userDao, IUserTransaction userTransaction, ICryptoService cryptoService)
        {
            _userDao = userDao;
            _userTransaction = userTransaction;
            _cryptoService = cryptoService;
        }

        public async Task<UserModel?> Get(int id) =>
            (await _userDao.SelectById(id))?.ToUserModel();

        public async Task<List<UserModel?>?> GetList(int id = 0) =>
            (await _userDao.SelectList(id))?.Select(u => u?.ToUserModel())?.ToList();

        public async Task<UserModel?> GetUserByLogin(string login) =>
            (await _userDao.SelectByLogin(login))?.ToUserModel();

        public async Task<List<UserCountModel>> GetUserCountByGroupId(int groupId)
        {
            var result = (await _userDao.SelectUserCountByGroupId(groupId))?.Select(u => u?.ToUserCountModel()).ToList();

            if (result == null || result.Count == 0) return new List<UserCountModel>
            {
                new UserCountModel { Active = true, UserCount = 0 },
                new UserCountModel { Active = false, UserCount = 0 },
            };

            if (result.Count == 1)
            {
                result.Add(new UserCountModel { Active = !result.ElementAt(0).Active, UserCount = 0 });
                return result;
            }

            if (result.Count == 2)
                return result;

            throw new Exception("GetUserCountByGroupId obsahuje neplatný počet položek.");
        }

        public async Task<List<UserModel?>> SelectUserByGroupIdActiveRoleId(int groupId, bool active, List<UserRoleEnum> roleId, bool activated = true) =>
            (await _userDao.SelectUserByGroupIdActiveRoleId(groupId, active, roleId?.ToRoleIdList(), activated))?.Select(u => u.ToUserModel()).ToList();

        public async Task<int> SetNewUser(UserModel userModel)
        {
            var (privatePem, publicPem) = _cryptoService.GetNewRSAPemEncrypted();

            var userKeys = new UserKeysModel
            {
                CreateAuthorId = userModel.CreateAuthor,
                CreateDateTime = userModel.CreateDateTime,
                Id = 0,
                PrivateKeyNonce = privatePem.Nonce,
                PrivateKeyTag = privatePem.Tag,
                RSAPrivateKey = privatePem.EncrptData,
                PublicKeyNonce = publicPem.Nonce,
                PublicKeyTag = publicPem.Tag,
                RSAPublicKey = publicPem.EncrptData,
                UserId = 0 // doplní se v transakci
            };

            // Přidat šifrování
            return await _userTransaction.SetNewUser(userModel, userKeys);
        }  
        
        public UserKeysModel PrepareUserKeysModel(UserModel userModel)
        {
            var (privatePem, publicPem) = _cryptoService.GetNewRSAPemEncrypted();

            return new UserKeysModel
            {
                CreateAuthorId = userModel.CreateAuthor,
                CreateDateTime = userModel.CreateDateTime,
                Id = 0,
                PrivateKeyNonce = privatePem.Nonce,
                PrivateKeyTag = privatePem.Tag,
                RSAPrivateKey = privatePem.EncrptData,
                PublicKeyNonce = publicPem.Nonce,
                PublicKeyTag = publicPem.Tag,
                RSAPublicKey = publicPem.EncrptData,
                UserId = 0 // doplní se v transakci
            };
        }

        public async Task SetUser(UserModel user) =>
            await _userTransaction.SetUser(user, null);

        public async Task UpdateBackupKey(UserBackupKeyModel data)
            => await _userDao.UpdateBackupKey(data?.ToUserUpdateBackupKey());

        public async Task UpdatePassword(UserPasswordModel data)
            => await _userDao.UpdatePassword(data?.ToUserUpdatePassword());
    }
}
