using ModelLayer.Data;

namespace FamilyPhotoSharing.ViewsData
{
    public class PhotoAlbumView : PhotoAlbumModel
    {
        public int PhotoCount { get; set; }
        public string IAuthorName { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public static partial class ViewExtension
    {
        public static PhotoAlbumView ToPhotoAlbumView(this PhotoAlbumModel photoAlbumModel, string authorName, int photoCount, bool selected = false) => new PhotoAlbumView
        {
            AlbumDescription = photoAlbumModel.AlbumDescription,
            PhotoCount = photoCount,
            AlbumName = photoAlbumModel.AlbumName,
            CreateAuthor = photoAlbumModel.CreateAuthor,
            CreateDateTime = photoAlbumModel.CreateDateTime,
            TitlePhotoId = photoAlbumModel.TitlePhotoId,
            IAuthorName = authorName,
            Id = photoAlbumModel.Id,
            OwnerUserId = photoAlbumModel.OwnerUserId,
            Personal = photoAlbumModel.Personal,
            UserGroupsId = photoAlbumModel.UserGroupsId,
            Selected = selected,
        };

        public static PhotoAlbumView ToPhotoAlbumView(this PhotoAlbumsModel photoAlbumModel, bool selected = false) => new PhotoAlbumView
        {
            AlbumDescription = photoAlbumModel.AlbumDescription,
            PhotoCount = photoAlbumModel.PhotoCount,
            AlbumName = photoAlbumModel.AlbumName,
            CreateAuthor = photoAlbumModel.CreateAuthor,
            CreateDateTime = photoAlbumModel.CreateDateTime,
            TitlePhotoId = photoAlbumModel.TitlePhotoId,
            IAuthorName = photoAlbumModel.CreateAuthorName,
            Id = photoAlbumModel.Id,
            OwnerUserId = photoAlbumModel.OwnerUserId,
            Personal = photoAlbumModel.Personal,
            UserGroupsId = photoAlbumModel.UserGroupsId,
            Selected = selected,
        };
    }
}
