using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationAlbumPhoto
{
    public class RelationAlbumPhotoInsert : RalationAlbumPhotoBase
    {
        public int CreateAuthor {  get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationAlbumPhotoInsert ToRelationAlbumPhotoInsert(this RelationAlbumPhotoModel rel) => new RelationAlbumPhotoInsert
        {
            AlbumId = rel.AlbumId,
            GroupId = rel.GroupId,
            PhotoId = rel.PhotoId,
            CreateAuthor = rel.AuthorId
        };
    }
}
