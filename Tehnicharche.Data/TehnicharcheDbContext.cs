using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data.Models;

namespace Tehnicharche.Data
{
    public class TehnicharcheDbContext : IdentityDbContext<ApplicationUser>
    {
        public TehnicharcheDbContext(DbContextOptions<TehnicharcheDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Region> Regions { get; set; } = null!;

        public virtual DbSet<City> Cities { get; set; } = null!;

        public virtual DbSet<Listing> Listings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Region>().HasData(
                new Region { Id = 1, Name = "Blagoevgrad" },
                new Region { Id = 2, Name = "Burgas" },
                new Region { Id = 3, Name = "Varna" },
                new Region { Id = 4, Name = "Veliko Tarnovo" },
                new Region { Id = 5, Name = "Vidin" },
                new Region { Id = 6, Name = "Vratsa" },
                new Region { Id = 7, Name = "Gabrovo" },
                new Region { Id = 8, Name = "Dobrich" },
                new Region { Id = 9, Name = "Kardzhali" },
                new Region { Id = 10, Name = "Kyustendil" },
                new Region { Id = 11, Name = "Lovech" },
                new Region { Id = 12, Name = "Montana" },
                new Region { Id = 13, Name = "Pazardzhik" },
                new Region { Id = 14, Name = "Pernik" },
                new Region { Id = 15, Name = "Pleven" },
                new Region { Id = 16, Name = "Plovdiv" },
                new Region { Id = 17, Name = "Razgrad" },
                new Region { Id = 18, Name = "Ruse" },
                new Region { Id = 19, Name = "Shumen" },
                new Region { Id = 20, Name = "Silistra" },
                new Region { Id = 21, Name = "Sliven" },
                new Region { Id = 22, Name = "Smolyan" },
                new Region { Id = 23, Name = "Sofia City" },
                new Region { Id = 24, Name = "Sofia Province" },
                new Region { Id = 25, Name = "Stara Zagora" },
                new Region { Id = 26, Name = "Targovishte" },
                new Region { Id = 27, Name = "Haskovo" },
                new Region { Id = 28, Name = "Yambol" }
            );

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Blagoevgrad", RegionId = 1 },
                new City { Id = 2, Name = "Bansko", RegionId = 1 },
                new City { Id = 3, Name = "Sandanski", RegionId = 1 },
                new City { Id = 4, Name = "Burgas", RegionId = 2 },
                new City { Id = 5, Name = "Nesebar", RegionId = 2 },
                new City { Id = 6, Name = "Sozopol", RegionId = 2 },
                new City { Id = 7, Name = "Varna", RegionId = 3 },
                new City { Id = 8, Name = "Provadiya", RegionId = 3 },
                new City { Id = 9, Name = "Veliko Tarnovo", RegionId = 4 },
                new City { Id = 10, Name = "Gorna Oryahovitsa", RegionId = 4 },
                new City { Id = 11, Name = "Vidin", RegionId = 5 },
                new City { Id = 12, Name = "Vratsa", RegionId = 6 },
                new City { Id = 13, Name = "Gabrovo", RegionId = 7 },
                new City { Id = 14, Name = "Dobrich", RegionId = 8 },
                new City { Id = 15, Name = "Balchik", RegionId = 8 },
                new City { Id = 16, Name = "Kardzhali", RegionId = 9 },
                new City { Id = 17, Name = "Kyustendil", RegionId = 10 },
                new City { Id = 18, Name = "Lovech", RegionId = 11 },
                new City { Id = 19, Name = "Montana", RegionId = 12 },
                new City { Id = 20, Name = "Pazardzhik", RegionId = 13 },
                new City { Id = 21, Name = "Pernik", RegionId = 14 },
                new City { Id = 22, Name = "Pleven", RegionId = 15 },
                new City { Id = 23, Name = "Plovdiv", RegionId = 16 },
                new City { Id = 24, Name = "Asenovgrad", RegionId = 16 },
                new City { Id = 25, Name = "Razgrad", RegionId = 17 },
                new City { Id = 26, Name = "Ruse", RegionId = 18 },
                new City { Id = 27, Name = "Shumen", RegionId = 19 },
                new City { Id = 28, Name = "Silistra", RegionId = 20 },
                new City { Id = 29, Name = "Sliven", RegionId = 21 },
                new City { Id = 30, Name = "Smolyan", RegionId = 22 },
                new City { Id = 31, Name = "Sofia", RegionId = 23 },
                new City { Id = 32, Name = "Kostinbrod", RegionId = 24 },
                new City { Id = 33, Name = "Pirdop", RegionId = 24 },
                new City { Id = 34, Name = "Stara Zagora", RegionId = 25 },
                new City { Id = 35, Name = "Targovishte", RegionId = 26 },
                new City { Id = 36, Name = "Haskovo", RegionId = 27 },
                new City { Id = 37, Name = "Yambol", RegionId = 28 }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics Repair" },
                new Category { Id = 2, Name = "Appliance Repair" },
                new Category { Id = 3, Name = "Soldering & PCB Assembly" },
                new Category { Id = 4, Name = "Computer Repair" },
                new Category { Id = 5, Name = "Mobile Phone Repair" },
                new Category { Id = 6, Name = "TV & Audio Repair" },
                new Category { Id = 7, Name = "Home Wiring & Electrical" },
                new Category { Id = 8, Name = "Handyman (general technical)" },
                new Category { Id = 9, Name = "3D Printing & Prototyping" }
            );

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "00000000-0000-0000-0000-000000000001",
                    UserName = "ivan.tech",
                    NormalizedUserName = "IVAN.TECH",
                    Email = "ivan@example.com",
                    NormalizedEmail = "IVAN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser1==",
                    SecurityStamp = "SECURITY_STAMP_1",
                    ConcurrencyStamp = "CONCURRENCY_STAMP_1",
                    PhoneNumber = "+359888123001",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new ApplicationUser
                {
                    Id = "00000000-0000-0000-0000-000000000002",
                    UserName = "maria.repairs",
                    NormalizedUserName = "MARIA.REPAIRS",
                    Email = "maria@example.com",
                    NormalizedEmail = "MARIA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser2==",
                    SecurityStamp = "SECURITY_STAMP_2",
                    ConcurrencyStamp = "CONCURRENCY_STAMP_2",
                    PhoneNumber = "+359888456002",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new ApplicationUser
                {
                    Id = "00000000-0000-0000-0000-000000000003",
                    UserName = "plovdiv.fixit",
                    NormalizedUserName = "PLOVDIV.FIXIT",
                    Email = "plovdiv@example.com",
                    NormalizedEmail = "PLOVDIV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser3==",
                    SecurityStamp = "SECURITY_STAMP_3",
                    ConcurrencyStamp = "CONCURRENCY_STAMP_3",
                    PhoneNumber = "+359888789003",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new ApplicationUser
                {
                    Id = "00000000-0000-0000-0000-000000000004",
                    UserName = "stz.electric",
                    NormalizedUserName = "STZ.ELECTRIC",
                    Email = "stz@example.com",
                    NormalizedEmail = "STZ@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser4==",
                    SecurityStamp = "SECURITY_STAMP_4",
                    ConcurrencyStamp = "CONCURRENCY_STAMP_4",
                    PhoneNumber = "+359888321004",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                }
            );

            modelBuilder.Entity<Listing>().HasData(
                new Listing
                {
                    Id = 1,
                    Title = "Surface-mount soldering & PCB repair (small boards)",
                    Description = "Micro-soldering, component replacement, BGA rework (where feasible). 5+ years experience with consumer and industrial PCBs.",
                    Price = 45.00m,
                    CategoryId = 3,
                    RegionId = 23,
                    CityId = 31,
                    ImageUrl = "https://cdn.pixabay.com/photo/2017/06/06/16/35/cyber-2377718_1280.jpg",
                    CreatorId = "00000000-0000-0000-0000-000000000001",
                    CreatedAt = DateTime.Parse("2025-11-01T09:30:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 2,
                    Title = "Washing machine diagnostics & repair",
                    Description = "Fault codes, pump & heater replacement, drum/shaft repair, water inlet and electronic control fixes. Parts quoted separately.",
                    Price = 80.00m,
                    CategoryId = 2,
                    RegionId = 2,
                    CityId = 4,
                    ImageUrl = "https://cdn.pixabay.com/photo/2014/03/06/11/30/washing-machine-280752_1280.jpg",
                    CreatorId = "00000000-0000-0000-0000-000000000002",
                    CreatedAt = DateTime.Parse("2025-10-20T14:00:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 3,
                    Title = "Laptop motherboard repair — diagnostics + component level",
                    Description = "Power rails, damaged connectors, short diagnosis. I repair DC jack, blown MOSFETs, burnt traces and do BGA reballing referrals.",
                    Price = 120.00m,
                    CategoryId = 4,
                    RegionId = 16,
                    CityId = 23,
                    ImageUrl = "https://cdn.pixabay.com/photo/2014/05/06/16/09/imac-338988_1280.jpg",
                    CreatorId = "00000000-0000-0000-0000-000000000003",
                    CreatedAt = DateTime.Parse("2025-12-05T11:15:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 4,
                    Title = "Mobile phone screen replacement (all makes)",
                    Description = "Original and high-quality aftermarket screens; installation and calibration. Data-safe service — backup recommended before service.",
                    Price = 60.00m,
                    CategoryId = 5,
                    RegionId = 3,
                    CityId = 7,
                    ImageUrl = "https://cdn.pixabay.com/photo/2023/10/19/12/01/technician-8326389_1280.jpg",
                    CreatorId = "00000000-0000-0000-0000-000000000001",
                    CreatedAt = DateTime.Parse("2025-09-30T08:45:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 5,
                    Title = "Home electrical — small jobs, sockets, switches, lighting",
                    Description = "Safe, certified wiring repairs, new socket installs, LED lighting installation and troubleshooting. I follow local electrical codes.",
                    Price = 35.00m,
                    CategoryId = 7,
                    RegionId = 25,
                    CityId = 34,
                    ImageUrl = "https://cdn.pixabay.com/photo/2015/12/07/10/49/electrician-1080554_1280.jpg",
                    CreatorId = "00000000-0000-0000-0000-000000000004",
                    CreatedAt = DateTime.Parse("2025-08-10T10:00:00Z"),
                    IsDeleted = false
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
