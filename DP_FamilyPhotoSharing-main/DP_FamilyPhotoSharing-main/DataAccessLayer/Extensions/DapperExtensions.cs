using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Extensions
{
    public static class DapperExtensions
    {
        public static DynamicParameters ToDynamicParameters<T>(this T obj)
        {
            var parameters = new DynamicParameters();
            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(obj);
                parameters.Add("@" + prop.Name, value);
            }
            return parameters;
        }
    }

}
