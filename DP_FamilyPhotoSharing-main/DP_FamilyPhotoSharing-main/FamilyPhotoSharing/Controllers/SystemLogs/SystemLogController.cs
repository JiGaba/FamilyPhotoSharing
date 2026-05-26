using DataAccessLayer.Dao.Groups;
using FamilyPhotoSharing.Controllers.Base;
using FamilyPhotoSharing.Requests;
using FamilyPhotoSharing.ViewsData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using System.Collections;
using System.Reflection;

namespace FamilyPhotoSharing.Controllers.Logs
{
    public class SystemLogController : BaseController
    {
        private readonly IGroupService _groupService;
        private readonly ISystemLogService _systemLogService;
        public SystemLogController(IMemoryCache cache, ISystemLogService log, IUserService userService, 
            IGroupService groupService, ISystemLogService systemLogService) : base(cache, log, userService)
        {
            _groupService = groupService;
            _systemLogService = systemLogService;
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var groups = await _groupService.GetList();
                var users = await _userService.GetList();
                var totalCount = await _systemLogService.IGetCountByParameters(0, 0, 0, 0);
                var groupList = await LoadGroups(groups, 0);
                var userList = await LoadUsers(users, 0);
                var logs = await _systemLogService.IGetListByParameters(0, 0, 0, 0, 0, 20);

                var log = new LogView
                {
                    ActionTypes = ViewHelper.ToSelectList<ActionTypeEnum>(0),
                    LogTypes = ViewHelper.ToSelectList<LogTypeEnum>(0),
                    Groups = groupList,
                    Users = userList,
                    IsUserSelected = false,
                    Limit = 20,
                    Offset = 0,
                    Total = totalCount,
                    LastIsUserSelected = false,
                    LastSelectedUserId = 0,
                    LastSelectedLogType = 0,
                    LastSelectedGroupId = 0,
                    LastSelectedActionType = 0,
                };

                var logsView = logs.Select(l => l.ToSystemLogView(groups, users)).ToList();
                log.TableData = logsView;

                return View("Logs", log);
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
        public async Task<IActionResult> LoadNewFilters(LogView logResult)
        {
            try
            {
                var userId = logResult.IsUserSelected ? logResult.SelectedUserId ?? 0 : 0;
                var groupId = logResult.IsUserSelected ? 0 : logResult.SelectedGroupId ?? 0;
                var actionTypeId = logResult.SelectedActionType ?? 0;
                var logType = logResult.SelectedLogType ?? 0;

                var groups = await _groupService.GetList();
                var users = await _userService.GetList();
                var totalCount = await _systemLogService.IGetCountByParameters(userId, groupId, actionTypeId, logType);
                var groupList = await LoadGroups(groups, groupId);
                var userList = await LoadUsers(users, userId);
                var logs = await _systemLogService.IGetListByParameters(userId, groupId, actionTypeId, logType, 0, 20);

                var log = new LogView
                {
                    ActionTypes = ViewHelper.ToSelectList<ActionTypeEnum>(actionTypeId),
                    LogTypes = ViewHelper.ToSelectList<LogTypeEnum>(logType),
                    Groups = groupList,
                    Users = userList,
                    IsUserSelected = false,
                    Limit = 20,
                    Offset = 0,
                    Total = totalCount,
                    LastIsUserSelected = logResult.IsUserSelected,
                    LastSelectedActionType = logResult.SelectedActionType ?? 0,
                    LastSelectedGroupId = logResult.SelectedGroupId ?? 0,
                    LastSelectedLogType = logResult.SelectedLogType ?? 0,
                    LastSelectedUserId = logResult.SelectedUserId ?? 0,
                };

                var logsView = logs.Select(l => l.ToSystemLogView(groups, users)).ToList();
                log.TableData = logsView;

                return View("Logs", log);
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
        public async Task<IActionResult> LoadLogs([FromBody] LogRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request is null");

                var groups = await _groupService.GetList();
                var users = await _userService.GetList();
                var logs = await _systemLogService.IGetListByParameters(request.UserId ?? 0, request.GroupId ?? 0, request.ActionType ?? 0, request.LogType ?? 0, request.Offset, request.Limit);

                var logsView = logs.Select(l => l.ToSystemLogView(groups, users)).ToList();

                return Json(logsView);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.Other, e.Message);
                TempData["ErrorMessage"] = e.Message;
                return BadRequest("Stale se chyba při načítání logu.");
            } 
        }


        private async Task<List<SelectListItem>> LoadGroups(List<UserGroupModel> groups, int lastId)
        {
            var groupList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Vše"} };

            if(groups != null)
                groupList.AddRange(
                    groups.Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.GroupName,
                        Selected = (g.Id == lastId)
                    })
                );

            return groupList;
        }

        private async Task<List<SelectListItem>> LoadUsers(List<UserModel> users, int lastId)
        {
            var userList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Vše" } };

            if(users != null)
                userList.AddRange(
                    users.Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.GetName(),
                        Selected = (g.Id == lastId)
                    })
                );

            return userList;
        }
    }
}
