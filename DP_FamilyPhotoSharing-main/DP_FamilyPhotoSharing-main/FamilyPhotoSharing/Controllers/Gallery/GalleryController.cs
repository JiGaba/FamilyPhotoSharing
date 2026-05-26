using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Gallery;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.SharedPhotoAlbum;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FamilyPhotoSharing.Controllers.Gallery
{
    public class GalleryController : BaseController
    {
        private readonly IDownloadFileService _downloadFileService;
        private readonly IPhotoService _photoService;
        private readonly IPhotoAlbumService _photoAlbumService;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IGroupService _groupService;
        private readonly IGalleryService _galleryService;

        private const int PAGE_SIZE = 100; 
        private const int PART_SIZE = 20;

        public GalleryController(IDownloadFileService downloadFileService, IMemoryCache cache, ISystemLogService log, 
            IUserService userService, IPhotoService photoService, IPhotoAlbumService photoAlbumService, 
            ISharedPhotoAlbumService sharedPhotoAlbumService, IGroupService groupService, IGalleryService galleryService) : base(cache, log, userService)
        {
            _downloadFileService = downloadFileService;
            _photoService = photoService;
            _photoAlbumService = photoAlbumService;
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
            _groupService = groupService;
            _galleryService = galleryService;
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> MainPersonalAlbum(int page = 1)
            => await GetAlbum(page, ThumbnailData.MainPersonalGallery, 0);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> MainGroupAlbum(int page = 1)
            => await GetAlbum(page, ThumbnailData.MainGroupGallery, 0);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> MainGroupAlbumOwnPhoto(int page = 1)
            => await GetAlbum(page, ThumbnailData.MainGroupGalleryOwnPhoto, 0);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowAlbum(int albumId, int page = 1)
            => await GetAlbum(page, ThumbnailData.Album, albumId);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowGroupAlbum(int albumId, int page = 1)
            => await GetAlbum(page, ThumbnailData.GroupAlbum, albumId);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowMySharedAlbum(int albumId, int page = 1)
            => await GetAlbum(page, ThumbnailData.MySharedAlbum, albumId);

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        public async Task<IActionResult> ShowSharedAlbum(int albumId, int page = 1)
            => await GetAlbum(page, ThumbnailData.SharedAlbum, albumId);

        private async Task<IActionResult> GetAlbum(int page, ThumbnailData thumbnailData, int albumId)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                if (page == 1) // smazání cache uchovávající celkový počet fotografií
                    _CacheInvalidate(_CacheKeyTotalImages(thumbnailData, result.userData.Id, result.userData.GroupId, albumId));

                var offset = PAGE_SIZE * (page - 1);

                var photoList = await GetPhoto(thumbnailData, result.userData.Id, result.userData.GroupId, albumId, PART_SIZE, offset);
                var galleryView = await GetGalleryViewAsync(thumbnailData, result.userData.Id, result.userData.GroupId, albumId);
                galleryView = await GetGalleryViewModals(galleryView, thumbnailData, result.userData.Id, albumId);
                galleryView.Photos = photoList;
                galleryView.AlbumId = albumId;
                galleryView.UserRole = result.userData.RoleEnum;

                ViewBag.ThumbnailData = (int)thumbnailData;
                ViewBag.Page = page;
                ViewBag.PageSize = PAGE_SIZE;
                ViewBag.TotalCount = await GetTotalPhotos(thumbnailData, result.userData.Id, result.userData.GroupId, albumId);

                var albumType = EnumHelper.GetDescription(thumbnailData);
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Zobrazení {albumType} fotoalba s Id {albumId}.");

                return View("Gallery", galleryView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost]
        public async Task<IActionResult> MovePersonalOrGroup([FromBody] MoveRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            if (request == null)
                return BadRequest();

            if (request.PhotoIds == null || request.PhotoIds.Count == 0)
                return BadRequest("Nebyla vybrána žádná fotografie.");

            try
            {
                await _galleryService.MovePersonalOrGroup(request.PhotoIds.ToList(), request.FromAlbumId, request.ToAlbumId, result.userData.GroupId, result.userData.Id, request.Personal);
                MoveAlbumInvalidate(request, result.userData.Id, result.userData.GroupId);
                
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Přesun {request.PhotoIds.Count} fotografií z alba {request.FromAlbumId} do alba {request.ToAlbumId}");

                return Json(new { success = true });
            }
            catch(Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                return BadRequest("Přesun fotografií se nezdařil.");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        public async Task<IActionResult> LoadPhotos(int page = 1, int part = 1, int thumbnailData = 1, int albumId = 0)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var thumbnailDataEnum = EnumHelper.GetEnum<ThumbnailData>(thumbnailData);
                var totalPhotos = await GetTotalPhotos(thumbnailDataEnum, result.userData.Id, result.userData.GroupId, albumId);
                var totalShowPhotos = (((part - 1) * PART_SIZE) + ((page - 1) * PAGE_SIZE));

                if (totalShowPhotos >= totalPhotos || ((part - 1) * PART_SIZE >= PAGE_SIZE))
                    return PartialView("Partials/_PhotoThumbnailsPartial", new List<PhotoModel>());
                
                var offset = ((page - 1) * PAGE_SIZE) + (part - 1) * PART_SIZE;
                var photoList = await GetPhoto(thumbnailDataEnum, result.userData.Id, result.userData.GroupId, albumId, PART_SIZE, offset);

                _LogInfo(ActionTypeEnum.Photo, $"Načtení množiny fotek page[{page}], part[{part}], thumbnailData[{thumbnailData}], albumId[{albumId}].");

                return PartialView("Partials/_PhotoThumbnailsPartial", photoList);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Photo, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            } 
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        public async Task<IActionResult> GetFullPhoto(int id)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var file = await _downloadFileService.GetPhoto(id, result.userData.Id, result.userData.Folder);

                if (file.data == null || file.data.Length == 0)
                    return NotFound();

                _LogInfo(ActionTypeEnum.Photo, $"Načtení fotografie s id [{id}].");

                return File(file.data, file.mimeType);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Photo, "Nepodařilo se načíst plný náhled fotografie."+ e.Message);
                return BadRequest("Nepodařilo se načíst plný náhled fotografie.");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        public async Task<IActionResult> GetThumbnail(int id)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var file = await _downloadFileService.GetThumbnail(id, result.userData.Id, result.userData.Folder);

                if (file == null || file.Length == 0)
                    return NotFound();

                _LogInfo(ActionTypeEnum.Photo, $"Načtení náhledu s id [{id}].");

                return File(file, MineTypesTextEnum.WEBP);
            }
            catch(Exception e)
            {
                _LogError(ActionTypeEnum.Photo, "Nepodařilo se načíst náhled fotografie." + e.Message);
                return BadRequest("Nepodařilo se načíst náhled fotografie.");
            }
        }

        private async Task<List<PhotoAlbumView>> GetPhotoAlbumViewModal(bool personal, int albumId = 0)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return new List<PhotoAlbumView>();

            var albums = new List<PhotoAlbumsModel>();

            if (personal)
            {
                var user = await _userService.Get(result.userData.Id);
                
                albums.Add(new PhotoAlbumsModel
                {
                    AlbumName = $"Soukromá galerie - {user?.GetName()}",
                    Id = 0,
                    PhotoCount = await GetPhotoCount(ThumbnailData.MainPersonalGallery, result.userData.Id, result.userData.GroupId, albumId),
                });

                albums.AddRange(await _photoAlbumService.GetByOwnerId(result.userData.Id));
            }
            else
            {
                albums.Add(new PhotoAlbumsModel
                {
                    AlbumName = $"Rodinná galerie - {result.userData.GroupName}",
                    Id = 0,
                    PhotoCount = await GetPhotoCount(ThumbnailData.MainGroupGallery, result.userData.Id, result.userData.GroupId, albumId),
                });

                albums.AddRange(await _photoAlbumService.GetByGroupsId(result.userData.GroupId));
            }       

            return albums.Select(a => a.ToPhotoAlbumView(a.Id == albumId)).ToList();
        }
        private async Task<List<SharedPhotoAlbumView>> GetSharedPhotoAlbumsModal(int albumId = 0)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return new List<SharedPhotoAlbumView>();

            var albums = await _sharedPhotoAlbumService.GetByOwnerId(result.userData.Id);

            return albums.Select(a => a.ToSharedPhotoAlbumView(null, a.Id == albumId)).ToList();
        }
        private async Task<int> GetTotalPhotos(ThumbnailData thumbnailData, int userId, int groupId, int albumId)
        {
            var cacheKey = _CacheKeyTotalImages(thumbnailData, userId, groupId, albumId);

            return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_MINUTES);

                return thumbnailData switch
                {
                    ThumbnailData.MainPersonalGallery => await _photoService.GetPhotoCount(userId, true),
                    ThumbnailData.MainGroupGalleryOwnPhoto => await _photoService.GetPhotoCount(userId, false),
                    ThumbnailData.MainGroupGallery => await _photoService.GetPhotoCountByGroupId(groupId),
                    ThumbnailData.Album or ThumbnailData.GroupAlbum => await _photoService.GetPhotoCountByAlbumId(albumId),
                    ThumbnailData.SharedAlbum or ThumbnailData.MySharedAlbum => await _photoService.GetPhotoCountByShardAlbumId(albumId),
                    _ => throw new NotImplementedException()
                };
            });
        }

        private void InvalidateTotalThumbnailCache(ThumbnailData thumbnailData, int userId, int groupId, int albumId)
            => _CacheInvalidate(_CacheKeyTotalImages(thumbnailData, userId, groupId, albumId));

        private async Task<GalleryView> GetGalleryViewModals(GalleryView gallery, ThumbnailData thumbnailData, int userId, int albumId)
        {
            var userModal = new List<UserModalView>();

            if (thumbnailData.Equals(ThumbnailData.MySharedAlbum))
            {
                var sharedAlbum = await _sharedPhotoAlbumService.Get(albumId);
                userModal = await GetUsersModalView(sharedAlbum?.HostUserIdsList, userId);
            }

            var albumsTask = GetPhotoAlbumViewModal(true, albumId);
            var albumsGroupTask = GetPhotoAlbumViewModal(false, albumId);
            var sharedAlbumsTask = GetSharedPhotoAlbumsModal(albumId);

            await Task.WhenAll(albumsTask, albumsGroupTask, sharedAlbumsTask);

            var (albumsModal, albumsGroupModal, sharedAlbumsModal) =
                (albumsTask.Result, albumsGroupTask.Result, sharedAlbumsTask.Result);

            gallery.PersonalPhotoAlbumModal = albumsModal;
            gallery.GroupPhotoAlbumModal = albumsGroupModal;
            gallery.SharedModal = sharedAlbumsModal;
            gallery.UsersModal = userModal;

            return gallery;
        }
        
        private async Task<GalleryView> GetGalleryViewAsync(ThumbnailData thumbnailData, int userId, int groupId, int albumId)
        {
            var galleryView = new GalleryView();
            
            switch (thumbnailData)
            {
                case ThumbnailData.MainPersonalGallery:
                    var user = await _userService.Get(userId);
                    galleryView.GalleryName = $"Soukromá galerie - {user.GetName()}";
                    galleryView.GalleryDescription = "";
                    galleryView.TitlePotoId = user.SystemImagesId;
                    break;
                case ThumbnailData.MainGroupGallery:
                    var group = await _groupService.Get(groupId);
                    galleryView.GalleryDescription = group.GroupDescription;
                    galleryView.GalleryName = $"Rodinná galerie - {group.GroupName}";
                    galleryView.TitlePotoId = 0;
                    break;
                case ThumbnailData.MainGroupGalleryOwnPhoto:
                    var groupOwnPhoto = await _groupService.Get(groupId);
                    galleryView.GalleryDescription = groupOwnPhoto.GroupDescription;
                    galleryView.GalleryName = $"Rodinná galerie - {groupOwnPhoto.GroupName} [vlastní]";
                    galleryView.TitlePotoId = 0;
                    break;
                case ThumbnailData.Album or ThumbnailData.GroupAlbum:
                    var album = await _photoAlbumService.Get(albumId);
                    galleryView.GalleryDescription = album.AlbumDescription;
                    galleryView.GalleryName = album.AlbumName;
                    galleryView.TitlePotoId = album.TitlePhotoId;
                    break;
                case ThumbnailData.MySharedAlbum or ThumbnailData.SharedAlbum:
                    var sharedAlbum = await _sharedPhotoAlbumService.Get(albumId);
                    galleryView.GalleryDescription = sharedAlbum.AlbumDescription;
                    galleryView.GalleryName = sharedAlbum.AlbumName;
                    galleryView.TitlePotoId = sharedAlbum.TitlePhotoId;
                    break;
                default: break;
            }

            galleryView.ThumbnailData = thumbnailData;

            return galleryView;
        }

        private async Task<List<PhotoModel>> GetPhoto(ThumbnailData thumbnailData, int userId, int groupId, int albumId, int fetch, int offset)
            => thumbnailData switch
            {
                ThumbnailData.MainPersonalGallery => await _photoService.SelectByOwnerAndPersonal(userId, true, fetch, offset),
                ThumbnailData.MainGroupGalleryOwnPhoto => await _photoService.SelectByOwnerAndPersonal(userId, false, fetch, offset),
                ThumbnailData.MainGroupGallery => await _photoService.SelectByGroupId(groupId, fetch, offset),
                ThumbnailData.Album or ThumbnailData.GroupAlbum => await _photoService.SelectByAlbumId(albumId, fetch, offset),
                ThumbnailData.MySharedAlbum or ThumbnailData.SharedAlbum => await _photoService.SelectBySharedAlbumId(albumId, fetch, offset),
                _ => throw new NotImplementedException()
            };

        private async Task<int> GetPhotoCount(ThumbnailData thumbnailData, int userId, int groupId, int albumId)
            => thumbnailData switch
            {
                ThumbnailData.MainPersonalGallery => await _photoService.GetPhotoCount(userId, true),
                ThumbnailData.MainGroupGallery => await _photoService.GetPhotoCountByGroupId(groupId),
                ThumbnailData.MainGroupGalleryOwnPhoto => await _photoService.GetPhotoCount(userId, false),
                ThumbnailData.Album => await _photoService.GetPhotoCountByAlbumId(albumId),
                ThumbnailData.SharedAlbum => await _photoService.GetPhotoCountByShardAlbumId(albumId),
                _ => throw new NotImplementedException()
            };
        private void MoveAlbumInvalidate(MoveRequest request, int userId, int groupId)
        {
            if (request.Personal)
            {
                if (request.FromAlbumId == 0)
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.MainPersonalGallery, userId, groupId, request.FromAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.Album, userId, groupId, request.ToAlbumId);
                }
                else if (request.ToAlbumId == 0)
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.MainPersonalGallery, userId, groupId, request.ToAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.Album, userId, groupId, request.FromAlbumId);
                }
                else
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.Album, userId, groupId, request.ToAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.Album, userId, groupId, request.FromAlbumId);
                }
            }
            else
            {
                if (request.FromAlbumId == 0)
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.MainGroupGallery, userId, groupId, request.FromAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.GroupAlbum, userId, groupId, request.ToAlbumId);
                }
                else if (request.ToAlbumId == 0)
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.MainGroupGallery, userId, groupId, request.ToAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.GroupAlbum, userId, groupId, request.FromAlbumId);
                }
                else
                {
                    InvalidateTotalThumbnailCache(ThumbnailData.GroupAlbum, userId, groupId, request.ToAlbumId);
                    InvalidateTotalThumbnailCache(ThumbnailData.GroupAlbum, userId, groupId, request.FromAlbumId);
                }
            }
        }
    }
}
