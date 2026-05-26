using ModelLayer.Data;

namespace FamilyPhotoSharing.ViewsData
{
    public class SharedPhotoAlbumView : SharedPhotoAlbumsModel 
    { 
        public List<UserModalView> UserModalViews { get; set; }
        public string SavedHostIds { get; set; } = string.Empty;
        public string ChangedHostIds { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public static partial class ViewExtension
    {
        public static SharedPhotoAlbumView ToSharedPhotoAlbumView(this SharedPhotoAlbumModel photoAlbumModel, List<UserModalView> userModalViews, string authorName, int hostUserId, int photoCount, int hostUserCount, bool selected = false) => new SharedPhotoAlbumView
        {
            AlbumDescription = photoAlbumModel.AlbumDescription,
            PhotoCount = photoCount,
            AlbumName = photoAlbumModel.AlbumName,
            CreateAuthor = photoAlbumModel.CreateAuthor,
            CreateDateTime = photoAlbumModel.CreateDateTime,
            TitlePhotoId = photoAlbumModel.TitlePhotoId,
            CreateAuthorName = authorName,
            Id = photoAlbumModel.Id,
            OwnerUserId = photoAlbumModel.OwnerUserId,
            UserGroupsId = photoAlbumModel.UserGroupsId,
            HostUserId = hostUserId,
            HostUserCount = hostUserCount,
            UserModalViews = userModalViews ?? new List<UserModalView>(),
            SavedHostIds = string.Join(",", (userModalViews ?? new List<UserModalView>()).Where(u => u.HasAccess).Select(u => u.Id)),
            ChangedHostIds = string.Join(",", (userModalViews ?? new List<UserModalView>()).Where(u => u.HasAccess).Select(u => u.Id)),
            Selected = selected,
        };

        public static SharedPhotoAlbumView ToSharedPhotoAlbumView(this SharedPhotoAlbumsModel photoAlbumModel, List<UserModalView> userModalViews, bool selected = false) => new SharedPhotoAlbumView
        {
            AlbumDescription = photoAlbumModel.AlbumDescription,
            PhotoCount = photoAlbumModel.PhotoCount,
            AlbumName = photoAlbumModel.AlbumName,
            CreateAuthor = photoAlbumModel.CreateAuthor,
            CreateDateTime = photoAlbumModel.CreateDateTime,
            TitlePhotoId = photoAlbumModel.TitlePhotoId,
            CreateAuthorName = photoAlbumModel.CreateAuthorName,
            Id = photoAlbumModel.Id,
            OwnerUserId = photoAlbumModel.OwnerUserId,
            UserGroupsId = photoAlbumModel.UserGroupsId,
            HostUserId = photoAlbumModel.HostUserId,
            HostUserCount = photoAlbumModel.HostUserCount,
            UserModalViews = userModalViews ?? new List<UserModalView>(),
            SavedHostIds = string.Join(",", (userModalViews ?? new List<UserModalView>()).Where(u => u.HasAccess).Select(u => u.Id)),
            ChangedHostIds = string.Join(",", (userModalViews ?? new List<UserModalView>()).Where(u => u.HasAccess).Select(u => u.Id)),
            Selected = selected,
        };
    }
}
