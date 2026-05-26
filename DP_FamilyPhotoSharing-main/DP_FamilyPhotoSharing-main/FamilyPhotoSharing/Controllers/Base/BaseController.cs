using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FamilyPhotoSharing.Controllers.Base
{
    public class BaseController : Controller
    {
        protected const int CACHE_MINUTES = 5;

        protected const string CACHE_TOTAL_IMAGES = "totalImages_";

        protected const string IMAGES_FOLDER = "images";

        protected readonly ISystemLogService _log;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IUserService _userService;

        private List<int> OriginalIds;
        private List<int> UpdatedIds;
        public BaseController(IMemoryCache cache, ISystemLogService log, IUserService userService)
        {
            _memoryCache = cache;
            _log = log;
            _userService = userService;
        }
        // Id, které se nezměnily a jsou v obou seznamech
        protected List<int> _usr_UnchangedIds => OriginalIds.Intersect(UpdatedIds).ToList();
        // Id, které byly ve starém ale v novém již nejsou
        // Odebraní uživatelé
        protected List<int> _usr_RemovedIds => OriginalIds.Except(UpdatedIds).ToList();
        // Id, které ve starém seznamu nebyli, ale jsou v novém seznamu
        // Přidaní uživatelé
        protected List<int> _usr_AddedIds => UpdatedIds.Except(OriginalIds).ToList();
        protected void _LoadUsersIdList(string? originalIds, string? updatedIds)
        {
            OriginalIds = ParseIds(originalIds);
            UpdatedIds = ParseIds(updatedIds);  
        }
        protected void _LoadUsersIdList(List<int> originalIds, List<int> updatedIds)
        {
            OriginalIds = originalIds;
            UpdatedIds = updatedIds;
        }
        protected void _LogInfo(ActionTypeEnum actionType, string description)
            => Log(LogTypeEnum.Ok, actionType, description);

        protected void _LogError(ActionTypeEnum actionType, string description)
            => Log(LogTypeEnum.Error, actionType, description);
        protected void _LogAPIInfo(ActionTypeEnum actionType, string description)
            => Log(LogTypeEnum.OkAPI, actionType, description);

        protected void _LogAPIError(ActionTypeEnum actionType, string description)
            => Log(LogTypeEnum.ErrorAPI, actionType, description);

        protected string _CacheKeyTotalImages(ThumbnailData thumbnailData, int userId, int groupId, int albumId)
            => $"{CACHE_TOTAL_IMAGES}_{(int)thumbnailData}_{userId}_{groupId}_{albumId}";

        protected void _CacheInvalidate(string cacheKey)
            => _memoryCache.Remove(cacheKey);
        protected IActionResult _Redirect_Logout()
            => RedirectToAction("Logout", "Account", new { message = "Z důvodu bezpečnosti jste byli odhlášeni, opětovně se prosím přihlašte." });
        
        protected (IActionResult? redirect, SystemUsersDataModel? userData) CT_UserData()
        {
            UserRoleEnum roleEnum = UserRoleEnum.Host;

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                _LogError(ActionTypeEnum.User, "Id přihlášeného uživatele nelze převést na integer.");
                return (_Redirect_Logout(), null);
            }

            var roleId = UserRoles.GetRoleByName(User.FindFirst(ClaimTypes.Role)?.Value);
            if (roleId == 0)
            {
                _LogError(ActionTypeEnum.User, "Uživatelská role přihlášeného uživatele nelze převést na integer.");
                return (_Redirect_Logout(), null);
            }

            if (!int.TryParse(User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value, out int groupId))
            {
                _LogError(ActionTypeEnum.User, "GroupId přihlášeného uživatele nelze převést na integer.");
                return (_Redirect_Logout(), null);
            }

            var folderName = User.FindFirst(CustomClaimTypes.FolderName)?.Value;
            if (string.IsNullOrWhiteSpace(folderName))
            {
                _LogError(ActionTypeEnum.User, "Název složky skupiny přihlášeného uživatele nelze načíst.");
                return (_Redirect_Logout(), null);
            }

            var groupName = User.FindFirst(CustomClaimTypes.GroupName)?.Value;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _LogError(ActionTypeEnum.User, "Název rodiny přihlášeného uživatele nelze načíst.");
                return (_Redirect_Logout(), null);
            }

            if (!int.TryParse(User.FindFirst(CustomClaimTypes.ProfileImage)?.Value, out int imageId))
            {
                _LogError(ActionTypeEnum.User, "ProfileImageId přihlášeného uživatele nelze převést na integer.");
                return (_Redirect_Logout(), null);
            }

            try
            {
                roleEnum = EnumHelper.GetEnum<UserRoleEnum>(roleId);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.User, e.Message);
                return (_Redirect_Logout(), null);
            }

            return (null, new SystemUsersDataModel
            {
                Id = userId,
                GroupId = groupId,
                RoleId = roleId,
                RoleEnum = roleEnum,
                GroupName = groupName,
                Folder = folderName,
                ProfileImageId = imageId
            });
        }
        protected async Task<List<UserModalView>> GetUsersModalView(List<int> activeIds = null, int ownerId = 0)
        {
            var result = CT_UserData();
            if (result.redirect != null) 
                return new List<UserModalView>();

            var users = await _userService.SelectUserByGroupIdActiveRoleId(result.userData.GroupId, true, new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.GroupAdmin, UserRoleEnum.User, UserRoleEnum.Host });
            
            return users?.Select(u => u?.ToUserModalView(activeIds, ownerId)).ToList();
        }

        private List<int> ParseIds(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<int>();

            return input
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList();
        }
        private void Log(LogTypeEnum logType, ActionTypeEnum actionType, string description)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleName = User.FindFirst(ClaimTypes.Role)?.Value;
            var userGroupIdString = User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value;

            int.TryParse(userIdString, out int userId);
            int.TryParse(userGroupIdString, out int userGroupId);

            var log = new SystemLogModel
            {
                ActionType = actionType,
                LogType = logType,
                LogDescription = $"{description} [{userRoleName}]",
                CreateAuthorId = userId,
                GroupsId = userGroupId,
            };

            _log.Set(log);
        }
    }
}
