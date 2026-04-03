using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Repositories;
using Tehnicharche.Data.Repositories.Interfaces;
using Tehnicharche.Data.Seeding;
using Tehnicharche.Services.Core;
using Tehnicharche.Services.Core.Interfaces;

namespace Tehnicharche.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<TehnicharcheDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TehnicharcheDbContext>();

            builder.Services.AddMemoryCache();

            // Repositories
            builder.Services.AddScoped<IListingRepository, ListingRepository>();
            builder.Services.AddScoped<ISavedListingRepository, SavedListingRepository>();
            builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
            builder.Services.AddScoped<IAdminListingRepository, AdminListingRepository>();
            builder.Services.AddScoped<IAdminCategoryRepository, AdminCategoryRepository>();
            builder.Services.AddScoped<IAdminRegionRepository, AdminRegionRepository>();
            builder.Services.AddScoped<IAdminCityRepository, AdminCityRepository>();
            builder.Services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            builder.Services.AddScoped<IGenericRepository<Region>, GenericRepository<Region>>();
            builder.Services.AddScoped<IGenericRepository<City>, GenericRepository<City>>();

            // Services 
            builder.Services.AddScoped<IListingService, ListingService>();
            builder.Services.AddScoped<ISavedListingService, SavedListingService>();
            builder.Services.AddScoped<IAdminListingService, AdminListingService>();
            builder.Services.AddScoped<IAdminMessageService, AdminMessageService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            builder.Services.AddScoped<IAdminRegionService, AdminRegionService>();
            builder.Services.AddScoped<IAdminCityService, AdminCityService>();
            builder.Services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            builder.Services.AddScoped<DataSeeder>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<BanMiddleware>();
            app.UseAuthorization();

            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
