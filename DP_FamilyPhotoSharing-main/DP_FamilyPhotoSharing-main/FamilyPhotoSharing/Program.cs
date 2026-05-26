using DataAccessLayer.Dao;
using EncryptionLayer.Photo;
using FamilyPhotoSharing.Compossition;
using FamilyPhotoSharing.MiddleWare;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.System;
using ServiceLayer.Services;
using ServiceLayer.Services.Accounts;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .SetApplicationName("FamilyPhotoSharing");

var key = builder.Configuration["Jwt:Key"]; 
var issuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDained";
    options.Cookie.Name = "FamilyPhotoSharingAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
}).AddJwtBearer("ApiJwtAuth", options => { 
    options.TokenValidationParameters = new TokenValidationParameters { 
        ValidateIssuer = true, 
        ValidateAudience = false, 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        ValidIssuer = issuer, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) 
    }; 
});

builder = new CompossitionRoot().Build(builder);
builder.Services.Configure<VaultOptions>(
    builder.Configuration.GetSection("Vault"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<VaultOptions>>().Value);
builder.Services.AddSingleton<VaultService>();
var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/api"),
        appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/Error/{0}")
    );
}
else
{
    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/api"),
        appBuilder => appBuilder.UseExceptionHandler("/Error/ServerError")
    );

    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/api"),
        appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/Error/{0}")
    );

    app.UseHsts();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapControllers();

app.Run();
