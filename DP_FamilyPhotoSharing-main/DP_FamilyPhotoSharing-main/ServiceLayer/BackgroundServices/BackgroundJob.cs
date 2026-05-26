using ModelLayer.BackgroundModels;
using ServiceLayer.BackgroundProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public class BackgroundJob
    {
        public Guid JobId { get; set; } = Guid.NewGuid();
        public JobTypeEnum JobType { get; set; }
        public PhotoDeleteBGModel PhotoDeleteBGModel { get; set; }
        public MoveToPersonalBGModel MoveToPersonalBGModel { get; set; }
        public MoveToGroupBGModel MoveToGroupBGModel { get; set; }
        public PhotoAlbumDeleteBGModel PhotoAlbumDeleteBGModel { get; set; }
        public AddUserToGroupBGModel AddUserToGroupBGModel { get; set; }
        public AddPhotoToSharedAlbumBGModel AddPhotoToSharedAlbumBGModel { get; set; }
        public RemoveFromSharedAlbumBGModel RemoveFromSharedAlbumBGModel{ get; set; }
        public SharedPhotoAlbumDeleteBGModel SharedPhotoAlbumDeleteBGModel{ get; set; }
        public ChangeUserSharedAlbumBGModel ChangeUserSharedAlbumBGModel { get; set; }
        public int Processed;
        public int Total { get; set; }
        public string Error { get; set; }
        public string Status { get; set; } = JobStatus.PENDING;
    }

}
