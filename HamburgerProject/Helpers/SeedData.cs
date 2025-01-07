using HamburgerProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace HamburgerProject.Helpers
{
    public static class SeedData
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleExist = await roleManager.RoleExistsAsync("Admin");

            if (!roleExist)
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
            }

            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");

            if (adminUser == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };

                await userManager.CreateAsync(newUser, "Admin@123");
                await userManager.AddToRoleAsync(newUser, "Admin");
            }

            // Customer rolü için aynı işlemi yapabilirsiniz
            var customerRoleExist = await roleManager.RoleExistsAsync("Customer");

            if (!customerRoleExist)
            {
                var customerRole = new IdentityRole("Customer");
                await roleManager.CreateAsync(customerRole);
            }
        }
    } 
}