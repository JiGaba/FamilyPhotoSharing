using DataAccessLayer.Dto.PhotoThumbnail;
using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto.RelationAlbumPhoto
{
    public class RelationAlbumPhotoDelete : RalationAlbumPhotoBase { }
    public static partial class ProcedureMapperExtensions
    {
        public static RelationAlbumPhotoDelete ToRelationAlbumPhotoDelete(this RelationAlbumPhotoModel rel) => new RelationAlbumPhotoDelete
        {
            AlbumId = rel.AlbumId,
            GroupId = rel.GroupId,
            PhotoId = rel.PhotoId,
        };
    }
}
