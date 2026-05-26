using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class SharedPhotoAlbumsModel : SharedPhotoAlbumModel
    {
        public string CreateAuthorName { get; set; }
        public int HostUserId { get; set; }
        public int HostUserCount { get; set; }
        public int PhotoCount { get; set; }
        public string HostUserIds { get; set; }
        public List<int>? HostUserIdsList 
        { 
            get { return (HostUserIds ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList();
            } 
        }
    }
}
