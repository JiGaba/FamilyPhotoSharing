using FamilyPhotoSharing.ViewsData;
using ModelLayer.Data;
using System.Runtime.CompilerServices;

namespace FamilyPhotoSharing.ApiResponse
{
    public class PhotoApiResponse
    {
        public int ThumbnailData { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public GalleryView Gallery {  get; set; }
    }

    public static partial class ResponseExtansion
    {
        public static PhotoApiResponse ToPhotoApiResponse(this GalleryView gallery, int thumbnailData, int page, int pageSize, int totalCount) => new PhotoApiResponse 
        { 
            ThumbnailData = thumbnailData, 
            Page = page, 
            PageSize = pageSize, 
            TotalCount = totalCount,
            Gallery = gallery
        };            

    }

}
