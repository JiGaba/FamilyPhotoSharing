using ModelLayer.PhotoEnums;

namespace FamilyPhotoSharing.Requests
{
    public abstract class BaseBSRequest
    {
        public List<int> PhotoIds { get; set; }
        public int AlbumId { get; set; }
        public ThumbnailData ThumbnailData { get; set; }
    }
}
