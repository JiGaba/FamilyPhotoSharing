using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ISelect<T>
    {
        Task<T> SelectById(int id);
    }
    public interface ISelectList<T>
    {
        Task<List<T>> SelectList(int id = 0);
    }
    public interface IInsert<T>
    {
        Task Insert(T data);
    }
    public interface IInsertReturnIdentity<T>
    {
        Task<int> InsertReturnIdentity(T data);
    }
    public interface IUpdate<T>
    {
        Task Update(T data);
    }
    public interface IUpdateRowAttached<T>
    {
        Task<int> UpdateRowAttached(T data);
    }
    public interface IDeleteRowAttached<T>
    {
        Task<int> DeleteRowAttached(int id);
    }
}
