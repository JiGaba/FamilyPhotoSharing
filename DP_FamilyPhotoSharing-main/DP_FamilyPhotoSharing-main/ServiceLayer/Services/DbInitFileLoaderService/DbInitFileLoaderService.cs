using DataAccessLayer.Transactions;
using FileAccessLayer.DbInit;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceLayer.Services.DbInitFileLoaderService
{
    public interface IDbInitFileLoaderService
    {
        Task<bool> CheckDbExistence();
        Task<bool> CheckTableExists();
        Task InitializeDatabase(UserModel user, UserKeysModel keys);
    }
    public class DbInitFileLoaderService : IDbInitFileLoaderService
    {
        private readonly IDbInitFileLoader _dbInitFileLoader;
        private readonly IDatabaseInitTransaction _databaseInitTransaction;
        private const string _TABLE_NAME = "Users";

        public DbInitFileLoaderService(IDbInitFileLoader dbInitFileLoader, IDatabaseInitTransaction databaseInitTransaction)
        {
            _dbInitFileLoader = dbInitFileLoader;
            _databaseInitTransaction = databaseInitTransaction;
        }

        public async Task<bool> CheckDbExistence()
            => await _databaseInitTransaction.CheckDbExistence();

        public async Task<bool> CheckTableExists()
            => await _databaseInitTransaction.CheckTableExists(_TABLE_NAME);

        public async Task InitializeDatabase(UserModel user, UserKeysModel keys)
        {
            var functions = await _dbInitFileLoader.GetDbInitFileByFolder(FoldersEnum.Functions);
            var tables = await _dbInitFileLoader.GetDbInitFileByFolder(FoldersEnum.Tables);
            var procedures = await _dbInitFileLoader.GetDbInitFileByFolder(FoldersEnum.Procedures);

            await _databaseInitTransaction.CreateEmptyDatabase();
            await _databaseInitTransaction.InitDatabase(functions, tables, procedures, user, keys);
        }
    }
}
