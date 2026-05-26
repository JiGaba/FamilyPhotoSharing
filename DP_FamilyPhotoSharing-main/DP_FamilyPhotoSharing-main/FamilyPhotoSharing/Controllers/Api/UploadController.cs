using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.ApiRequests;
using FamilyPhotoSharing.ApiResponse;
using FamilyPhotoSharing.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Files;
using ServiceLayer.Services.Logs;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : BaseController
    {
        private readonly IUploadFileService _uploadFileService;
        private readonly IConfiguration _config;

        public UploadController(IUploadFileService uploadFileService, IConfiguration config, IMemoryCache cache, ISystemLogService log, IUserService userService) : base(cache, log, userService)
        {
            _uploadFileService = uploadFileService;
            _config = config;
        }

        [HttpPost("UploadPersonal")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> UploadPersonal([FromForm] IFormFile file)
        {
            try
            {
                var photo = PrepareUpload(file, true);

                using (Stream stream = file.OpenReadStream())
                {
                    try
                    {
                        await _uploadFileService.UploadFileForFamilyAsync(photo, stream, User.FindFirst(CustomClaimTypes.FolderName)?.Value ?? "");
                        _LogAPIInfo(ActionTypeEnum.Photo, "Fotografie byla úspěšně nahrána do systému.");
                        return Ok(ApiResponse<bool>.Ok(true, $"Fotografie byla úspěšně nahrána do systému."));
                    }
                    catch (Exception e)
                    {
                        _LogAPIError(ActionTypeEnum.Photo, "Fotografie nebyla nahrána do systému.");
                        return Ok(ApiResponse<string>.Fail($"Fotografie nebyla nahrána do systému."));
                    }
                }
            }
            catch (Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, "Chyba při nahrávání fotografie do systému.");
                return Ok(ApiResponse<string>.Fail(e.Message));
            }
        }

        [HttpPost("UploadGroup")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> UploadGroup([FromForm] IFormFile file)
        {
            try
            {
                var photo = PrepareUpload(file, false);

                using (Stream stream = file.OpenReadStream())
                {
                    try
                    {
                        await _uploadFileService.UploadFileForFamilyAsync(photo, stream, User.FindFirst(CustomClaimTypes.FolderName)?.Value ?? "");
                        _LogAPIInfo(ActionTypeEnum.Photo, "Fotografie byla úspěšně nahrána do systému.");
                        return Ok(ApiResponse<bool>.Ok(true, $"Fotografie byla úspěšně nahrána do systému."));
                    }
                    catch (Exception e)
                    {
                        _LogAPIError(ActionTypeEnum.Photo, "Fotografie nebyla nahrána do systému.");
                        return Ok(ApiResponse<string>.Fail($"Fotografie nebyla nahrána do systému."));
                    }
                }
            }
            catch(Exception e)
            {
                _LogAPIError(ActionTypeEnum.Photo, "Chyba při nahrávání fotografie do systému.");
                return Ok(ApiResponse<string>.Fail(e.Message));
            }      
        }

        private PhotoModel PrepareUpload(IFormFile file, bool personal)
        {

            var maxLimit = _config.GetValue<int>("FILE_MAX_SIZE");
            double sizeInMB = (double)maxLimit / (1024 * 1024);

            if (file == null || file.Length == 0)
                throw new Exception("Soubor není validní.");

            if (file.Length > maxLimit) // 50 MB Limit
                throw new Exception("Soubor nelze nahrát. Velikost souboru je větší než {sizeInMB:F2} MB.");

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ||
                !int.TryParse(User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value, out int groupId))
                throw new Exception("Pro vkládání souborů musíte být přihlášen!");

            if (string.IsNullOrEmpty(User.FindFirst(CustomClaimTypes.FolderName)?.Value))
                throw new Exception("Skupina ve které je uživatel přihlášen nemá správně nastavenou složku pro ukládání, kontaktujte administrátora systému!");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var mimeType = file.ContentType.ToLower();

            if (!TextEnumHelper.AllowedMimeTypeAndExtension(mimeType, extension))
                throw new Exception("Nepodporovaný typ souboru!");

            string photoName = file.FileName.Length > 10
                ? file.FileName.Substring(file.FileName.Length - 10)
                : file.FileName;

            return new PhotoModel
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
        }

    }
}
