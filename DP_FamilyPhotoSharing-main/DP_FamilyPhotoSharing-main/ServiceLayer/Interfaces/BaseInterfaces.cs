using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IGet<T>
    {
        Task<T> Get(int id);
    }

    public interface IGetList<T>
    {
        Task<List<T?>?> GetList(int id = 0);
    }

    public interface ISetValue<T>
    {
        Task Set(T value);
    }

    public interface ISetValueReturnIdentity<T>
    {
        Task<int> Set(T value);
    }

    public interface IUpdateValue<T>
    {
        Task Update(T value);
    }
}
