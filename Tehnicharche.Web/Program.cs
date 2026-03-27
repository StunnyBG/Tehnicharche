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
                // Set to false — no email sender is configured; confirm emails automatically on register
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
            builder.Services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            builder.Services.AddScoped<IGenericRepository<Region>, GenericRepository<Region>>();
            builder.Services.AddScoped<IGenericRepository<City>, GenericRepository<City>>();

            builder.Services.AddScoped<IListingService, ListingService>();

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
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
