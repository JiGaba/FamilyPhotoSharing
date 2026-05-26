using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using FileAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyPhotoSharing.Controllers.SystemData
{
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        public GroupController(IMemoryCache cache, ISystemLogService log, IGroupService groupService, IUserService userService)
            : base(cache, log, userService)
        {
            _groupService = groupService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UserGroups()
        {
            try
            {
                var groups = await _groupService.GetList();
                var groupView = new List<UserGroupView>();

                foreach (var group in groups)
                {
                    var userCount = await _userService.GetUserCountByGroupId(group.Id);
                    var user = await _userService.Get(group.CreateAuthor);
                    groupView.Add(group.ToUserGroupView(userCount, user?.GetName()));
                }

                _LogInfo(ActionTypeEnum.Group, "Zobrazení všech rodin.");

                return View(groupView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Group, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UserGroup(int id)
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                if (id == 0)
                {
                    return View(new UserGroupView
                    {
                        GroupDescription = string.Empty,
                        GroupName = string.Empty,
                    });
                }
                else
                {
                    var group = await _groupService.Get(id);

                    if (group == null)
                        return _Redirect_Logout();

                    var redactor = await _userService.Get(result.userData.Id);
                    var redactorName = redactor == null ? "" : redactor.GetName();

                    _LogInfo(ActionTypeEnum.Group, $"Zobrazení všech rodiny {group.GroupName}.");

                    return View(group?.ToUserGroupView(redactorName));
                }
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Group, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserGroupView userGroup)
        {
            try
            {
                var message = "";
                var groupId = 0;

                if (userGroup == null)
                    return _Redirect_Logout();

                var result = CT_UserData();
                if (result.redirect != null)
                    return result.redirect;

                var groups = await _groupService.GetList();
                if(groups.Any(x => x.GroupName.Equals(userGroup.GroupName, StringComparison.OrdinalIgnoreCase) && userGroup.Id != x.Id))
                {
                    ViewBag.Type = MessageTypes.Warning;
                    ViewBag.Message = $"Uživatelská skupina {userGroup.GroupName} s tímto názvem již v systému existuje.";
                    return View("UserGroup", userGroup);
                }

                userGroup.CreateAuthor = result.userData.Id;

                if (userGroup?.Id == 0) // vytvoření nového profilu
                {
                    // vytvoření názvu složky pro uživatelský profil
                    userGroup.FolderName = FileHelper.GetFolderName(userGroup.GroupName);

                    groupId = await _groupService.Set(userGroup);
                    _LogInfo(ActionTypeEnum.AddGroup, $"Group ID: {groupId}, Název {userGroup.GroupName}");
                    message = $"uživatelská skupina {userGroup.GroupName} byla úspěšně vytvořena.";
                }
                else // editace stávajícího profilu
                {
                    groupId = userGroup.Id;
                    await _groupService.Update(userGroup);
                    _LogInfo(ActionTypeEnum.EditGroup, $"Group ID: {userGroup.Id}, Název {userGroup.GroupName}");
                    message = $"uživatelská skupina {userGroup.GroupName} byla úspěšně upravena.";
                }

                if (result.userData.GroupId == userGroup.Id)
                {
                    await ChangeClaimItem.UpdateUserCustomClaimAsync(HttpContext, userGroup.GroupName, CustomClaimTypes.GroupName);
                    ViewBag.RefreshFamily = userGroup.GroupName;
                }
                    

                ViewBag.Message = message;
                ViewBag.Type = MessageTypes.Success;
                var userGroupLoad = await _groupService.Get(groupId);
                var redactor = await _userService.Get(result.userData.Id);
                var redactorName = redactor == null ? "" : redactor.GetName();
                var userGroupView = userGroupLoad?.ToUserGroupView(redactorName);

                return View("UserGroup", userGroupView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Group, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/Error/500");
            }
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> DeleteProfile([FromBody] DeleteGroupRequest request)
        {
            try
            {
                await _groupService.DeleteGroupById(request.Id);
                _LogInfo(ActionTypeEnum.RemoveGroup, $"Group ID: {request.Id} byla smazána.");
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Group, e.Message);
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}
