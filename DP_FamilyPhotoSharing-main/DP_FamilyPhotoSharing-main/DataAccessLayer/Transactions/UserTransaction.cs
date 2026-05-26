using Dapper;
using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.SystemImages;
using DataAccessLayer.Dao.UserKeys;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.SystemImages;
using DataAccessLayer.Dto.UserKeys;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Transactions
{
    public interface IUserTransaction
    {
        Task<int> SetNewUser(UserModel userModel, UserKeysModel userKeysModel);
        Task<int> SetUser(UserModel userModel, SystemImagesModel imagesModel);
    }

    public class UserTransaction : DbWithLoggerAbstract, IUserTransaction
    {
        private readonly IInsertReturnIdentityTransaction<UserInsert> _insertUserDao;
        private readonly IUpdateRowAttachedTransaction<UserUpdate> _updateUserDao;
        private readonly IUserKeysDao _insertUserKeysDao;
        private readonly ISystemImageDao _systemImageDao;
        public UserTransaction(IInsertReturnIdentityTransaction<UserInsert> insertUserDao, 
            IUpdateRowAttachedTransaction<UserUpdate> updateUserDao, IUserKeysDao insertUserKeysDao, 
            IConfiguration configuration, ISystemLogDao logger, ISystemImageDao systemImageDao) : base(configuration, logger)
        { 
            _insertUserDao = insertUserDao;
            _updateUserDao = updateUserDao;
            _insertUserKeysDao = insertUserKeysDao;
            _systemImageDao = systemImageDao;
        }
        public async Task<int> SetNewUser(UserModel userModel, UserKeysModel userKeysModel)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var userId = await _insertUserDao.Tr_InsertReturnIdentity(connection, transaction, userModel?.ToUserInsert());

                            userKeysModel.UserId = userId;
                            var userKeysId = await _insertUserKeysDao.Tr_InsertReturnIdentity(connection, transaction, userKeysModel?.ToUserKeysInsert());
                            
                            await transaction.CommitAsync();

                            return userId;
                        }
                        catch(Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"UserTransaction - SetNewUser -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw e;
                        }
                    }
                }
            }
            catch
            {
                throw;
            } 
        }

        public async Task<int> SetUser(UserModel userModel, SystemImagesModel imagesModel)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var newImageId = 0;

                            if (imagesModel != null)
                            {
                                // Smazat původní image
                                await _systemImageDao.Tr_DeleteRowById(connection, transaction, userModel.SystemImagesId);
                                // Nahrát nový
                                newImageId = await _systemImageDao.Tr_InsertReturnIdentity(connection, transaction, imagesModel?.ToSystemImagesInsert());
                                userModel.SystemImagesId = newImageId;
                            }
                            
                            var userId = await _updateUserDao.Tr_UpdateRowAttached(connection, transaction, userModel.ToUserUpdate());

                            await transaction.CommitAsync();

                            return newImageId;
                        }
                        catch (Exception e)
                        {
                            await Log(LogTypeEnum.Error, ActionTypeEnum.Other, $"UserTransaction - SetUser -> {e.Message}", 0, 0);
                            await transaction.RollbackAsync();
                            throw e;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
