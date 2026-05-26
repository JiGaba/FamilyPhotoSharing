namespace FamilyPhotoSharing.ApiRequests
{
    public class LoadPhotosRequest
    {
        public int Page { get; set; }
        public int Part { get; set; }
        public int ThumbnailData { get; set; }
        public int AlbumId { get; set; }
    }
}
