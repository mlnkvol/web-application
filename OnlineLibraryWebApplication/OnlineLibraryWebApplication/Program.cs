using OnlineLibraryWebApplication;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;
using OnlineLibraryWebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DblibraryContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddScoped<IDataPortServiceFactory<Book>, BookDataPortServiceFactory>();

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

app.UseAuthorization();

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
        name: "default",
        pattern: "{controller= Categories}/{action=Index}/{id?}");

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

