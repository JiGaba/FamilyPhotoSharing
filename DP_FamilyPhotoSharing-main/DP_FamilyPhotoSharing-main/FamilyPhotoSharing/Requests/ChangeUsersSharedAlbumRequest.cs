namespace FamilyPhotoSharing.Requests
{
    public class ChangeUsersSharedAlbumRequest
    {
        public List<int> UserIds { get; set; }
        public int AlbumId { get; set; }
    }
}
