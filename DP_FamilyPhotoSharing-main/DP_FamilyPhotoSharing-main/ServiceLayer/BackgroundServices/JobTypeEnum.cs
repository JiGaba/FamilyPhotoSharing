using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public enum JobTypeEnum : int
    {
        PhotoDelete, 
        MoveToPersonal, 
        MoveToGroup,
        AddUserToGroup,
        ChangeUserSharedAlbum,
        AddPhotoToSharedAlbum, 
        RemoveFromSharedAlbum,
        PhotoAlbumDelete,
        SharedPhotoAlbumDelete
    }
}
