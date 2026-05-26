using DataAccessLayer.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public interface ITestService
    {
        Task<bool> TestDbConnection();
    }
    public class TestService : ITestService
    {
        private readonly ITestDao _testDao;
        public TestService(ITestDao testDao) 
        { 
            _testDao = testDao;
        }
        public Task<bool> TestDbConnection() =>
            _testDao.TestDbConnection();
    }
}
