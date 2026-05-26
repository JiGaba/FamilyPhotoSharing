using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IInsertTransaction<T>
    {
        Task Tr_Insert(SqlConnection connection, IDbTransaction transaction, T data);
    }
    public interface IInsertReturnIdentityTransaction<T>
    {
        Task<int> Tr_InsertReturnIdentity(SqlConnection connection, IDbTransaction transaction, T data);
    }
    public interface IUpdateTransaction<T>
    {
        Task Tr_Update(SqlConnection connection, IDbTransaction transaction, T data);
    }
    public interface IUpdateRowAttachedTransaction<T>
    {
        Task<int> Tr_UpdateRowAttached(SqlConnection connection, IDbTransaction transaction, T data);
    }
    public interface IDeleteRowByIdTransaction
    {
        Task Tr_Delete(SqlConnection connection, IDbTransaction transaction, int id);
    }
}
