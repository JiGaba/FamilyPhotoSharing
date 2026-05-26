using DataAccessLayer.Dao.Groups;
using DataAccessLayer.Dto.Groups;
using ModelLayer.Data;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Groups
{
    public interface IGroupService : IGetList<UserGroupModel>, IGet<UserGroupModel>, 
        ISetValueReturnIdentity<UserGroupModel>, IUpdateValue<UserGroupModel>
    {
        Task DeleteGroupById(int id);
    }
    public class GroupService : IGroupService
    {
        private readonly IUserGroupDao _groupDao;
        public GroupService(IUserGroupDao groupDao)
            => _groupDao = groupDao;

        public async Task DeleteGroupById(int id)
            => await _groupDao.DeleteGroupById(id);

        public async Task<UserGroupModel> Get(int id)
            => (await _groupDao.SelectById(id))?.ToUserGroupModel();

        public async Task<List<UserGroupModel?>?> GetList(int id = 0)
            => (await _groupDao.SelectList())?.Select(g => g?.ToUserGroupModel()).ToList();

        public async Task<int> Set(UserGroupModel value)
            => await _groupDao.InsertReturnIdentity(value?.ToUserGroupsInsert());

        public async Task Update(UserGroupModel value)
            => await _groupDao.Update(value?.ToUserGroupsUpdate());  
    }
}
