using HaiAnhTra.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace HaiAnhTra.Web.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminRole = "Admin";
            if (!await roleMgr.RoleExistsAsync(adminRole))
                await roleMgr.CreateAsync(new IdentityRole(adminRole));

            var email = "admin@haianhtra.vn";
            var user = await userMgr.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                await userMgr.CreateAsync(user, "Admin@12345"); // đổi sau khi deploy
                await userMgr.AddToRoleAsync(user, adminRole);
            }
        }
    }
}
