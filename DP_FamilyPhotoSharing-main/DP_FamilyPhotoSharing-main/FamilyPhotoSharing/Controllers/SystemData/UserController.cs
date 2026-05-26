using EncryptionLayer.Password;
using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using FileAccessLayer.DbInit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.BackgroundModels;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.BackgroundServices;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using System.Collections;
using System.Reflection;
using System.Security.Claims;

namespace FamilyPhotoSharing.Controllers.SystemData
{
    public class UserController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IDownloadFileService _downloadFileService;
        public UserController(IUserService userService, IGroupService groupService, IMemoryCache cache, ISystemLogService log,
            IUploadFileService uploadFileService, IDownloadFileService downloadFileService) : base(cache, log, userService)
        { 
            _groupService = groupService;
            _uploadFileService = uploadFileService;
            _downloadFileService = downloadFileService;
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin}")]
        public async Task<IActionResult> UserProfiles()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var groups = (await _groupService.GetList()) ?? new List<UserGroupModel?>();
                var groupId = result.userData.RoleEnum.Equals(UserRoleEnum.Admin) ? 0 : result.userData.GroupId;

                var users = await _userService.GetList(groupId);
                var usersView = users?.Select(u => u?.ToUserView(0, string.Empty, groups)).ToList();

                _LogInfo(ActionTypeEnum.User, "Zobrazení uživatelských profilů.");

                return View(usersView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.User, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        // zabezpeč aby uživatele viděl jen svůj profil nebo admin viděl ostatní profily
        public async Task<IActionResult> Profile(int userId = 0, bool newProfile = false)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                // 1) Vytvoření nového uživatele
                if (newProfile && userId == 0)
                {
                    // Oprávnění ->
                    // Ověření oprávnění zda může redactor vytvářet uživatele
                    if (!UserRoles.CanCreateUser(result.userData.RoleId))
                    {
                        // Nemá oprávnění, zobrazení profilu redactora
                        var userView = await GetUserView(result.userData.Id);

                        if (userView == null)
                            return RedirectToAction("Logout", "Account", new { message = "Z důvodu bezpečnosti jste byli odhlášeni, opětovně se prosím přihlašte." });

                        ViewBag.Type = MessageTypes.Warning;
                        ViewBag.Message = "Můžete editovat pouze vlastní profil nebo profily s nižším oprávněním.";

                        _LogInfo(ActionTypeEnum.User, "Uživatel nemá oprávnění vytvářet nové uživatele.");

                        return View(userView);
                    }
                    // <- Oprávnění 

                    var groups = new List<UserGroupModel?>();
                    if (result.userData.RoleEnum.Equals(UserRoleEnum.Admin))
                        groups = (await _groupService.GetList()) ?? new List<UserGroupModel?>();
                    else
                        groups.Add(await _groupService.Get(result.userData.GroupId));

                    return View(new UserView
                    {
                        UserDescription = "",
                        UserLogin = "",
                        UserName = "",
                        UserPasswordPlain = "",
                        UserSurname = "",
                        RedactorRoleId = result.userData.RoleId,
                        Active = true,
                        RoleId = 3,
                        UserGroup = groups
                    });
                }
                // 2) Editace uživatelského profilu 
                else
                {
                    var userView = await GetUserView(userId);

                    if (userView == null) // Tento stav by neměl nastat
                        return RedirectToAction("Logout", "Account", new { message = "Došlo k neočekávané chybě. Z důvodu bezpečnosti jste byli odhlášeni, opětovně se prosím přihlašte." });

                    userView.RedactorId = result.userData.Id;
                    userView.RedactorRoleId = result.userData.RoleId;

                    // Oprávnění ->
                    if (userView.Id != result.userData.Id) // Needituji vlastní profil musím zkusit oprávnění
                    {
                        var withoutPermission = false;
                        var logMessage = "";

                        if (userView.GroupId == result.userData.GroupId && !UserRoles.IsEditableByRoleSameGroup(userView.RoleId, result.userData.RoleId))
                        {
                            ViewBag.Message = "Můžete editovat pouze vlastní profil nebo profily s nižším oprávněním.";
                            logMessage = "Uživatele se pokusil editovat cizí profil s vyšším oprávněním.";
                            withoutPermission = true;
                        }
                        else if (userView.GroupId != result.userData.GroupId && !UserRoles.IsEditableByRoleOtherGroup(userView.RoleId, result.userData.RoleId))
                        {
                            ViewBag.Message = "Můžete editovat pouze vlastní profil nebo profily s nižším oprávněním ve vlastní skupině.";
                            logMessage = "Uživatel se pokusil editovat cizí profil s vyšším oprávněním nebo necházející se v jiné skupině.";
                            withoutPermission = true;
                        }

                        if (withoutPermission)
                        {
                            var redactorView = await GetUserView(result.userData.Id);
                            ViewBag.Type = MessageTypes.Warning;

                            if (redactorView == null)
                                return RedirectToAction("Logout", "Account", new { message = "Došlo k neočekávané chybě. Z důvodu bezpečnosti jste byli odhlášeni, opětovně se prosím přihlašte." });

                            _LogInfo(ActionTypeEnum.User, logMessage);
                            return View(redactorView);
                        }
                    }

                    return View(userView);
                }
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.User, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var user = await _userService.GetUserByLogin(model.UserLogin);

            if (user?.UserLogin == model.UserLogin &&
                PasswordEncryption.VerifyPassword(model.OldPassword, user.UserPasswordHash))
            {
                await _userService.UpdatePassword(new UserPasswordModel
                {
                    Id = model.UserId,
                    NewPassword = model.NewPassword,
                });

                return Json(new { success = true });
            }

            _LogInfo(ActionTypeEnum.User, $"Uživatel s loginem {model.UserLogin} si úspěšně změnil heslo.");

            return Json(new { success = false, message = "Zadané heslo se neshoduje s původním heslem uživatele!" });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [HttpPost]
        public async Task<IActionResult> Profile(UserView user)
        {
            UserModel userModel = user;
            var logMessage = string.Empty;
            ActionTypeEnum actionType = ActionTypeEnum.Info;
            ViewBag.Type = MessageTypes.Success;
            var smallerPermissions = false;
            var userOriginal = await _userService.Get(user.Id);

            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            var userWithSameLogin = await _userService.GetUserByLogin(user.UserLogin);
            if(userWithSameLogin != null && user.Id == 0)
            {
                ViewBag.Message = $"Uživatel s loginem {user.UserLogin} již existuje!";
                ViewBag.Type = MessageTypes.Warning;

                var groups = new List<UserGroupModel?>();
                if (result.userData.RoleEnum.Equals(UserRoleEnum.Admin))
                    groups = (await _groupService.GetList()) ?? new List<UserGroupModel?>();
                else
                    groups.Add(await _groupService.Get(result.userData.GroupId));

                return View(new UserView
                {
                    UserDescription = user.UserDescription,
                    UserLogin = user.UserLogin,
                    UserName = user.UserName,
                    UserPasswordPlain = "",
                    UserSurname = user.UserSurname,
                    RedactorRoleId = result.userData.RoleId,
                    Active = true,
                    RoleId = user.RoleId,
                    UserGroup = groups
                });
            }

            user.CreateAuthor = result.userData.Id;
            var userId = user.Id;

            try
            {
                if (user.Id == 0)
                {
                    actionType = ActionTypeEnum.AddUser;
                    logMessage = "Vytvoření uživatele selhalo.";
                    ViewBag.Type = MessageTypes.Warning;
                    userId = await _userService.SetNewUser(userModel);
                    logMessage = "Vytvoření uživatele proběhlo úspěšně.";
                    ViewBag.Type = MessageTypes.Success;
                }
                else
                {
                    actionType = ActionTypeEnum.EditUser;
                    logMessage = "Editace uživatele selhala.";
                    ViewBag.Type = MessageTypes.Warning;
                    await _userService.SetUser(userModel);
                    logMessage = "Editace uživatele proběhla úspěšně.";
                    ViewBag.Type = MessageTypes.Success;

                    if (result.userData.Id == user.Id)
                        await ChangeClaimItem.UpdateUserCustomClaimAsync(HttpContext, user.UserName, ClaimTypes.Name);

                    // Ověřit zda se snížilo oprávnění
                    smallerPermissions = (userOriginal.RoleId < userModel.RoleId && userOriginal.Id == result.userData.Id);
                }

                _LogInfo(actionType, string.Empty);

                if (smallerPermissions)
                    return RedirectToAction("Logout", "Account", new { message = "Byli jste odhlášeni z důvodu snížení Vašich uživatelský práv, opětovně se prosím přihlašte." });
            }
            catch (Exception e)
            {
                _LogError(actionType, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }

            ViewBag.Message = logMessage;
            var updatedUser = await GetUserView(userId);
            return View(updatedUser);
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("Nepodařilo se načíst údaje přihlášeného uživatele.");

                if (file == null || file.Length == 0)
                    return BadRequest("Soubor nebyl nahrán.");

                // validace typu
                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("Soubor není obrázek.");

                // načtení do byte[]
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                var image = new SystemImagesModel
                {
                    PhotoNameOriginal = file.FileName,
                    CreateAuthorId = result.userData.Id,
                    Size = imageBytes.Length,
                };

                var imageId = await _uploadFileService.UploadProfileImageAsync(image, imageBytes, result.userData.Id, IMAGES_FOLDER);

                if (imageId != 0)
                    await ChangeClaimItem.UpdateUserCustomClaimAsync(HttpContext, imageId.ToString(), CustomClaimTypes.ProfileImage);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Nastala chyba při ukládání fotografie.");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetProfileImage(int id)
        {
            try
            {
                var file = await _downloadFileService.GetProfileImage(id, IMAGES_FOLDER);

                return File(file, MineTypesTextEnum.WEBP);

            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Photo, "Nepodařilo se načíst plný náhled fotografie." + e.Message);
                return BadRequest("Nepodařilo se načíst náhled fotografie.");
            }
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [HttpPost("SetNewBackupKey")]
        public async Task<IActionResult> SetNewBackupKey([FromBody] BackupKeyRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            try
            {
                await _userService.UpdateBackupKey(new UserBackupKeyModel
                {
                    BackupKey = request.BackupKey,
                    Id = result.userData.Id
                });  
                
                _LogInfo(ActionTypeEnum.EditUserKeys, "Nastaven nový záchranný klíč.");
            }
            catch(Exception e)
            {
                _LogError(ActionTypeEnum.EditUserKeys, "Nepodařilo se uložit nový záchranný klíč." + e.Message);
                return BadRequest("Nepodařilo se uložit nový záchranný klíč.");
            }

            return Json(new { success = true });
        }

        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User},{UserRoles.Host}")]
        [HttpPost("ValidateBackupKey")]
        public async Task<IActionResult> ValidateBackupKey([FromBody] BackupKeyRequest request)
        {
            var result = CT_UserData();
            if (result.redirect != null)
                return result.redirect;

            try
            {
                var user = await _userService.Get(result.userData.Id);

                if(PasswordEncryption.VerifyPassword(request.BackupKey, user.BackupKey))
                {
                    _LogInfo(ActionTypeEnum.EditUserKeys, "Provedena úspěšná validace záchranného klíče.");
                    return Json(new { success = true });
                }
                else
                {
                    _LogInfo(ActionTypeEnum.EditUserKeys, "Provedena neúspěšná validace záchranného klíče.");
                    return Json(new { success = false });
                }
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.EditUserKeys, "Nepodařilo se validovat záchranný klíč." + e.Message);
                return BadRequest("Nepodařilo se validovat záchranný klíč.");
            } 
        }

        private async Task<UserView>? GetUserView(int userId)
        {
            // Načtení uživateslkých skupin
            var groups = (await _groupService.GetList()) ?? new List<UserGroupModel?>();

            var user = await _userService.Get(userId);
            var redactorName = "";
            var redactorId = 0;

            if (user == null) return null;
            
            // Poslední změnu profilu provedl sám uživatel
            if (user?.Id == user?.CreateAuthor)
            {
                redactorName = user.GetName();
                redactorId = user.Id;
            }
            else
            {
                var redactor = await _userService.Get(user.CreateAuthor);
                redactorName = redactor == null ? "" : redactor.GetName();
            }

            return user.ToUserView(redactorId, redactorName, groups);
        }
    }
}
