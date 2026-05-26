using Dapper;
using DataAccessLayer.Dao;
using DataAccessLayer.Dao.Accounts;
using DataAccessLayer.Dao.Logs;
using DataAccessLayer.Dao.UserKeys;
using DataAccessLayer.Dto.Accounts;
using DataAccessLayer.Dto.UserKeys;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Transactions
{
    public interface IDatabaseInitTransaction
    {
        Task<bool> CheckDbExistence();
        Task<bool> CheckTableExists(string tableName);
        Task CreateEmptyDatabase();
        Task InitDatabase(List<string> functions, List<string> tables, List<string> procedures, UserModel user, UserKeysModel keys);
    }
    public class DatabaseInitTransaction : DbWithLoggerAbstract, IDatabaseInitTransaction
    {
        private const string DB_NAME = "FamilyPhotoSharing";
        private const string DB_EXISTS = @"SELECT database_id FROM sys.databases WHERE name = @dbName";
        private static string DB_CREATE => $@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName)
                                           BEGIN
                                                CREATE DATABASE [{DB_NAME}];
                                           END";
        private const string DB_TABLE_EXISTS = @"SELECT COUNT(*) FROM sys.tables WHERE name = @tableName";
        private readonly IUserDao _userDao;
        private readonly IUserKeysDao _userKeysDao;
        public DatabaseInitTransaction(IConfiguration configuration, ISystemLogDao logger, IUserKeysDao userKeysDao, 
            IUserDao userDao) : base(configuration, logger)
        {
            _userDao = userDao;
            _userKeysDao = userKeysDao;
        }

        public async Task<bool> CheckDbExistence()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionMasterString()))
                {
                    await connection.OpenAsync();

                    var exists = await connection.ExecuteScalarAsync<int?>(DB_EXISTS, new { dbName = DB_NAME });

                    return exists.HasValue;   
                }
            }
            catch (Exception e)
            {
                throw new Exception("Inicializaci databáze nebylo možné provést, pokud problém přetrvá kontaktujte administrátora systému.");
            }
        }

        public async Task<bool> CheckTableExists(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    var exists = await connection.ExecuteScalarAsync<int?>(DB_TABLE_EXISTS, new { tableName = tableName });

                    return exists >= 1;
                }
            }
            catch
            {
                throw new Exception($"Inicializaci databáze nebylo možné provést, tabulka {tableName} nebyla nalezena. pokud problém přetrvá kontaktujte administrátora systému.");
            }
        }

        public async Task CreateEmptyDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionMasterString()))
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(DB_CREATE, new { dbName = DB_NAME });
                }
            }
            catch
            {
                throw new Exception("Inicializaci databáze nebylo možné provést, pokud problém přetrvá kontaktujte administrátora systému.");
            }
        }

        public async Task InitDatabase(List<string> functions, List<string> tables, List<string> procedures, UserModel user, UserKeysModel keys)
        {
            var i = 0;
            try
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            foreach (var elem in functions)
                                foreach (var batch in SplitByGo(elem))
                                    await connection.ExecuteAsync(batch, transaction: transaction);

                            foreach (var elem in tables)
                                foreach (var batch in SplitByGo(elem))
                                    await connection.ExecuteAsync(batch, transaction: transaction);

                           
                            foreach (var elem in procedures)
                                foreach (var batch in SplitByGo(elem))
                                    await connection.ExecuteAsync(batch, transaction: transaction);

                            var userId = await _userDao.Tr_InsertReturnIdentity(connection, transaction, user?.ToUserInsert());

                            keys.UserId = userId;
                            await _userKeysDao.Tr_InsertReturnIdentity(connection, transaction, keys?.ToUserKeysInsert());
                                
                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                new Exception("Inicializaci databáze nebylo možné provést, pokud problém přetrvá kontaktujte administrátora systému.");
            }
        }

        private List<string> SplitByGo(string script)
        {
            return Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
        }
    }
}
