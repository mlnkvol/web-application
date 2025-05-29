using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineLibraryWebApplication.Data.Identity;
using OnlineLibraryWebApplication.Models;
using OnlineLibraryWebApplication.Services;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DblibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add DefaultIdentity instead of AddIdentityCore
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Вимикаємо підтвердження акаунту за електронною поштою
})
    .AddRoles<IdentityRole>() // Add this line to register RoleManager
    .AddEntityFrameworkStores<ApplicationIdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IDataPortServiceFactory<Book>, BookDataPortServiceFactory>();
builder.Services.AddScoped<IExportService<Book>, PdfExportService>();

builder.Services.AddDbContext<ApplicationIdentityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure the localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("uk-UA"), // Add Ukrainian culture
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    await app.InitializeRolesAsync();
    await app.InitializeDefaultUsersAsync(app.Configuration.GetSection("IdentityDefaults:SuperUser"), app.Configuration.GetSection("IdentityDefaults:DefaultUsers"));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable Localization
var supportedCultures = new[] { "en-US", "uk-UA" }; // Add Ukrainian culture
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture("en-US").AddSupportedCultures(supportedCultures).AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "books",
        pattern: "books/{action=Index}/{id?}/{name?}",
        defaults: new { controller = "Books" });

    endpoints.MapControllerRoute(
        name: "categories",
        pattern: "{action=Index}/{id?}",
        defaults: new { controller = "Categories" });

    endpoints.MapControllerRoute(
        name: "reviews",
        pattern: "reviews/{action=Index}/{id?}",
        defaults: new { controller = "Reviews" });

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Categories}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "publishers",
        pattern: "{action=Index}/{id?}",
        defaults: new { controller = "Publishers" });

    endpoints.MapControllerRoute(
        name: "genres",
        pattern: "{action=Index}/{id?}",
        defaults: new { controller = "Genres" });

    endpoints.MapControllerRoute(
        name: "authors",
        pattern: "{action=Index}/{id?}",
        defaults: new { controller = "Authors" });

});

app.Run();
