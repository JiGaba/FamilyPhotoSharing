using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.BackgroundModels
{
    public class AddPhotoToSharedAlbumBGModel
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int SharedAlbumId { get; set; }
        public UserRoleEnum UserRoleEnum { get; set; }
        public List<int> PhotoIds { get; set; }
    }
}
