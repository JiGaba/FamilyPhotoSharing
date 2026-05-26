using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationAlbumPhoto
{
    public class RelationAlbumPhotoSelectByPhotoId : RalationAlbumPhotoBase
    {
        public int CreateAuthor { get; set; }
        public int Id { get; set; }
        public RelationAlbumPhotoModel ToRelationAlbumPhotoModel() => new RelationAlbumPhotoModel
        {
            AlbumId = AlbumId,
            AuthorId = CreateAuthor,
            GroupId = GroupId,
            Id = Id,
            PhotoId = PhotoId
        };
    }
}
