using Microsoft.AspNetCore.Diagnostics;

namespace FamilyPhotoSharing.MiddleWare
{
    public static class ApiExceptionPageExtensions
    {
        public static IApplicationBuilder UseConditionalDeveloperExceptionPage(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    var devPage = app.ApplicationServices.GetRequiredService<DeveloperExceptionPageMiddleware>();
                    await devPage.Invoke(context);
                }
                else
                {
                    await next();
                }
            });
        }
    }

}
