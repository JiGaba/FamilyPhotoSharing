using EncryptionLayer.Password;
using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.DbInitFileLoaderService;
using System.Diagnostics;
using System.Security.Claims;

namespace FamilyPhotoSharing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbInitFileLoaderService _dbInitFileLoaderService;
        private readonly IUserService _userService; 

        public HomeController(ILogger<HomeController> logger, 
            IDbInitFileLoaderService dbInitFileLoaderService, IUserService userService)
        {
            _logger = logger;
            _dbInitFileLoaderService = dbInitFileLoaderService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dbTest = await _dbInitFileLoaderService.CheckDbExistence();

                if (dbTest)
                {
                    var tableTest = false;

                    if (dbTest)
                        tableTest = await _dbInitFileLoaderService.CheckTableExists();

                    if (tableTest)
                    {
                        var role = User.FindFirst(ClaimTypes.Role)?.Value;
                        var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userIdTemp) ? userIdTemp : 0;

                        if(role == UserRoles.Admin || role == UserRoles.GroupAdmin || role == UserRoles.User)
                        {
                            return RedirectToAction("MainPersonalAlbum", "Gallery");
                        }
                        else
                        {
                            if(userId != 0)
                                return RedirectToAction("Profile", "User", new { userId });
                            else
                                return RedirectToAction("Login", "Account");
                        }
                    }
                        
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Register(AdminRegisterModel model)
        {
            try
            {
                var userModel = new UserModel
                {
                    UserDescription = string.Empty,
                    UserLogin = model.UserLogin,
                    UserName = model.UserName,
                    UserPasswordPlain = model.UserPasswordPlain,
                    UserSurname = model.UserSurname,
                    Activated = true,
                    Active = true,
                    BackupKey = "",
                    CreateAuthor = 1,
                    CreateDateTime = DateTime.Now,
                    GroupId = 1,
                    Id = 0,
                    RoleId = 1,
                    SystemImagesId = 0,
                    UserPasswordHash = ""
                };

                var userKeyModel = _userService.PrepareUserKeysModel(userModel);

                await _dbInitFileLoaderService.InitializeDatabase(userModel, userKeyModel);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/599");
            } 
        }

        public IActionResult Licence()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
