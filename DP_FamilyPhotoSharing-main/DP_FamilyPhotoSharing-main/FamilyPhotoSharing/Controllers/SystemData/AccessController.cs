using EncryptionLayer.Photo;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.DbInitFileLoaderService;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Logs;

namespace FamilyPhotoSharing.Controllers.SystemData
{
    public class AccessController : BaseController
    {
        private readonly IDbInitFileLoaderService _dbInitFileLoaderService;
        private readonly IUploadFileService _uploadFileService;
        private readonly ICryptoService _cryptoService;
        public AccessController(IDbInitFileLoaderService dbInitFileLoaderService, IMemoryCache cache, ISystemLogService log, 
            IUserService userService, IUploadFileService uploadFileService, ICryptoService cryptoService) : base(cache, log, userService)
        {
            _dbInitFileLoaderService = dbInitFileLoaderService;
            _uploadFileService = uploadFileService;
            _cryptoService = cryptoService;
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<IActionResult> CheckAccess()
        {
            try
            {
                var dbConn = await _dbInitFileLoaderService.CheckDbExistence();
                var tableConn = await _dbInitFileLoaderService.CheckTableExists();
                var keyExists = _cryptoService.KeyExists();

                var model = new CheckAccessModal
                {
                    CheckDbConn = dbConn,
                    CheckTableConn = tableConn,
                    CheckKeyConn = keyExists,
                };

                _LogInfo(ActionTypeEnum.Other, "Zobrazení stavu systému.");

                return View(model);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Other, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }  
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpPost]
        public async Task<IActionResult> GenerateTestFile()
        {
            try
            {
                await _uploadFileService.CreateTestFile();

                _LogInfo(ActionTypeEnum.Other, $"Vygenerován testovací soubor.");

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Other, e.Message);
                return BadRequest("Generování souboru se nezdařilo.");
            }
        }
    }
}
