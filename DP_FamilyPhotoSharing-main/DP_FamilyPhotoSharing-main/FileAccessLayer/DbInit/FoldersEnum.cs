using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAccessLayer.DbInit
{
    public enum FoldersEnum
    {
        [Description("SQLFunctions")]
        Functions = 1,
        [Description("SQLProcedures")]
        Procedures = 2,
        [Description("SQLTables")]
        Tables = 3
    }
}
