using DataAccessLayer.Dto.RelationSharedAlbumPhoto;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationUserSharedAlbum
{
    public class RelationSharedAlbumDelete : RelationUserSharedAlbumBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationSharedAlbumDelete ToRelationSharedAlbumDelete(this RelationUserSharedAlbumModel rel) => new RelationSharedAlbumDelete
        {
            SharedAlbumId = rel.SharedAlbumId,
            UserId = rel.UserId,
        };
    }
}
