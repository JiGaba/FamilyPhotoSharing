using DataAccessLayer.Transactions;
using ModelLayer.Data;
using ServiceLayer.Services.PhotoAlbum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Gallery
{
    public interface IGalleryService
    {
        Task MovePersonalOrGroup(List<int> photoIdList, int fromAlbumId, int toAlbumId, int groupId, int userId, bool personal);
    }
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryTransaction _galleryTransaction;
        private readonly IPhotoAlbumService _photoAlbumService;
        public GalleryService(IGalleryTransaction galleryTransaction, IPhotoAlbumService photoAlbumService)
        {
            _galleryTransaction = galleryTransaction;
            _photoAlbumService = photoAlbumService;
        }

        public async Task MovePersonalOrGroup(List<int> photoIdList, int fromAlbumId, int toAlbumId, int groupId, int userId, bool personal)
        {
            if (fromAlbumId == toAlbumId)
                throw new Exception("Nelze přesunout fotografie pokud se ID zdrojového a cílového alba shodují.");  
            
            var insertRelace = new List<RelationAlbumPhotoModel>();
            var deleteRelace = new List<RelationAlbumPhotoModel>();
            PhotoAlbumModel photoAlbum = null;

            if (toAlbumId == 0) // do hlavní galerie, budu mazat relaci
            {
                var albumFrom = await _photoAlbumService.Get(fromAlbumId);
                photoAlbum = await AlbumUpdate(photoIdList, albumFrom);

                await PersonalTest(personal, albumFrom.Personal);

                foreach(var photoId in photoIdList)
                {
                    deleteRelace.Add(new RelationAlbumPhotoModel 
                    {
                        PhotoId = photoId,
                        AlbumId = fromAlbumId,
                        GroupId = groupId,
                        AuthorId = userId,
                    });
                }
            }
            else if(fromAlbumId == 0) // z hlavní galerie, vytvořím relaci
            {
                var albumTo = await _photoAlbumService.Get(toAlbumId);
                await PersonalTest(personal, albumTo.Personal);

                foreach (var photoId in photoIdList)
                {
                    insertRelace.Add(new RelationAlbumPhotoModel
                    {
                        PhotoId = photoId,
                        AlbumId = toAlbumId,
                        GroupId = groupId,
                        AuthorId = userId,
                    });
                }
            }
            else // mezi alby, budu mazat relaci + vytvořím novou relaci
            {
                var albumFrom = await _photoAlbumService.Get(fromAlbumId);
                var albumTo = await _photoAlbumService.Get(toAlbumId);
                photoAlbum = await AlbumUpdate(photoIdList, albumFrom);

                await PersonalTest(albumFrom.Personal, albumTo.Personal);

                foreach (var photoId in photoIdList)
                {
                    insertRelace.Add(new RelationAlbumPhotoModel
                    {
                        PhotoId = photoId,
                        AlbumId = toAlbumId,
                        GroupId = groupId,
                        AuthorId = userId,
                    });

                    deleteRelace.Add(new RelationAlbumPhotoModel
                    {
                        PhotoId = photoId,
                        AlbumId = fromAlbumId,
                        GroupId = groupId,
                        AuthorId = userId,
                    });
                }
            }

            await _galleryTransaction.MovePersonal(deleteRelace, insertRelace, photoAlbum);
        }

        private async Task PersonalTest(bool firstAlbumPersonal, bool secondAlbumPersonal)
        {
            if (firstAlbumPersonal != secondAlbumPersonal)
                throw new Exception("Nelze přesouvat fotografie mezi rodinným a sdíleným fotoalbem");
        }

        private async Task<PhotoAlbumModel> AlbumUpdate(List<int> photoIdList, PhotoAlbumModel? photoAlbum)
        {
            if (photoIdList.Contains(photoAlbum.TitlePhotoId)) // odstraní titulní foto z alba
                photoAlbum.TitlePhotoId = 0;

            return photoAlbum;
        }
    }
}
