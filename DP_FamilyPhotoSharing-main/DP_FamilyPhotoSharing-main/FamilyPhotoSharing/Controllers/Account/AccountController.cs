using EncryptionLayer.Password;
using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using System.Security.Claims;

namespace FamilyPhotoSharing.Controllers.Accounts
{
    public class AccountController : BaseController
    {
        private readonly IGroupService _groupService;

        public AccountController(IMemoryCache cache, ISystemLogService log, IUserService userService, 
            IGroupService groupService) : base(cache, log, userService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult Login(string? message = null, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.Type = MessageTypes.Warning;
                ViewBag.LoginMessage = "Pro zobrazení této stránky je nutné se přihlásit!";
                ViewBag.ReturnUrl = returnUrl;
            }

            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Type = MessageTypes.Success; 
                ViewBag.Message = message;
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userLogin, string password, string returnUrl)
        {
            try
            {
                var user = await _userService.GetUserByLogin(userLogin);

                if (user?.UserLogin == userLogin && PasswordEncryption.VerifyPassword(password, user.UserPasswordHash) && (user?.Active ?? false) && (user?.Activated ?? false))
                {
                    var principal = await GetClaim(user);

                    // Přihlášení pomocí cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    _LogInfo(ActionTypeEnum.LoginUser, "Přihlášení uživatele.");

                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl ?? "/");

                    return RedirectToAction("Index", "Home");
                }

                // Chybová hláška
                ViewBag.Type = MessageTypes.Danger;
                if (user != null && !user.Active && user.UserLogin == userLogin)
                {
                    ViewBag.Message = "Uživatel je deaktivován.";
                    _LogError(ActionTypeEnum.LoginUser, "Pokus o přihlášení deaktivovaného uživatele.");
                }
                else
                {
                    ViewBag.Message = "Neplatné přihlašovací údaje.";
                    _LogError(ActionTypeEnum.LoginUser, "Neplatné přihlašovací údaje.");
                }   

                return View();
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.LoginUser, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(UserBackupKeyModel userBackupKeyModel)
        {
            try
            {
                var user = await _userService.GetUserByLogin(userBackupKeyModel.Login);

                if (
                    !string.IsNullOrWhiteSpace(userBackupKeyModel.BackupKey) && 
                    !string.IsNullOrWhiteSpace(userBackupKeyModel.NewPassword) && 
                    user?.UserLogin == userBackupKeyModel.Login && 
                    PasswordEncryption.VerifyPassword(userBackupKeyModel.BackupKey, user.BackupKey) && 
                    (user?.Active ?? false) && 
                    (user?.Activated ?? false))
                {
                    // Nastavit nové heslo
                    await _userService.UpdatePassword(new UserPasswordModel
                    {
                        Id = user.Id,
                        NewPassword = userBackupKeyModel.NewPassword,
                    });

                    var principal = await GetClaim(user);

                    // Přihlášení pomocí cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    _LogInfo(ActionTypeEnum.LoginUser, "Přihlášení uživatele.");

                    return RedirectToAction("Index", "Home");
                }

                // Chybová hláška
                ViewBag.Type = MessageTypes.Danger;
                ViewBag.Message = "Neplatné přihlašovací údaje. Chybný login, záchranný klíč. (Nebo byl uživatele v systému deaktivován)";
                _LogError(ActionTypeEnum.LoginUser, "Neplatné přihlašovací údaje.");

                return View();
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.LoginUser, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        public IActionResult AccessDained()
        {
            return View();
        }
        public IActionResult Logout(string message = "")
        {
            if (string.IsNullOrEmpty(message)) message = "Vaše odhlášení proběhlo úspěšně.";

            _LogInfo(ActionTypeEnum.LogoutUser, "");
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account", new { message });
        }

        private async Task<ClaimsPrincipal> GetClaim(UserModel user)
        {
            var group = await _groupService.Get(user.GroupId);

            // Vytvoření identity
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, UserRoles.GetRoleById(user.RoleId)),
                        new Claim(ClaimTypes.PrimaryGroupSid, user.GroupId.ToString()),
                        new Claim(CustomClaimTypes.FolderName, group?.FolderName ?? ""),
                        new Claim(CustomClaimTypes.GroupName, group?.GroupName ?? ""),
                        new Claim(CustomClaimTypes.ProfileImage, user?.SystemImagesId.ToString()),
                    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Vytvoření principalu
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
