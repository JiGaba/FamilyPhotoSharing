using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;

namespace FamilyPhotoSharing.ViewsData
{
    public class GalleryView
    {
        public string GalleryName {  get; set; }
        public string GalleryDescription { get; set; }
        public int TitlePotoId { get; set; }
        public ThumbnailData ThumbnailData { get; set; }
        public int AlbumId { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public List<PhotoModel> Photos {  get; set;  } = new List<PhotoModel>();
        public List<UserModalView> UsersModal { get; set; } = new List<UserModalView>();
        public List<SharedPhotoAlbumView> SharedModal { get; set; } = new List<SharedPhotoAlbumView>();
        public List<PhotoAlbumView> PersonalPhotoAlbumModal { get; set; } = new List<PhotoAlbumView>();
        public List<PhotoAlbumView> GroupPhotoAlbumModal { get; set; } = new List<PhotoAlbumView>();
    }
}
