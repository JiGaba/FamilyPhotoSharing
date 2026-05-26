using FamilyPhotoSharing.ApiRequests;
using FamilyPhotoSharing.ApiResponse;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
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

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : BaseController
    {
        private readonly IDownloadFileService _downloadFileService;
        private readonly IPhotoService _photoService;
        private readonly IPhotoAlbumService _photoAlbumService;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IGroupService _groupService;
        private readonly IGalleryService _galleryService;


        private const int PAGE_SIZE = 100; // Pak nastav třeba 800
        private const int PART_SIZE = 20; // pak nastav třeba 50

        public PhotoController(IDownloadFileService downloadFileService, IMemoryCache cache, ISystemLogService log, 
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

        [HttpPost("GetMainPersonalAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetMainPersonalAlbum(GetMainGalleryRequest request)
             => await GetAlbum(request.Page, ThumbnailData.MainPersonalGallery, 0);

        [HttpPost("GetMainGroupAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetMainGroupAlbum(GetMainGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.MainGroupGallery, 0);

        [HttpPost("MainGroupAlbumOwnPhoto")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> MainGroupAlbumOwnPhoto(GetMainGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.MainGroupGalleryOwnPhoto, 0);

        [HttpPost("ShowAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowAlbum(GetGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.Album, request.AlbumId);

        [HttpPost("ShowGroupAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowGroupAlbum(GetGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.GroupAlbum, request.AlbumId);

        [HttpPost("ShowMySharedAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowMySharedAlbum(GetGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.MySharedAlbum, request.AlbumId);

        [HttpPost("ShowSharedAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> ShowSharedAlbum(GetGalleryRequest request)
            => await GetAlbum(request.Page, ThumbnailData.SharedAlbum, request.AlbumId);

        private async Task<IActionResult> GetAlbum(int page, ThumbnailData thumbnailData, int albumId)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                if (page == 1) // smazání cache uchovávající celkový počet fotografií
                    _CacheInvalidate(_CacheKeyTotalImages(thumbnailData, result.userData.Id, result.userData.GroupId, albumId));

                var offset = PART_SIZE * (page - 1);

                var photoList = await GetPhoto(thumbnailData, result.userData.Id, result.userData.GroupId, albumId, PART_SIZE, offset);
                var galleryView = await GetGalleryViewAsync(thumbnailData, result.userData.Id, result.userData.GroupId, albumId);
                galleryView.Photos = photoList;
                galleryView.AlbumId = albumId;
                galleryView.UserRole = result.userData.RoleEnum;

                var totalCount = await GetTotalPhotos(thumbnailData, result.userData.Id, result.userData.GroupId, albumId);
                ViewBag.ThumbnailData = (int)thumbnailData;
                ViewBag.Page = page;
                ViewBag.PageSize = PAGE_SIZE;
                ViewBag.TotalCount = totalCount;

                var albumType = EnumHelper.GetDescription(thumbnailData);
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Zobrazení {albumType} fotoalba s Id {albumId}.");

                var photoApiResponse = galleryView.ToPhotoApiResponse((int)thumbnailData, page, PAGE_SIZE, totalCount);

                return Ok(ApiResponse<PhotoApiResponse>.Ok(photoApiResponse, $"Načtení množiny fotografií thumbnailData {thumbnailData}, page {page}, albumId {albumId}."));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se načíst množinu fotografií."));
            }
        }

        [HttpPost("LoadPhotos")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> LoadPhotos(LoadPhotosRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var thumbnailDataEnum = EnumHelper.GetEnum<ThumbnailData>(request.ThumbnailData);
                var totalPhotos = await GetTotalPhotos(thumbnailDataEnum, result.userData.Id, result.userData.GroupId, request.AlbumId);
                var totalShowPhotos = (((request.Part - 1) * PART_SIZE) + ((request.Page - 1) * PAGE_SIZE));

                if (totalShowPhotos >= totalPhotos || ((request.Part - 1) * PART_SIZE >= PAGE_SIZE))
                    return Ok(ApiResponse<List<PhotoModel>>.Ok(new List<PhotoModel>(), $"Načtení množiny fotek."));

                var offset = ((request.Page - 1) * PAGE_SIZE) + (request.Part - 1) * PART_SIZE;
                var photoList = await GetPhoto(thumbnailDataEnum, result.userData.Id, result.userData.GroupId, request.AlbumId, PART_SIZE, offset);

                _LogAPIInfo(ActionTypeEnum.Photo, $"Načtení množiny fotografií.");

                return Ok(ApiResponse<List<PhotoModel>>.Ok(photoList, $"Načtení množiny fotografií."));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se načíst množinu fotografií."));
            
             }
        }

        [HttpPost("GetFullPhoto")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetFullPhoto(GetPhotoRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var file = await _downloadFileService.GetPhoto(request.PhotoId, result.userData.Id, result.userData.Folder);

                if (file.data == null || file.data.Length <= 0)
                    return NotFound("Soubor nenalezen.");

                _LogAPIInfo(ActionTypeEnum.Photo, $"Zobrazení plného náhledu fotografie id {request.PhotoId}.");

                return File(file.data, file.mimeType);
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, "Nepodařilo se načíst plný náhled fotografie." + e.Message);
                return BadRequest("Nepodařilo se načíst plný náhled fotografie.");
            }
        }

        [HttpPost("GetThumbnail")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetThumbnail(GetPhotoRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var file = await _downloadFileService.GetThumbnail(request.PhotoId, result.userData.Id, result.userData.Folder);

                if (file == null || file.Length <= 0)
                    return NotFound("Soubor nenalezen.");

                _LogAPIInfo(ActionTypeEnum.Photo, $"Zobrazení náhledu fotografie id {request.PhotoId}.");

                return File(file, MineTypesTextEnum.WEBP);
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, "Nepodařilo se načíst náhled fotografie." + e.Message);
                return BadRequest("Nepodařilo se načíst náhled fotografie.");
            }
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

    }
}
