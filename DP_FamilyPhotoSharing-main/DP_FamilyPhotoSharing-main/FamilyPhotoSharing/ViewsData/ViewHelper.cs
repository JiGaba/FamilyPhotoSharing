using ModelLayer.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyPhotoSharing.ViewsData
{
    public class ViewHelper
    {
        public static List<SelectListItem> ToSelectList<TEnum>(int lastId) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(x => new SelectListItem
                {
                    Value = Convert.ToInt32(x).ToString(),
                    Text = (x as Enum).GetDescription(),
                    Selected = (Convert.ToInt32(x) == lastId)
                })
                .ToList();
        }
    }
}
