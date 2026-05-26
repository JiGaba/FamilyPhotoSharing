using DataAccessLayer.Dto.RelationAlbumPhoto;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationSharedAlbumPhoto
{
    public class RelationSharedAlbumPhotoDelete : RelationSharedAlbumPhotoBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationSharedAlbumPhotoDelete ToRelationSharedAlbumPhotoDelete(this RelationSharedAlbumPhotoModel rel) => new RelationSharedAlbumPhotoDelete
        {
            PhotoId = rel.PhotoId,
            SharedAlbumId = rel.SharedAlbumId
        };
    }
}
