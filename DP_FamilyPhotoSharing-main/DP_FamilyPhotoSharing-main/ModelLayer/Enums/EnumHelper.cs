using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Enums
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                    return attributes[0].Description;
            }

            return value.ToString();
        }

        public static T GetEnum<T>(int value) where T : Enum
        {
            if(Enum.IsDefined(typeof(T), value))
                return (T)(object)value;

            throw new ArgumentException($"Hodnota {value} není platná pro enum typu {typeof(T).Name}.");
        }

        public static T GetEnum<T>(Int16 value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)(object)value;

            throw new ArgumentException($"Hodnota {value} není platná pro enum typu {typeof(T).Name}.");
        }

        public static List<int> ToRoleIdList(this List<UserRoleEnum> userRoleEnums)
        {
            var list = new List<int>();

            userRoleEnums.ForEach(r => { list.Add((int)r); });

            return list;
        }
    }
}
