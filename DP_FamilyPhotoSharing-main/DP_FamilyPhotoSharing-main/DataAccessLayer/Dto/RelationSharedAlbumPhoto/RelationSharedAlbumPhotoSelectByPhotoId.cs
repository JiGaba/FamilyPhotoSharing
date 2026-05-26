using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationSharedAlbumPhoto
{
    public class RelationSharedAlbumPhotoSelectByPhotoId : RelationSharedAlbumPhotoBase
    {
        public int Id { get; set; }
        public int CreateAuthorId { get; set; }
        public DateTime CteateDateTime { get; set; }
        public RelationSharedAlbumPhotoModel ToRelationSharedAlbumPhotoModel() => new RelationSharedAlbumPhotoModel
        {
            CreateAuthorId = CreateAuthorId,
            Id = Id,
            PhotoId = PhotoId,
            SharedAlbumId = SharedAlbumId,
            CreateDateTime = CteateDateTime,
        };
    }
}
