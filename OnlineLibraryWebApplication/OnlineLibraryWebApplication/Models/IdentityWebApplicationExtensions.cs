using Microsoft.AspNetCore.Identity;
using OnlineLibraryWebApplication.Data.Identity;
using OnlineLibraryWebApplication.Models; 


public static class IdentityWebApplicationExtensions
{
    private record UserInfo(string Username, string Password)
    {
        public UserInfo() : this(string.Empty, string.Empty)
        {
        }
    }

    private static async Task AddUserIfNotExistsAsync(UserManager<ApplicationUser> userManager, ILogger logger, string userName, string password, ICollection<string> roles)
    {
        var applicationUser = await userManager.FindByEmailAsync(userName);
        if (applicationUser is null)
        {
            applicationUser = new ApplicationUser
            {
                UserName = userName,
                Email = userName
            };
            await userManager.CreateAsync(applicationUser, password);
            logger.LogInformation("{username} user added", userName);
        }
        else
        {
            logger.LogInformation("User {username} is already in database", userName);
        }

        var existingRoles = await userManager.GetRolesAsync(applicationUser);
        foreach (var role in roles.Where(role => !existingRoles.Contains(role)))
        {
            await userManager.AddToRoleAsync(applicationUser, role);
            logger.LogInformation("{username} has {rolename} assigned", userName, role);
        }
    }

    public static async Task InitializeRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var roleName in RoleNames.All.Select(c => c.ToString()))
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole { Name = roleName };
                await roleManager.CreateAsync(role);
            }
        }
    }

    public static async Task InitializeDefaultUsersAsync(this WebApplication app, IConfiguration? superUserConfiguration, IConfiguration? defaultUsersConfiguration)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var superUserInfo = superUserConfiguration?.Get<UserInfo>();
        if (superUserInfo != null)
        {
            var allRoles = RoleNames.All.Select(c => c.ToString()).ToList();
            await AddUserIfNotExistsAsync(userManager, app.Logger, superUserInfo.Username, superUserInfo.Password, allRoles);
        }

        var defaultUserInfos = defaultUsersConfiguration?.Get<UserInfo[]>();
        if (defaultUserInfos != null)
        {
            foreach (var defaultUserInfo in defaultUserInfos)
            {
                await AddUserIfNotExistsAsync(userManager, app.Logger, defaultUserInfo.Username, defaultUserInfo.Password, Array.Empty<string>());
            }
        }
    }
}
