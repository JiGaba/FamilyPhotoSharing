using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.SharedPhotoAlbum;
using System.Text.RegularExpressions;

namespace FamilyPhotoSharing.Controllers.Gallery
{
    public class SharedAlbumController : BaseController
    {
        private readonly ISharedPhotoAlbumService _sharedAlbumService;
        public SharedAlbumController(IMemoryCache cache, ISystemLogService log, 
            ISharedPhotoAlbumService sharedAlbumService, IUserService userService) : base(cache, log, userService)
        {
            _sharedAlbumService = sharedAlbumService;
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> MySharedPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albums = await _sharedAlbumService.GetByOwnerId(result.userData.Id);
                var albumsView = albums.Select(a => a?.ToSharedPhotoAlbumView(null)).ToList();
                albumsView.ForEach(a => a.HostUserCount++);

                _LogInfo(ActionTypeEnum.SharedPhotoAlbum, "Zobrazení vlastní sdílených alb.");

                return View(albumsView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        public async Task<IActionResult> SharedPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albums = await _sharedAlbumService.GetByHostId(result.userData.Id);
                var albumsView = albums.Select(a => a?.ToSharedPhotoAlbumView(null)).ToList();
                albumsView.ForEach(a => a.HostUserCount++);

                _LogInfo(ActionTypeEnum.SharedPhotoAlbum, "Zobrazení se mnou sdílených alb.");

                return View(albumsView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> SharedPhotoAlbum(int albumId)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                if (albumId == 0)
                {
                    var newUserModal = await GetUsersModalView(new List<int> { result.userData.Id }, result.userData.Id);

                    var newAlbumView = new SharedPhotoAlbumView
                    {
                        UserGroupsId = result.userData.GroupId,
                        OwnerUserId = result.userData.Id,
                        CreateAuthor = result.userData.Id,
                        UserModalViews = newUserModal,
                    };

                    return View(newAlbumView);
                }

                // tady získej aktivní ID
                var album = await _sharedAlbumService.Get(albumId);
                var userModal = await GetUsersModalView(album?.HostUserIdsList, result.userData.Id);
                var albumView = album?.ToSharedPhotoAlbumView(userModal);

                if (albumView?.OwnerUserId != result.userData.Id)
                    return _Redirect_Logout();

                _LogInfo(ActionTypeEnum.SharedPhotoAlbum, $"Zobrazení sdíleného alba {albumView.AlbumName}.");

                return View(albumView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost]
        public async Task<IActionResult> EditSharedPhotoAlbum(SharedPhotoAlbumView photoAlbum)
        {
            try
            {
                var albumId = 0;
                var message = "";

                if (photoAlbum == null) // nemělo by nastat
                    return _Redirect_Logout();

                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                photoAlbum.CreateAuthor = result.userData.Id;

                if (photoAlbum.Id == 0)
                {
                    photoAlbum.OwnerUserId = result.userData.Id;
                    photoAlbum.UserGroupsId = result.userData.GroupId;

                    albumId = await _sharedAlbumService.Set(photoAlbum);
                    _LogInfo(ActionTypeEnum.AddSharedPhotoAlbum, $"Album ID: {photoAlbum.Id}, Název {photoAlbum.AlbumName}");

                    ViewBag.Type = MessageTypes.Success;
                    ViewBag.Message = $"Sdílené album {photoAlbum.AlbumName} bylo úspěšně vytvořeno.";
                }
                else
                {
                    albumId = photoAlbum.Id;
                    await _sharedAlbumService.Update(photoAlbum);
                    _LogInfo(ActionTypeEnum.EditSharedPhotoAlbum, $"Album ID: {albumId}, Název {photoAlbum.AlbumName}");

                    ViewBag.Type = MessageTypes.Success;
                    ViewBag.Message = $"Sdílené album {photoAlbum.AlbumName} bylo úspěšně upraveno.";
                }

                var photoAlbumLoad = await _sharedAlbumService.Get(albumId);
                var userModal = await GetUsersModalView(photoAlbumLoad?.HostUserIdsList, result.userData.Id);
                var photoAlbumView = photoAlbumLoad?.ToSharedPhotoAlbumView(userModal);

                return View("SharedPhotoAlbum", photoAlbumView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.SharedPhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        [HttpPost]
        public async Task<IActionResult> AddTitleImage([FromBody] AddTitleImageRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            if (request == null)
                return BadRequest();

            if (request.PhotoId == 0 || request.AlbumId == 0)
                return BadRequest("Nebyla vybrána žádná fotografie nebo není určeno album id.");

            try
            {
                var album = await _sharedAlbumService.Get(request.AlbumId);
                album.TitlePhotoId = request.PhotoId;
                album.CreateAuthor = result.userData.Id;
                await _sharedAlbumService.Update(album);

                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Přiřazení fotografie {request.PhotoId} ke sdílenému albu {request.AlbumId}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                return BadRequest("Přiřazení úvodní fotografie se nezdařilo.");
            }
        }
    }
}
