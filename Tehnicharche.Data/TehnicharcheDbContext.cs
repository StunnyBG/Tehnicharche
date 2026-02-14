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
            // Seed data
            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Електроника и запояване" },
                new Category { Id = 2, Name = "Ремонт на домакински уреди" },
                new Category { Id = 3, Name = "Електротехника и окабеляване" },
                new Category { Id = 4, Name = "Ремонт на телефони и таблети" },
                new Category { Id = 5, Name = "Монтаж и сглобяване" },
                new Category { Id = 6, Name = "Диагностика и сервиз" }
            );

            // Regions
            modelBuilder.Entity<Region>().HasData(
                new Region { Id = 1, Name = "София-град" },
                new Region { Id = 2, Name = "Пловдив" },
                new Region { Id = 3, Name = "Варна" },
                new Region { Id = 4, Name = "Бургас" },
                new Region { Id = 5, Name = "Русе" }
            );

            // Cities
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "София", RegionId = 1 },
                new City { Id = 2, Name = "Младост", RegionId = 1 },
                new City { Id = 3, Name = "Пловдив", RegionId = 2 },
                new City { Id = 4, Name = "Карлово", RegionId = 2 },
                new City { Id = 5, Name = "Варна", RegionId = 3 },
                new City { Id = 6, Name = "Бургас", RegionId = 4 },
                new City { Id = 7, Name = "Русе", RegionId = 5 }
            );

            // ApplicationUser
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "b1a7f9d6-4e3c-4a1b-9a2b-111111111111",
                    UserName = "ivan_petrov",
                    NormalizedUserName = "IVAN_PETROV",
                    Email = "ivan.petrov@example.com",
                    NormalizedEmail = "IVAN.PETROV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359887123001"
                },
                new ApplicationUser
                {
                    Id = "c2b8e0a7-5f4d-4b2c-8b3c-222222222222",
                    UserName = "maria_georgieva",
                    NormalizedUserName = "MARIA_GEORGIEVA",
                    Email = "maria.georgieva@example.com",
                    NormalizedEmail = "MARIA.GEORGIEVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359888234567"
                },
                new ApplicationUser
                {
                    Id = "d3c9f1b8-6a5e-4c3d-9c4d-333333333333",
                    UserName = "stoyan_tech",
                    NormalizedUserName = "STOYAN_TECH",
                    Email = "stoyan.tech@example.com",
                    NormalizedEmail = "STOYAN.TECH@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359878345678"
                },
                new ApplicationUser
                {
                    Id = "e4d0a2c9-7b6f-4d4e-0d5e-444444444444",
                    UserName = "elenaservice",
                    NormalizedUserName = "ELENASERVICE",
                    Email = "elena.service@example.com",
                    NormalizedEmail = "ELENA.SERVICE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359889456789"
                },
                new ApplicationUser
                {
                    Id = "f5e1b3da-8c7g-4e5f-1e6f-555555555555",
                    UserName = "petar_fix",
                    NormalizedUserName = "PETAR_FIX",
                    Email = "petar.fix@example.com",
                    NormalizedEmail = "PETAR.FIX@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359886567890"
                },
                new ApplicationUser
                {
                    Id = "a6f2c4eb-9d8h-4f6g-2f7g-666666666666",
                    UserName = "radostin_electro",
                    NormalizedUserName = "RADOSTIN_ELECTRO",
                    Email = "radostin.electro@example.com",
                    NormalizedEmail = "RADOSTIN.ELECTRO@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "+359882678901"
                }
            );

            // Listings
            modelBuilder.Entity<Listing>().HasData(
                new Listing
                {
                    Id = 1,
                    Title = "Запояване на платки и ремонт на дребна електроника",
                    Description = "Запояване на SMD/TH components, подмяна на чипове, ремонт на дистанционни и малки платки. Работя с стационарна станция за запояване и горещ въздух.",
                    Price = 35.00m,
                    CategoryId = 1,
                    RegionId = 1,
                    CityId = 1,
                    ImageUrl = "https://source.unsplash.com/800x450/?soldering,electronics",
                    CreatorId = "b1a7f9d6-4e3c-4a1b-9a2b-111111111111",
                    CreatedAt = DateTime.Parse("2025-01-10T09:00:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 2,
                    Title = "Ремонт и диагностика на перални",
                    Description = "Диагностика за 20 лв, ремонт на помпи, лагер и електроника. Извършвам и монтаж/демонтаж.",
                    Price = 70.00m,
                    CategoryId = 2,
                    RegionId = 1,
                    CityId = 2,
                    ImageUrl = "https://source.unsplash.com/800x450/?washing-machine,repair",
                    CreatorId = "c2b8e0a7-5f4d-4b2c-8b3c-222222222222",
                    CreatedAt = DateTime.Parse("2025-01-21T14:30:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 3,
                    Title = "Инсталация и окабеляване за нови контакти",
                    Description = "Монтаж на контакти, ключове и осветление. Гаранция 12 месеца за извършената работа.",
                    Price = 45.00m,
                    CategoryId = 3,
                    RegionId = 2,
                    CityId = 3,
                    ImageUrl = "https://source.unsplash.com/800x450/?electrician,tools",
                    CreatorId = "d3c9f1b8-6a5e-4c3d-9c4d-333333333333",
                    CreatedAt = DateTime.Parse("2025-02-01T08:15:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 4,
                    Title = "Смяна на дисплей и батерия за смартфони",
                    Description = "Бърза смяна на дисплей и батерия на повечето марки. Оригинални/следпазарни части по избор.",
                    Price = 60.00m,
                    CategoryId = 4,
                    RegionId = 3,
                    CityId = 5,
                    ImageUrl = "https://source.unsplash.com/800x450/?phone-repair,smartphone",
                    CreatorId = "e4d0a2c9-7b6f-4d4e-0d5e-444444444444",
                    CreatedAt = DateTime.Parse("2025-01-18T11:45:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 5,
                    Title = "Сервиз и монтаж на бойлери",
                    Description = "Проверка, смяна на нагревател и термостат. Монтаж и обезвъздушаване.",
                    Price = 50.00m,
                    CategoryId = 2,
                    RegionId = 4,
                    CityId = 6,
                    ImageUrl = "https://source.unsplash.com/800x450/?water-heater,repair",
                    CreatorId = "f5e1b3da-8c7g-4e5f-1e6f-555555555555",
                    CreatedAt = DateTime.Parse("2025-01-05T10:00:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 6,
                    Title = "Диагностика на електронни табла и управление",
                    Description = "Пълен тест и ремонт на контролни платки за битова и индустриална техника.",
                    Price = 95.00m,
                    CategoryId = 6,
                    RegionId = 5,
                    CityId = 7,
                    ImageUrl = "https://source.unsplash.com/800x450/?circuit-board,repair",
                    CreatorId = "a6f2c4eb-9d8h-4f6g-2f7g-666666666666",
                    CreatedAt = DateTime.Parse("2025-02-03T16:20:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 7,
                    Title = "Спешен техник за битови уреди (на място)",
                    Description = "Излизане на адрес в рамките на деня за малки ремонти на кухни, печки, хладилници.",
                    Price = 40.00m,
                    CategoryId = 2,
                    RegionId = 1,
                    CityId = 1,
                    ImageUrl = "https://source.unsplash.com/800x450/?home-appliance,repair",
                    CreatorId = "c2b8e0a7-5f4d-4b2c-8b3c-222222222222",
                    CreatedAt = DateTime.Parse("2025-01-29T09:50:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 8,
                    Title = "Електрически табла и защити - монтаж",
                    Description = "Професионален монтаж на електрически табла, автомати и защити. Сертифициран електротехник.",
                    Price = 120.00m,
                    CategoryId = 3,
                    RegionId = 2,
                    CityId = 4,
                    ImageUrl = "https://source.unsplash.com/800x450/?electrical-panel,installation",
                    CreatorId = "d3c9f1b8-6a5e-4c3d-9c4d-333333333333",
                    CreatedAt = DateTime.Parse("2025-01-12T13:05:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 9,
                    Title = "Запояване на BGA/микропоколения (усложнена електроника)",
                    Description = "Ремонт на дънни платки и BGA преинсталации (по договаряне).",
                    Price = 180.00m,
                    CategoryId = 1,
                    RegionId = 3,
                    CityId = 5,
                    ImageUrl = "https://source.unsplash.com/800x450/?bga,soldering",
                    CreatorId = "b1a7f9d6-4e3c-4a1b-9a2b-111111111111",
                    CreatedAt = DateTime.Parse("2025-02-05T12:00:00Z"),
                    IsDeleted = false
                },
                new Listing
                {
                    Id = 10,
                    Title = "Монтаж на мебели и електрически елементи",
                    Description = "Сглобяване на мебели, монтиране на осветителни тела, окабеляване за уреди.",
                    Price = 30.00m,
                    CategoryId = 5,
                    RegionId = 4,
                    CityId = 6,
                    ImageUrl = "https://source.unsplash.com/800x450/?assembly,tools",
                    CreatorId = "e4d0a2c9-7b6f-4d4e-0d5e-444444444444",
                    CreatedAt = DateTime.Parse("2025-01-08T15:40:00Z"),
                    IsDeleted = false
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
