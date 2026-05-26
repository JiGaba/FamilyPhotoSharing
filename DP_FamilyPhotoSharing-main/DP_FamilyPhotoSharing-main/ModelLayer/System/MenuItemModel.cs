using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.System
{
    public class MenuItemModel
    {
        public string Title { get; set; } = string.Empty;
        public MenuPicture Picture {  get; set; }
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string RouteParamName { get; set; } = string.Empty;
        public int RouteParamValue { get; set; } = 0;

    }
}
