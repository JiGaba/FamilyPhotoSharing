namespace FamilyPhotoSharing.Requests
{
    public class MoveRequest
    {
        public bool Personal { get; set; }
        public int FromAlbumId { get; set; }
        public int ToAlbumId { get; set; }
        public List<int> PhotoIds { get; set; }
    }
}
