using DataAccessLayer.Dao.UserRefreshToken;
using EncryptionLayer.Password;
using FamilyPhotoSharing.AccountManagement;
using FamilyPhotoSharing.ApiRequests;
using FamilyPhotoSharing.ApiResponse;
using FamilyPhotoSharing.Controllers.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Data;
using ModelLayer.Enums;
using ModelLayer.System;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Groups;
using ServiceLayer.Services.Logs;
using ServiceLayer.Services.UserRefreshToken;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IGroupService _groupService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthController(IConfiguration config, IMemoryCache cache, ISystemLogService log, IUserService userService, IGroupService groupService, IRefreshTokenService refreshTokenService) : base(cache, log, userService)
        {
            _config = config;
            _groupService = groupService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] LoginRequest request) 
        {
            try
            {
                var user = await _userService.GetUserByLogin(request.Login);

                if (user == null)
                {
                    _LogAPIError(ActionTypeEnum.LoginUser, "Neplatné přihlašovací údaje.");
                    return Unauthorized(new { error = "Neplatné přihlašovací údaje." });
                }

                var group = await _groupService.Get(user.GroupId);

                if (user?.UserLogin == request.Login && PasswordEncryption.VerifyPassword(request.Password, user.UserPasswordHash) && (user?.Active ?? false) && (user?.Activated ?? false) && user.RoleId != 4)
                {
                    var jwt = GenerateToken(user, group);
                    var refresh = GenerateRefreshToken();

                    _LogAPIInfo(ActionTypeEnum.LoginUser, "Přihlášení uživatele.");

                    refresh.CreateAuthorId = user.Id;
                    refresh.UserId = user.Id;
                    await _refreshTokenService.Update(new RefreshTokenModel { UserId = user.Id});
                    await _refreshTokenService.Set(refresh);

                    _LogAPIInfo(ActionTypeEnum.AddRefreshToken, "Přidání nového refresh tokenu.");

                    return Ok(new 
                    { 
                        token = jwt,
                        refreshToken = refresh
                    });
                }

                if (user != null && user.RoleId == 4)
                    _LogAPIError(ActionTypeEnum.LoginUser, "Uživatel s rolí host nemá přístup k API.");
                else if (user != null && !user.Active && user.UserLogin == request.Login)
                    _LogAPIError(ActionTypeEnum.LoginUser, "Pokus o přihlášení deaktivovaného uživatele.");
                else
                    _LogAPIError(ActionTypeEnum.LoginUser, "Neplatné přihlašovací údaje.");

                return Unauthorized("Vaše přihlašovací údaje nebyl správné nebo nemáte oprávnění přístupu k API");
            }
            catch (Exception e)
            {
                
                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<UserModel>.Fail($"Nepodařilo provést úspěšné přihlášení uživatele."));
            } 
        }

        [HttpGet("getuserinfo")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.GroupAdmin},{UserRoles.User}")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var result = CT_UserData();
                if (result.redirect != null)
                    return BadRequest("U přihlášeného uživatele nejsou uloženy potřebné údaje.");

                var user = await _userService.Get(result.userData.Id);

                var response = new UserInfoResponse
                {
                    GroupName = result.userData.GroupName,
                    Login = user?.UserLogin,
                    Name = user?.UserName,
                    Surname = user?.UserSurname,
                    RoleId = user.RoleId,
                };

                _LogAPIInfo(ActionTypeEnum.User, $"Předány informace o uživateli s id {result.userData.Id}.");

                return Ok(ApiResponse<UserInfoResponse>.Ok(response, $"Předány informace o uživateli s id {result.userData.Id}."));
            }
            catch (Exception e)
            {

                _LogAPIError(ActionTypeEnum.LoginUser, e.Message);
                return Ok(ApiResponse<UserInfoResponse>.Fail($"Nepodařilo se předat informace o přihlášeném uživateli."));
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var stored = await _refreshTokenService.SelectByToken(request.Token);

            if (stored == null || stored.IsRevoked || stored.Expires < DateTime.UtcNow)
            {
                _LogAPIError(ActionTypeEnum.LoginUser, $"Uživatel se pokusil přihlásit neplatným tokenem {request.Token}.");
                return Unauthorized("Neplatný nebo expirovaný refresh token.");
            }

            var user = await _userService.Get(stored.UserId);
            var group = await _groupService.Get(user.GroupId);

            _LogAPIInfo(ActionTypeEnum.LoginUser, "Přihlášení uživatele.");

            stored.IsRevoked = true;
            await _refreshTokenService.Update(stored);

            var newJwt = GenerateToken(user, group);
            var newRefresh = GenerateRefreshToken();

            newRefresh.CreateAuthorId = user.Id;
            newRefresh.UserId = user.Id;
            await _refreshTokenService.Set(newRefresh);

            _LogAPIInfo(ActionTypeEnum.AddRefreshToken, "Přidání nového refresh tokenu.");

            return Ok(new
            {
                token = newJwt,
                refreshToken = newRefresh.Token
            });
        }


        private string GenerateToken(UserModel user, UserGroupModel group) 
        { 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"], 
                claims: new[] 
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, UserRoles.GetRoleById(user.RoleId)),
                    new Claim(ClaimTypes.PrimaryGroupSid, user.GroupId.ToString()),
                    new Claim(CustomClaimTypes.FolderName, group?.FolderName ?? ""),
                    new Claim(CustomClaimTypes.GroupName, group?.GroupName ?? ""),
                    new Claim(CustomClaimTypes.ProfileImage, user?.SystemImagesId.ToString()),
                }, 
                expires: DateTime.Now.AddHours(2), 
                signingCredentials: creds); 
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }

        private RefreshTokenModel GenerateRefreshToken()
        {
            return new RefreshTokenModel
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(14),
                IsRevoked = false
            };
        }

    }
}
