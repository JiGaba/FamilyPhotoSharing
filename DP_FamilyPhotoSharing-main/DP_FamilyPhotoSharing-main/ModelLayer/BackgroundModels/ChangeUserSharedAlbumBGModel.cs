using ModelLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.BackgroundModels
{
    public class ChangeUserSharedAlbumBGModel
    {
        public int AlbumId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public UserRoleEnum UserRoleEnum { get; set; }
        public List<int> UserIdRemove { get; set; }
        public List<int> UserIdAdd { get; set; }
    }
}
