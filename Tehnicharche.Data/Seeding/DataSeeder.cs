using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Tehnicharche.Data.Models;
using Tehnicharche.Data.Seeding.DTOs;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Data.Seeding
{
    public class DataSeeder
    {
        public static readonly string[] Roles = { AdminRole, UserRole };

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

        // If there is already data in the database, nothing will be seeded.
        // During seeding, invalid DTOs are skipped (not inserted).
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

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private async Task SeedCategoriesFromJsonAsync()
        {
            if (await context.Categories.AnyAsync())
                return;

            var json = await ReadSeedFileAsync("categories.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(json)!;

            var categories = new List<Category>();

            foreach (var dto in dtos)
            {
                if (IsValid(dto))
                {
                    categories.Add(new Category()
                    {
                        Id = dto.Id,
                        Name = dto.Name
                    });
                }
            }

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

            var regions = new List<Region>();

            foreach (var dto in dtos)
            {
                if (IsValid(dto))
                {
                    regions.Add(new Region()
                    {
                        Id = dto.Id,
                        Name = dto.Name
                    });
                }
            }

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

            var cities = new List<City>();

            foreach (var dto in dtos)
            {
                if (IsValid(dto))
                {
                    cities.Add(new City
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        RegionId = dto.RegionId
                    });
                }
            }

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
                if (IsValid(dto))
                {
                    if (await userManager.FindByIdAsync(dto.Id.ToString()) != null)
                        continue;

                    var user = new ApplicationUser
                    {
                        Id = dto.Id.ToString(),
                        UserName = dto.UserName,
                        Email = dto.Email,
                        PhoneNumber = dto.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, dto.Password);

                    if (!result.Succeeded)
                        throw new InvalidOperationException(
                            $"Failed to seed user '{dto.UserName}': " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));

                    if (dto.Roles != null)
                    {
                        foreach (var role in dto.Roles)
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }

        private async Task SeedListingsFromJsonAsync()
        {
            if (await context.Listings.IgnoreQueryFilters().AnyAsync())
                return;

            var json = await ReadSeedFileAsync("listings.json");
            var dtos = JsonSerializer.Deserialize<IEnumerable<ListingDto>>(json)!;

            var listings = new List<Listing>();

            foreach (var dto in dtos)
            {
                if (IsValid(dto))
                {
                    listings.Add(new Listing
                    {
                        Id = dto.Id,
                        Title = dto.Title,
                        Description = dto.Description,
                        Price = dto.Price,
                        CategoryId = dto.CategoryId,
                        RegionId = dto.RegionId,
                        CityId = dto.CityId,
                        ImageUrl = dto.ImageUrl,
                        CreatorId = dto.CreatorId.ToString(),
                        CreatedAt = dto.CreatedAt,
                        UpdatedAt = dto.CreatedAt,
                        IsDeleted = false
                    });
                }
            }

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

        // helpers
        public static bool IsValid<T>(T dto)
        {
            if (dto == null)
                return false;

            var context = new ValidationContext(dto);
            return Validator.TryValidateObject(dto, context, null, validateAllProperties: true);
        }

        private static async Task<string> ReadSeedFileAsync(string fileName)
        {
            var path = Path.Combine(SeedsPath, fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found: {path}");

            return await File.ReadAllTextAsync(path);
        }

        // SET IDENTITY_INSERT must run on the same open connection as SaveChanges;
        // otherwise SQL Server treats it as OFF (it is connection-scoped).
        // Raw SQL is safe here — table names are internal constants, not user input.
        private async Task SetIdentityInsertOnAsync(string tableName)
            => await context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT [{tableName}] ON");

        private async Task SetIdentityInsertOffAsync(string tableName)
            => await context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT [{tableName}] OFF");
    }
}