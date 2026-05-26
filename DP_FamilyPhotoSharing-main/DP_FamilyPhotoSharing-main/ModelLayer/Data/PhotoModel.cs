using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Data
{
    public class PhotoModel
    {
        public int Id { get; set; }
        public string PhotoName { get; set; } = string.Empty;
        public string PhotoDescription { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public int GroupsId { get; set; }
        public int FileSize { get; set; }
        public string FSFileName { get; set; } = string.Empty;
        public bool Personal { get; set; }
        public int CreateAuthor { get; set; }
        public DateTime CreateDateTime {  get; set; }
        public byte[] Data { get; init; }
    }
}
