using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Structs
{
    public struct PhotoEncryptDeleteStruct
    {
        public FileTypeEnum FileTypeEnum { get; set; }
        public int FileId { get; set; }
        public List<int> NotDeleteUsers { get; set; }
    }
}
