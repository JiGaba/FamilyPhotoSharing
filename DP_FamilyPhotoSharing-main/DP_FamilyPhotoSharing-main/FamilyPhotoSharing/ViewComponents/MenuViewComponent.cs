using FamilyPhotoSharing.AccountManagement;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.PhotoEnums;
using ModelLayer.System;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.PhotoAlbum;
using ServiceLayer.Services.Photos;
using ServiceLayer.Services.SharedPhotoAlbum;
using System.Security.Claims;

namespace FamilyPhotoSharing.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _http;
        private readonly ISystemLogService _logger;
        private readonly ISharedPhotoAlbumService _sharedPhotoAlbumService;
        private readonly IPhotoService _photoService;
        private readonly IPhotoAlbumService _photoAlbumService;
        public MenuViewComponent(IHttpContextAccessor http, ISystemLogService logger, ISharedPhotoAlbumService sharedPhotoAlbumService,
            IPhotoAlbumService photoAlbumService, IPhotoService photoService)
        {
            _http = http;
            _logger = logger;
            _photoAlbumService = photoAlbumService;
            _photoService = photoService;
            _sharedPhotoAlbumService = sharedPhotoAlbumService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var result = CT_UserData();
                if(result.userData == null || result.userData.Id == 0)
                    return Content("");

                var menuItems = result.userData.RoleEnum switch
                {
                    UserRoleEnum.Host => await GetHostMenu(result.userData.Id),
                    UserRoleEnum.User => await GetUserMenu(result.userData.Id, result.userData.GroupId),
                    UserRoleEnum.GroupAdmin => await GetGroupAdminMenu(result.userData.Id, result.userData.GroupId),
                    UserRoleEnum.Admin =>  await GetAdminMenu(result.userData.Id, result.userData.GroupId),
                    _ => throw new Exception($"Role uživatele {result.userData.RoleEnum.ToString()} se neshoduje s rolemi v systému."),
                };

                var currentController = RouteData.Values["controller"]?.ToString();
                var currentAction = RouteData.Values["action"]?.ToString();

                ViewBag.CurrentController = currentController;
                ViewBag.CurrentAction = currentAction;

                if(!result.userData.RoleEnum.Equals(UserRoleEnum.Host))
                    menuItems.Where(m => m.Title.Equals(MainMenuItemsName.Family))
                    .First()
                    .Title = result.userData.GroupName;

                return View(menuItems);
            }
            catch (Exception e)
            {
                return Content("Chyba při načítání dat.");
            }
        }

        private async Task<List<MenuSectionModel>> GetHostMenu(int userId) 
        {
            var menu = new List<MenuSectionModel>
            {
                new MenuSectionModel { Title = MainMenuItemsName.SharedWithMe, Picture = MenuPicture.SharedPhotoWithMe },
                new MenuSectionModel { Title = MainMenuItemsName.Settings, Picture = MenuPicture.Settings }
            };

            menu = await GetSharedAlbums(userId, menu);
            menu = await GetSettings_MyProfile(userId, menu);

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetUserMenu(int userId, int groupId) 
        {
            var menu = await GetMenuUserBase(userId, groupId);
            menu = await GetSettingsBase(userId, menu);

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetGroupAdminMenu(int userId, int groupId) 
        {
            var menu = await GetMenuUserBase(userId, groupId);
            menu = await GetSettingsGroupAdmin(userId, menu);

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetAdminMenu(int userId, int groupId) 
        {
            var menu = await GetMenuUserBase(userId, groupId);
            menu = await GetSettingsAdmin(userId, menu);
            menu = await GetLogs(userId, menu);

            return menu;
        }

        private async Task<List<MenuSectionModel>> GetMenuUserBase(int userId, int groupId)
        {
            var menu = new List<MenuSectionModel>
            {
                new MenuSectionModel {Title = MainMenuItemsName.Personal, Picture = MenuPicture.PrivateGallery },
                new MenuSectionModel {Title = MainMenuItemsName.Family, Picture = MenuPicture.PublicGallery},
                new MenuSectionModel {Title = MainMenuItemsName.Shared, Picture = MenuPicture.SharedPhoto},
                new MenuSectionModel {Title = MainMenuItemsName.SharedWithMe, Picture = MenuPicture.SharedPhotoWithMe},
                new MenuSectionModel {Title = MainMenuItemsName.Settings, Picture = MenuPicture.Settings},
            };

            menu = await GetPersonalAlbums(userId, menu);
            menu = await GetGroupAlbums(groupId, menu);
            menu = await GetMySharedAlbums(userId, menu);
            menu = await GetSharedAlbums(userId, menu);

            return menu;
        }

        private async Task<List<MenuSectionModel>> GetPersonalAlbums(int userId, List<MenuSectionModel> menu)
        {
            var personalAlbums = await _photoAlbumService.GetByOwnerId(userId);

            var albumMenuItems = new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Title = "Soukromá sbírka",
                    Controller = "Gallery",
                    Action = "MainPersonalAlbum",
                    Picture = MenuPicture.Empty
                }
            };

            if (personalAlbums != null && personalAlbums.Any())
            {
                foreach (var personalAlbum in personalAlbums)
                {
                    albumMenuItems.Add(new MenuItemModel
                    {
                        Title = personalAlbum.AlbumName,
                        Action = $"ShowAlbum",
                        Controller = "Gallery",
                        RouteParamValue = personalAlbum.Id,
                        RouteParamName = "albumId",
                        Picture = MenuPicture.Empty,
                    });
                }
            }

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Personal))
                .First()
                .SubItems = albumMenuItems;

            return menu;
        }

        private async Task<List<MenuSectionModel>> GetGroupAlbums(int groupId, List<MenuSectionModel> menu)
        {
            var groupAlbums = await _photoAlbumService.GetByGroupsId(groupId);

            var albumMenuItems = new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Title = "Rodinná sbírka",
                    Controller = "Gallery",
                    Action = "MainGroupAlbum",
                    Picture = MenuPicture.Empty
                },
                new MenuItemModel
                {
                    Title = "Rodinná sbírka - [vlastní]",
                    Controller = "Gallery",
                    Action = "MainGroupAlbumOwnPhoto",
                    Picture = MenuPicture.Empty
                }
            };
            
            if (groupAlbums != null && groupAlbums.Any())
            {
                foreach (var groupAlbum in groupAlbums)
                {
                    albumMenuItems.Add(new MenuItemModel
                    {
                        Title = groupAlbum.AlbumName,
                        Action = $"ShowGroupAlbum",
                        Controller = "Gallery",
                        RouteParamName = "albumId",
                        RouteParamValue = groupAlbum.Id,
                        Picture = MenuPicture.Empty,
                    });
                }
            }

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Family))
                .First()
                .SubItems = albumMenuItems;

            return menu;
        }

        private async Task<List<MenuSectionModel>> GetMySharedAlbums(int userId, List<MenuSectionModel> menu) 
        {
            var sharedAlbums = await _sharedPhotoAlbumService.GetByOwnerId(userId);
            var sharedAlbumMenuItems = new List<MenuItemModel>();

            if (sharedAlbums != null && sharedAlbums.Any())
            {
                foreach (var sharedAlbum in sharedAlbums)
                {
                    sharedAlbumMenuItems.Add(new MenuItemModel
                    {
                        Title = sharedAlbum.AlbumName,
                        Action = $"ShowMySharedAlbum",
                        Controller = "Gallery",
                        RouteParamName = "albumId",
                        RouteParamValue = sharedAlbum.Id,
                        Picture = MenuPicture.Empty,
                    });
                }
            }

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Shared))
                .First()
                .SubItems = sharedAlbumMenuItems;

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetSharedAlbums(int userId, List<MenuSectionModel> menu) 
        {
            var sharedAlbums = await _sharedPhotoAlbumService.GetByHostId(userId);
            var sharedAlbumMenuItems = new List<MenuItemModel>();

            if (sharedAlbums != null && sharedAlbums.Any())
            {
                foreach (var sharedAlbum in sharedAlbums)
                {
                    sharedAlbumMenuItems.Add(new MenuItemModel
                    {
                        Title = sharedAlbum.AlbumName,
                        Action = $"ShowSharedAlbum",
                        Controller = "Gallery",
                        RouteParamName = "albumId",
                        RouteParamValue = sharedAlbum.Id,
                        Picture = MenuPicture.Empty,
                    });
                }
            }

            menu.Where(m => m.Title.Equals(MainMenuItemsName.SharedWithMe))
                .First()
                .SubItems = sharedAlbumMenuItems;

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetSettingsBase(int userId, List<MenuSectionModel> menu) 
        {
            var settings = await GetSettings_MyProfile(userId, menu);
            var settingsItems = new List<MenuItemModel>
            {
                new MenuItemModel { Title = "Soukromá fotoalba", Controller = "Album", Action = "PersonalPhotoAlbums", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Sdílená fotoalba (vlastní)", Controller = "SharedAlbum", Action = "MySharedPhotoAlbums", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Sdílená fotoalba", Controller = "SharedAlbum", Action = "SharedPhotoAlbums", Picture = MenuPicture.Empty}
            };

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Settings))
                .First()
                .SubItems.AddRange(settingsItems);

            return menu;
        }

        private async Task<List<MenuSectionModel>> GetSettingsGroupAdmin(int userId, List<MenuSectionModel> menu)
        {
            menu = await GetSettingsBase(userId, menu);
            var settingsItems = new List<MenuItemModel>
            {
                new MenuItemModel { Title = "Rodinná fotoalba", Controller = "Album", Action = "GroupPhotoAlbums", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Uživatelé", Controller = "User", Action = "UserProfiles", Picture = MenuPicture.Empty}
            };

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Settings))
                .First()
                .SubItems.AddRange(settingsItems);

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetSettingsAdmin(int userId, List<MenuSectionModel> menu)
        {
            var settings = await GetSettingsBase(userId, menu);
            var settingsItems = new List<MenuItemModel>
            {
                new MenuItemModel { Title = "Rodinná fotoalba", Controller = "Album", Action = "GroupPhotoAlbums", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Uživatelé", Controller = "User", Action = "UserProfiles", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Rodiny", Controller = "Group", Action = "UserGroups", Picture = MenuPicture.Empty},
            };

            menu.Where(m => m.Title.Equals(MainMenuItemsName.Settings))
                .First()
                .SubItems.AddRange(settingsItems);

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetLogs(int userId, List<MenuSectionModel> menu) 
        {
            var logsItems = new List<MenuItemModel>
            {
                new MenuItemModel { Title = "Systémový log", Controller = "SystemLog", Action = "Index", Picture = MenuPicture.Empty},
                new MenuItemModel { Title = "Stav systému", Controller = "Access", Action = "CheckAccess", Picture = MenuPicture.Empty}
            };

            menu.Add(new MenuSectionModel { Title = MainMenuItemsName.Logs, Picture = MenuPicture.Logs });
            menu.Where(m => m.Title.Equals(MainMenuItemsName.Logs))
                .First()
                .SubItems = logsItems;

            return menu;
        }
        private async Task<List<MenuSectionModel>> GetSettings_MyProfile(int userId, List<MenuSectionModel> menu)
        {
            menu.Where(m => m.Title.Equals(MainMenuItemsName.Settings))
                .First()
                .SubItems = new List<MenuItemModel> { new MenuItemModel
                {
                    Title = "Uživatelský profil",
                    Controller = "User",
                    Action = "Profile",
                    RouteParamName = "userId",
                    RouteParamValue = userId,
                    Picture = MenuPicture.Empty,
                }};

            return menu;
        }
        private (Exception?, SystemUsersDataModel? userData) CT_UserData()
        {
            UserRoleEnum roleEnum = UserRoleEnum.Host;

            if (!int.TryParse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                _LogError(ActionTypeEnum.User, "Menu - Id přihlášeného uživatele nelze převést na integer.");
                return (new Exception("Id přihlášeného uživatele nelze převést na integer."), null);
            }

            var roleId = UserRoles.GetRoleByName(_http.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value);
            if (roleId == 0)
            {
                _LogError(ActionTypeEnum.User, "Menu - Uživatelská role přihlášeného uživatele nelze převést na integer.");
                return (new Exception("Uživatelská role přihlášeného uživatele nelze převést na integer."), null);
            }

            if (!int.TryParse(_http.HttpContext.User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value, out int groupId))
            {
                _LogError(ActionTypeEnum.User, "Menu - GroupId přihlášeného uživatele nelze převést na integer.");
                return (new Exception("GroupId přihlášeného uživatele nelze převést na integer."), null);
            }

            var folderName = _http.HttpContext.User.FindFirst(CustomClaimTypes.FolderName)?.Value;
            if (string.IsNullOrWhiteSpace(folderName))
            {
                _LogError(ActionTypeEnum.User, "Menu - Název složky rodiny přihlášeného uživatele nelze načíst.");
                return (new Exception("Název složky rodiny přihlášeného uživatele nelze načíst."), null);
            }

            var groupName = _http.HttpContext.User.FindFirst(CustomClaimTypes.GroupName)?.Value;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _LogError(ActionTypeEnum.User, "Menu - Název rodiny přihlášeného uživatele nelze načíst.");
                return (new Exception("Název rodiny přihlášeného uživatele nelze načíst."), null);
            }

            try
            {
                roleEnum = EnumHelper.GetEnum<UserRoleEnum>(roleId);
            }
            catch (Exception e)
            {
                _LogError(ActionTypeEnum.User, e.Message);
                return (e, null);
            }

            return (null, new SystemUsersDataModel
            {
                Id = userId,
                GroupId = groupId,
                RoleId = roleId,
                RoleEnum = roleEnum,
                Folder = folderName,
                GroupName = groupName
            });
        }
        protected void _LogError(ActionTypeEnum actionType, string description)
            => Log(LogTypeEnum.Error, actionType, description);
        private void Log(LogTypeEnum logType, ActionTypeEnum actionType, string description)
        {
            var userIdString = _http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleName = _http.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var userGroupIdString = _http.HttpContext.User.FindFirst(ClaimTypes.PrimaryGroupSid)?.Value;

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

            _logger.Set(log);
        }
    }

}
