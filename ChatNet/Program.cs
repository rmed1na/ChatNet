using ChatNet;
using ChatNet.Data.Context;
using ChatNet.Data.Models.Settings;
using ChatNet.Hubs.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Services
var appSettings = builder.Configuration
    .GetSection("Application")
    .Get<AppSettings>();

if (appSettings == null)
    throw new InvalidOperationException("Can't continue. Application settings must be present");

builder.Services.AddDbContext<ChatNetContext>(opt =>
{
    opt.UseLazyLoadingProxies();
    opt.UseSqlServer(appSettings.DataSource.BuildSqlServerConnectionString());
}, ServiceLifetime.Transient);

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Auth/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSassCompiler();
}

DependencyInjection.Configure(builder.Services);
#endregion

#region App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");
app.MapHubs();
app.Run();
#endregion