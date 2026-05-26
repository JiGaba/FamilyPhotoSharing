using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class MenuSectionModel : MenuItemModel
    {
        public List<MenuItemModel> SubItems { get; set; } = new();
    }
}
