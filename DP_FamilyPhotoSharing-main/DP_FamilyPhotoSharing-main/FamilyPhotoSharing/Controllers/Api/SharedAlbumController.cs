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
using ServiceLayer.Services.SharedPhotoAlbum;

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedAlbumController : BaseController
    {
        private readonly ISharedPhotoAlbumService _sharedAlbumService;
        public SharedAlbumController(IMemoryCache cache, ISystemLogService log, IUserService userService, 
            ISharedPhotoAlbumService sharedAlbumService) : base(cache, log, userService)
        {
            _sharedAlbumService = sharedAlbumService;
        }

        [HttpGet("GetMySharedPhotoAlbums")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetMySharedPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albums = await _sharedAlbumService.GetByOwnerId(result.userData.Id);
                var albumsView = albums.Select(a => a?.ToSharedPhotoAlbumView(null)).ToList();
                albumsView.ForEach(a => a.HostUserCount++);

                _LogAPIInfo(ActionTypeEnum.SharedPhotoAlbum, "Zobrazení vlastních sdílených fotoalb.");

                return Ok(ApiResponse<List<SharedPhotoAlbumView>>.Ok(albumsView, $"Zobrazení vlastních sdílených fotoalb."));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se zobrazení vlastních sdílených fotoalb."));
            }
        }

        [HttpGet("GetSharedPhotoAlbums")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetSharedPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var albums = await _sharedAlbumService.GetByHostId(result.userData.Id);
                var albumsView = albums.Select(a => a?.ToSharedPhotoAlbumView(null)).ToList();
                albumsView.ForEach(a => a.HostUserCount++);

                _LogAPIInfo(ActionTypeEnum.SharedPhotoAlbum, "Zobrazení sdílených fotoalb.");

                return Ok(ApiResponse<List<SharedPhotoAlbumView>>.Ok(albumsView, $"Zobrazení sdílených fotoalb."));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se zobrazit sdílených fotoalb."));
            }
        }

        [HttpPost("GetSharedPhotoAlbum")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetSharedPhotoAlbum(GetPhotoAlbumRequest request)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var album = await _sharedAlbumService.Get(request.AlbumId);
              
                _LogAPIInfo(ActionTypeEnum.SharedPhotoAlbum, "Zobrazení sdíleného fotoalba.");
                
                return Ok(ApiResponse<SharedPhotoAlbumModel>.Ok(album, $"Zobrazení sdíleného fotoalba s id ${request.AlbumId}"));
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<string>.Fail($"Nepodařilo se zobrazit sdílené fotoalba s id ${request.AlbumId}"));
            }
        }
    }
}
