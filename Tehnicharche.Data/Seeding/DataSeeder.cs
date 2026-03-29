using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Seeding.DTOs;

namespace Tehnicharche.Data.Seeding
{
    public class DataSeeder
    {
        public static string[] Roles = { "Admin", "User" };

        private static string SeedsPath =>
            Path.GetFullPath(@"..\Tehnicharche.Data\Seeding\Seeds\");

        private readonly TehnicharcheDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DataSeeder(
            TehnicharcheDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync();
            await SeedCategoriesFromJsonAsync();
            await SeedRegionsFromJsonAsync();
            await SeedCitiesFromJsonAsync();
            await SeedUsersFromJsonAsync();
            await SeedListingsFromJsonAsync();
        }

        public async Task SeedRolesAsync()
        {
            foreach (var roleName in Roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }


        private async Task SeedCategoriesFromJsonAsync()
        {
            if (await context.Categories.AnyAsync()) 
                return;

            var json = await ReadSeedFileAsync("categories.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(json)!;

            var categories = dtos.Select(d => new Category { Id = d.Id, Name = d.Name });

            await context.Database.OpenConnectionAsync();

            try
            {
                await SetIdentityInsertOnAsync("Categories");

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                await SetIdentityInsertOffAsync("Categories");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }


        private async Task SeedRegionsFromJsonAsync()
        {
            if (await context.Regions.AnyAsync()) 
                return;

            var json = await ReadSeedFileAsync("regions.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<RegionDto>>(json)!;

            var regions = dtos.Select(d => new Region { Id = d.Id, Name = d.Name });

            await context.Database.OpenConnectionAsync();

            try
            {
                await SetIdentityInsertOnAsync("Regions");

                await context.Regions.AddRangeAsync(regions);
                await context.SaveChangesAsync();

                await SetIdentityInsertOffAsync("Regions");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }


        private async Task SeedCitiesFromJsonAsync()
        {
            if (await context.Cities.AnyAsync()) 
                return;

            var json = await ReadSeedFileAsync("cities.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<CityDto>>(json)!;

            var cities = dtos.Select(d => new City
            {
                Id = d.Id,
                Name = d.Name,
                RegionId = d.RegionId
            });

            await context.Database.OpenConnectionAsync();

            try
            {
                await SetIdentityInsertOnAsync("Cities");

                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();

                await SetIdentityInsertOffAsync("Cities");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }


        private async Task SeedUsersFromJsonAsync()
        {
            var json = await ReadSeedFileAsync("users.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<UserDto>>(json)!;

            foreach (var dto in dtos)
            {
                if (await userManager.FindByIdAsync(dto.Id) != null) 
                    continue;

                var user = new ApplicationUser
                {
                    Id = dto.Id,
                    UserName = dto.UserName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to seed user '{dto.UserName}'");
                }

                if (dto.Roles != null)
                {
                    foreach (var role in dto.Roles)
                        await userManager.AddToRoleAsync(user, role);
                }
            }
        }


        private async Task SeedListingsFromJsonAsync()
        {
            if (await context.Listings.IgnoreQueryFilters().AnyAsync()) 
                return;

            var json = await ReadSeedFileAsync("listings.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json)!;

            var listings = dtos.Select(d => new Listing
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                Price = d.Price,
                CategoryId = d.CategoryId,
                RegionId = d.RegionId,
                CityId = d.CityId,
                ImageUrl = d.ImageUrl,
                CreatorId = d.CreatorId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.CreatedAt,
                IsDeleted = false
            });

            await context.Database.OpenConnectionAsync();

            try
            {
                await SetIdentityInsertOnAsync("Listings");

                await context.Listings.AddRangeAsync(listings);
                await context.SaveChangesAsync();

                await SetIdentityInsertOffAsync("Listings");
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }


        // helper methods
        private static async Task<string> ReadSeedFileAsync(string fileName)
        {
            var path = Path.Combine(SeedsPath, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found: {path}");

            return await File.ReadAllTextAsync(path);
        }

        // IDENTITY_INSERT has to be enabled in order to seed explicit ids into identity columns
        // NOTE: this must run on the same open db connection as SaveChanges(),
        // otherwise IDENTITY_INSERT can still be treated as OFF (scope issue)

        // SQL injection isn't possible here because the methods are only used internally
        private async Task SetIdentityInsertOnAsync(string tableName)
        {
            await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] ON");
        }

        private async Task SetIdentityInsertOffAsync(string tableName)
        {
            await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] OFF");
        }
    }
}