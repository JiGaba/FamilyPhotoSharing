using FamilyPhotoSharing.ApiRequests;
using FamilyPhotoSharing.ApiResponse;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : BaseController
    {
        private readonly IPhotoAlbumService _photoAlbumService;
        public AlbumController(IMemoryCache cache, ISystemLogService log, IUserService userService, IPhotoAlbumService photoAlbumService) : base(cache, log, userService)
        {
            _photoAlbumService = photoAlbumService;
        }

        [HttpGet("GetPersonalPhotoAlbums")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetPersonalPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albums = await _photoAlbumService.GetByOwnerId(result.userData.Id);

                _LogAPIInfo(ActionTypeEnum.PhotoAlbum, "Zobrazení všech soukromých fotoalb.");

                return Ok(ApiResponse<List<PhotoAlbumsModel>>.Ok(albums, $"Předány informace o všech soukromých fotoalbech uživatele s id {result.userData.Id}."));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<UserModel>.Fail($"Nepodařilo se předat informace o soukromých fotoalbech."));
            }
        }

        [HttpPost("GetPersonalPhotoAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetPersonalPhotoAlbum(GetPhotoAlbumRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albumView = await GetPhotoAlbumView(request.AlbumId, result.userData.GroupId, result.userData.Id, true);
                if (albumView.OwnerUserId != result.userData.Id)
                    return BadRequest("Nejedná se o soukromé album daného uživatele.");

                _LogAPIInfo(ActionTypeEnum.PhotoAlbum, "Zobrazení všech soukromých fotoalb.");

                return Ok(ApiResponse<PhotoAlbumView>.Ok(albumView, $"Zobrazení alba {albumView.AlbumName}"));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<UserModel>.Fail($"Nepodařilo se předat informace o soukromém fotoalbu."));
            }
        }

        [HttpGet("GetGroupPhotoAlbums")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetGroupPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albums = await _photoAlbumService.GetByGroupsId(result.userData.GroupId);
                var albumView = albums.Select(a => a?.ToPhotoAlbumView()).ToList();

                _LogAPIInfo(ActionTypeEnum.PhotoAlbum, "Zobrazení všech rodinných fotoalb rodiny.");

                return Ok(ApiResponse< List<PhotoAlbumView>>.Ok(albumView, $"Zobrazení rodinných alb {result.userData.GroupName}"));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<UserModel>.Fail($"Nepodařilo se zobrazit rodinné alba."));
            }
        }

        [HttpPost("GetGroupPhotoAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetGroupPhotoAlbum(GetPhotoAlbumRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albumView = await GetPhotoAlbumView(request.AlbumId, result.userData.GroupId, result.userData.Id, false);

                _LogAPIInfo(ActionTypeEnum.PhotoAlbum, "Zobrazení rodinného fotoalba.");

                if (albumView.UserGroupsId != result.userData.GroupId || albumView.Personal)
                    return BadRequest("Zobrazení rodinného fotoalba je soukromé nebo z jiné rodinného fotoalba.");

                return Ok(ApiResponse<PhotoAlbumView>.Ok(albumView, $"Zobrazení rodinných alb {result.userData.GroupName}"));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se zobrazit rodinné alba."));
            }
        }
        private async Task<PhotoAlbumView> GetPhotoAlbumView(int albumId, int groupId, int userId, bool personal)
        {
            if (albumId == 0)
                return new PhotoAlbumView
                {
                    Personal = personal,
                    UserGroupsId = groupId,
                    OwnerUserId = userId,
                    CreateAuthor = userId,
                };

            var album = await _photoAlbumService.Get(albumId);
            var redactor = await _userService.Get(album.CreateAuthor);
            var redactorName = redactor == null ? "" : redactor.GetName();
            var albumView = album?.ToPhotoAlbumView(redactorName, 0);

            return albumView;
        }
    }
}
