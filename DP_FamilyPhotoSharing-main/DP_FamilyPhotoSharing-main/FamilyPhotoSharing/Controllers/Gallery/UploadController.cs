using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Logs;
using System.Security.Claims;

namespace FamilyPhotoSharing.Controllers.Gallery
{
    public class UploadController : BaseController
    {
        private readonly IUploadFileService _uploadFileService;
        private readonly IConfiguration _config;

        public UploadController(IUploadFileService uploadFileService, IConfiguration config, 
            IMemoryCache cache, ISystemLogService log, IUserService userService) : base(cache, log, userService)
        {
            _uploadFileService = uploadFileService;
            _config = config;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> UploadSingle(IFormFile file, bool personal = true)
            => await UploadMain(file, personal);

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> UploadMultiple(IFormFile file, bool personal = false)
            => await UploadMain(file, personal);

        private async Task<IActionResult> UploadMain(IFormFile file, bool personal)
        {
            var maxLimit = _config.GetValue<int>("FILE_MAX_SIZE");
            double sizeInMB = (double)maxLimit / (1024 * 1024);

            if (file == null || file.Length == 0)
                return BadRequest("Soubor není validní.");

            if (file.Length > maxLimit) // 50 MB Limit
                return BadRequest($"Soubor nelze nahrát. Velikost souboru je větší než {sizeInMB:F2} MB.");

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ||
                !int.TryParse(User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value, out int groupId))
                return BadRequest($"Pro vkládání souborů musíte být přihlášen!");

            if (string.IsNullOrEmpty(User.FindFirst(CustomClaimTypes.FolderName)?.Value))
                return BadRequest($"Skupina ve které je uživatel přihlášen nemá správně nastavenou složku pro ukládání, kontaktujte administrátora systému!");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var mimeType = file.ContentType.ToLower();

            if (!TextEnumHelper.AllowedMimeTypeAndExtension(mimeType, extension))
                return BadRequest($"Nepodporovaný typ souboru!");

            string photoName = file.FileName.Length > 10
                ? file.FileName.Substring(file.FileName.Length - 10)
                : file.FileName;

            var photo = new PhotoModel
            {
                PhotoName = photoName,
                FileSize = (int)file.Length,
                CreateAuthor = userId,
                OwnerId = userId,
                Personal = personal,
                CreateDateTime = DateTime.Now,
                PhotoDescription = string.Empty,
                GroupsId = groupId,
            };
          
            using (Stream stream = file.OpenReadStream())
            {
                try
                {
                    if(personal)
                        await _uploadFileService.UploadFileAsync(photo, stream, User.FindFirst(CustomClaimTypes.FolderName)?.Value ?? "");
                    else
                        await _uploadFileService.UploadFileForFamilyAsync(photo, stream, User.FindFirst(CustomClaimTypes.FolderName)?.Value ?? "");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Ok();
        }
    }
}
