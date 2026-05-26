using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationSharedAlbumPhoto
{
    public class RelationSharedAlbumPhotoInsert : RelationSharedAlbumPhotoBase
    {
        public int CreateAuthor { get; set; }
    }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationSharedAlbumPhotoInsert ToRelationSharedAlbumPhotoInsert(this RelationSharedAlbumPhotoModel rel) => new RelationSharedAlbumPhotoInsert
        {
            PhotoId = rel.PhotoId,
            SharedAlbumId = rel.SharedAlbumId,
            CreateAuthor = rel.CreateAuthorId,
        };
    }
}
