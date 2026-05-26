using FamilyPhotoSharing.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Enums;
using ServiceLayer.Services.Accounts;
using ServiceLayer.Services.Logs;

namespace FamilyPhotoSharing.Controllers.Error
{
    public class ErrorController : BaseController
    {
        public ErrorController(IMemoryCache cache, ISystemLogService log, IUserService userService)
            : base(cache, log, userService) { }

        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        [Route("Error/500")]
        public IActionResult ServerError()
        {
            if (CT_UserData().userData?.RoleEnum.Equals(UserRoleEnum.Admin) ?? false)
                ViewBag.Message = TempData["ErrorMessage"];

            Response.StatusCode = 500;
            return View("ServerError");
        }

        [Route("Error/599")]
        public IActionResult InitializeError()
        {
            ViewBag.Message = TempData["ErrorMessage"];
            Response.StatusCode = 599;
            return View("ServerError");
        }

        [Route("Error/{code}")]
        public IActionResult Error(int code)
        {
            Response.StatusCode = code;

            return code switch
            {
                404 => View("NotFound"),
                _ => View("ServerError")
            };
        }
    }
}
