using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace FamilyPhotoSharing.AccountManagement
{
    public static class ChangeClaimItem
    {
        public static async Task UpdateUserCustomClaimAsync(HttpContext httpContext, string newClaim, string customClaim)
        {
            var identity = (ClaimsIdentity)httpContext.User.Identity;

            var oldClaim = identity.FindFirst(customClaim);
            if (oldClaim != null)
                identity.RemoveClaim(oldClaim);

            identity.AddClaim(new Claim(customClaim, newClaim));

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
        }
    }

}
