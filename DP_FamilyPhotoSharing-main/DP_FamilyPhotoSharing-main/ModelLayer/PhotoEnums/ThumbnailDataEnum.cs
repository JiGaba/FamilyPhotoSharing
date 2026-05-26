using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.PhotoEnums
{
    public enum ThumbnailData : int
    {
        [Description("Soukromá galerie")]
        MainPersonalGallery = 1,
        [Description("Rodinná galerie")]
        MainGroupGallery = 2,
        [Description("Soukromé album")]
        Album = 3,
        [Description("Rodinné album")]
        GroupAlbum = 4,
        [Description("Vlastní sdílené album")]
        MySharedAlbum = 5,
        [Description("Sdílené album")]
        SharedAlbum = 6,
        [Description("Rodinná galerie - vlastní foto")]
        MainGroupGalleryOwnPhoto = 7,
    }
}
