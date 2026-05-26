using FamilyPhotoSharing.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyPhotoSharing.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("Run")] 
        public IActionResult Run() 
        {
            var success = "API běží!";
            return Ok(ApiResponse<string>.Ok(success));
        }
        [HttpGet("RunLock")]
        [Authorize(AuthenticationSchemes = "ApiJwtAuth")]
        public IActionResult RunLock()
        {
            var success = "API běží!";
            return Ok(ApiResponse<string>.Ok(success));
        }
    }
}
