using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HamburgerProject.Data;
using HamburgerProject.Areas.Identity.Data;
using HamburgerProject.Helpers;
using HamburgerProject.Models;
namespace HamburgerProject
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("HamburgerProjectContextConnection") ?? throw new InvalidOperationException("Connection string 'HamburgerProjectContextConnection' not found.");             

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<HamburgerProjectContext>();
            builder.Services.AddControllersWithViews();

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<HamburgerProjectContext>()
            //    .AddDefaultTokenProviders();

            builder.Services.AddDbContext<HamburgerProjectContext>(options => options.UseSqlServer("HamburgerProjectContextConnection"));
           
            // builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //using (var scope = app.Services.CreateScope())
            //{
            //    var serviceProvider = scope.ServiceProvider;
            //    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //    await SeedData.SeedAdminUserAsync(userManager, roleManager);

            //}

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages(); // Sayfalarý iliþkilendir. ( Partial bölümlerini )
            app.Run();
        }
    }
}
