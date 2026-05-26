using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationUserSharedAlbum
{
    public class RelationUserSharedAlbumSelectByAlbumId : RelationUserSharedAlbumBase
    {
        public int Id { get; set; }
        public int CreateAuthorId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public RelationUserSharedAlbumModel ToRelationUserSharedAlbumModel() => new RelationUserSharedAlbumModel
        {
            Id = this.Id,
            CreateAuthorId = this.CreateAuthorId,
            CreateDateTime = this.CreateDateTime,
            SharedAlbumId = this.SharedAlbumId,
            UserId = this.UserId
        };
    }
}
