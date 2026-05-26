using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Enums
{
    public enum LogTypeEnum : int
    {
        [Description("Vše.")]
        All = 0,
        [Description("Informativní")]
        Ok = 1,
        [Description("Chyba")]
        Error = -1,
        [Description("API Informativní")]
        OkAPI = 2,
        [Description("API Chyba")]
        ErrorAPI = -2,
    }
}
