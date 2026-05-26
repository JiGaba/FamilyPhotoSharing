using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FamilyPhotoSharing.Controllers.Gallery
{
    public class AlbumController : BaseController
    {
        private readonly IPhotoAlbumService _photoAlbumService;
        public AlbumController(IMemoryCache cache, ISystemLogService log, IPhotoAlbumService photoAlbumService, IUserService userService) : base(cache, log, userService)
        {
            _photoAlbumService = photoAlbumService;
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> PersonalPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albums = await _photoAlbumService.GetByOwnerId(result.userData.Id);
                var albumView = albums.Select(a => a?.ToPhotoAlbumView()).ToList();

                _LogInfo(ActionTypeEnum.PhotoAlbum, "Zobrazení všech soukromých fotoalb.");

                return View(albumView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> PersonalPhotoAlbum(int albumId = 0)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albumView = await GetPhotoAlbumView(albumId, result.userData.GroupId, result.userData.Id, true);

                if (albumView.OwnerUserId != result.userData.Id)
                    return _Redirect_Logout();

                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Zobrazení alba {albumView.AlbumName}");

                return View(albumView);
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
        public async Task<IActionResult> EditPersonalPhotoAlbum(PhotoAlbumView photoAlbum)
        {
            try
            {
                if (photoAlbum == null || !photoAlbum.Personal) // nemělo by nastat
                    return _Redirect_Logout();

                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var saveResult = await SavePhotoAlbum(photoAlbum, result.userData.Id, result.userData.GroupId);
                _LogInfo(ActionTypeEnum.EditPhotoAlbum, $"Editace alba {saveResult.photoAlbum.AlbumName}");

                ViewBag.Message = saveResult.message;
                ViewBag.Type = saveResult.type;
                return View("PersonalPhotoAlbum", saveResult.photoAlbum);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin}")]
        public async Task<IActionResult> GroupPhotoAlbums()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albums = await _photoAlbumService.GetByGroupsId(result.userData.GroupId);
                var albumView = albums.Select(a => a?.ToPhotoAlbumView()).ToList();
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Zobrazení všech rodinných fotoalb rodiny {result.userData.GroupName}.");

                return View(albumView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin}")]
        public async Task<IActionResult> GroupPhotoAlbum(int albumId)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var albumView = await GetPhotoAlbumView(albumId, result.userData.GroupId, result.userData.Id, false);
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Zobrazení rodinného fotoalba {albumView.AlbumName} rodiny {result.userData.GroupName}.");

                // Album je soukromé nebo z jiné skupiny, editovat jdou pouze Alba z vlastní skupiny
                if (albumView.UserGroupsId != result.userData.GroupId || albumView.Personal)
                    return _Redirect_Logout();

                return View(albumView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin}")]
        [HttpPost]
        public async Task<IActionResult> EditGroupPhotoAlbum(PhotoAlbumView photoAlbum)
        {
            try
            {
                if (photoAlbum == null || photoAlbum.Personal) // nemělo by nastat
                    return _Redirect_Logout();

                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var saveResult = await SavePhotoAlbum(photoAlbum, result.userData.Id, result.userData.GroupId);
                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Editace rodinného fotoalba {saveResult.photoAlbum.AlbumName} rodiny {result.userData.GroupName}.");

                ViewBag.Type = saveResult.type;
                ViewBag.Message = saveResult.message;
                return View("GroupPhotoAlbum", saveResult.photoAlbum);
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
                var album = await _photoAlbumService.Get(request.AlbumId);
                album.TitlePhotoId = request.PhotoId;
                album.CreateAuthor = result.userData.Id;
                await _photoAlbumService.Update(album);

                _LogInfo(ActionTypeEnum.PhotoAlbum, $"Přiřazení fotografie {request.PhotoId} k albu {request.AlbumId}");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.PhotoAlbum, e.Message);
                return BadRequest("Přiřazení úvodní fotografie se nezdařilo.");
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

        private async Task<(PhotoAlbumView photoAlbum, string message, string type)> SavePhotoAlbum(PhotoAlbumView photoAlbum, int userId, int groupId)
        {
            var albumId = 0;
            var message = "";
            var albumType = photoAlbum.Personal ? "Soukromé" : "Rodinné";

            photoAlbum.CreateAuthor = userId;

            if (photoAlbum.Id == 0)
            {
                photoAlbum.OwnerUserId = userId;
                photoAlbum.UserGroupsId = groupId;
                albumId = await _photoAlbumService.Set(photoAlbum);
                _LogInfo(ActionTypeEnum.AddPhotoAlbum, $"Album ID: {photoAlbum.Id}, Název {photoAlbum.AlbumName}");
                message = $"{albumType} album {photoAlbum.AlbumName} bylo úspěšně vytvořeno.";
            }
            else
            {
                albumId = photoAlbum.Id;
                await _photoAlbumService.Update(photoAlbum);
                _LogInfo(ActionTypeEnum.AddPhotoAlbum, $"Album ID: {albumId}, Název {photoAlbum.AlbumName}");
                message = $"{albumType} album {photoAlbum.AlbumName} bylo úspěšně upraveno.";
            }

            var photoAlbumLoad = await _photoAlbumService.Get(albumId);
            var redactor = await _userService.Get(photoAlbum.CreateAuthor);
            var redactorName = redactor == null ? "" : redactor.GetName();
            var photoAlbumView = photoAlbumLoad?.ToPhotoAlbumView(redactorName, 0);

            return (photoAlbumView, message, MessageTypes.Success);
        }
    }
}
