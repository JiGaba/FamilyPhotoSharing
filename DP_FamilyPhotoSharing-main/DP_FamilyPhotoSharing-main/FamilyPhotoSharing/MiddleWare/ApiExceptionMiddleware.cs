namespace FamilyPhotoSharing.MiddleWare
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // status kódy (404, 401, 403…)
                if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600)
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.ContentType = "application/json";

                        var result = new
                        {
                            success = false,
                            status = context.Response.StatusCode,
                            error = GetDefaultMessage(context.Response.StatusCode)
                        };

                        await context.Response.WriteAsJsonAsync(result);
                    }
                }
            }
            catch (Exception ex)
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var result = new
                    {
                        success = false,
                        status = 500,
                        error = ex.Message
                    };

                    await context.Response.WriteAsJsonAsync(result);
                }
                else
                {
                    throw;
                }
            }
        }

        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                404 => "Endpoint nenalezen.",
                401 => "Nejste autorizován.",
                403 => "Nemáte oprávnění.",
                _ => "Došlo k chybě."
            };
        }
    }


}
