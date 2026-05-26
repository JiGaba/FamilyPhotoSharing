using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationUserSharedAlbum
{
    public class RelationUserSharedAlbumInsert : RelationUserSharedAlbumBase
    {
        public int CreateAuthor { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationUserSharedAlbumInsert ToRelationUserSharedAlbumInsert(this RelationUserSharedAlbumModel rel) => new RelationUserSharedAlbumInsert
        {
            SharedAlbumId = rel.SharedAlbumId,
            UserId = rel.UserId,
            CreateAuthor = rel.CreateAuthorId,
        };
    }
}
