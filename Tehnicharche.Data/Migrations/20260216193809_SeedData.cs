using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tehnicharche.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "00000000-0000-0000-0000-000000000001", 0, "CONCURRENCY_STAMP_1", "ivan@example.com", true, false, null, "IVAN@EXAMPLE.COM", "IVAN.TECH", "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser1==", "+359888123001", true, "SECURITY_STAMP_1", false, "ivan.tech" },
                    { "00000000-0000-0000-0000-000000000002", 0, "CONCURRENCY_STAMP_2", "maria@example.com", true, false, null, "MARIA@EXAMPLE.COM", "MARIA.REPAIRS", "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser2==", "+359888456002", true, "SECURITY_STAMP_2", false, "maria.repairs" },
                    { "00000000-0000-0000-0000-000000000003", 0, "CONCURRENCY_STAMP_3", "plovdiv@example.com", true, false, null, "PLOVDIV@EXAMPLE.COM", "PLOVDIV.FIXIT", "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser3==", "+359888789003", true, "SECURITY_STAMP_3", false, "plovdiv.fixit" },
                    { "00000000-0000-0000-0000-000000000004", 0, "CONCURRENCY_STAMP_4", "stz@example.com", true, false, null, "STZ@EXAMPLE.COM", "STZ.ELECTRIC", "AQAAAAIAAYagAAAAEMockHashValueForDevOnlyUser4==", "+359888321004", true, "SECURITY_STAMP_4", false, "stz.electric" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics Repair" },
                    { 2, "Appliance Repair" },
                    { 3, "Soldering & PCB Assembly" },
                    { 4, "Computer Repair" },
                    { 5, "Mobile Phone Repair" },
                    { 6, "TV & Audio Repair" },
                    { 7, "Home Wiring & Electrical" },
                    { 8, "Handyman (general technical)" },
                    { 9, "3D Printing & Prototyping" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Blagoevgrad" },
                    { 2, "Burgas" },
                    { 3, "Varna" },
                    { 4, "Veliko Tarnovo" },
                    { 5, "Vidin" },
                    { 6, "Vratsa" },
                    { 7, "Gabrovo" },
                    { 8, "Dobrich" },
                    { 9, "Kardzhali" },
                    { 10, "Kyustendil" },
                    { 11, "Lovech" },
                    { 12, "Montana" },
                    { 13, "Pazardzhik" },
                    { 14, "Pernik" },
                    { 15, "Pleven" },
                    { 16, "Plovdiv" },
                    { 17, "Razgrad" },
                    { 18, "Ruse" },
                    { 19, "Shumen" },
                    { 20, "Silistra" },
                    { 21, "Sliven" },
                    { 22, "Smolyan" },
                    { 23, "Sofia City" },
                    { 24, "Sofia Province" },
                    { 25, "Stara Zagora" },
                    { 26, "Targovishte" },
                    { 27, "Haskovo" },
                    { 28, "Yambol" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { 1, "Blagoevgrad", 1 },
                    { 2, "Bansko", 1 },
                    { 3, "Sandanski", 1 },
                    { 4, "Burgas", 2 },
                    { 5, "Nesebar", 2 },
                    { 6, "Sozopol", 2 },
                    { 7, "Varna", 3 },
                    { 8, "Provadiya", 3 },
                    { 9, "Veliko Tarnovo", 4 },
                    { 10, "Gorna Oryahovitsa", 4 },
                    { 11, "Vidin", 5 },
                    { 12, "Vratsa", 6 },
                    { 13, "Gabrovo", 7 },
                    { 14, "Dobrich", 8 },
                    { 15, "Balchik", 8 },
                    { 16, "Kardzhali", 9 },
                    { 17, "Kyustendil", 10 },
                    { 18, "Lovech", 11 },
                    { 19, "Montana", 12 },
                    { 20, "Pazardzhik", 13 },
                    { 21, "Pernik", 14 },
                    { 22, "Pleven", 15 },
                    { 23, "Plovdiv", 16 },
                    { 24, "Asenovgrad", 16 },
                    { 25, "Razgrad", 17 },
                    { 26, "Ruse", 18 },
                    { 27, "Shumen", 19 },
                    { 28, "Silistra", 20 },
                    { 29, "Sliven", 21 },
                    { 30, "Smolyan", 22 },
                    { 31, "Sofia", 23 },
                    { 32, "Kostinbrod", 24 },
                    { 33, "Pirdop", 24 },
                    { 34, "Stara Zagora", 25 },
                    { 35, "Targovishte", 26 },
                    { 36, "Haskovo", 27 },
                    { 37, "Yambol", 28 }
                });

            migrationBuilder.InsertData(
                table: "Listings",
                columns: new[] { "Id", "CategoryId", "CityId", "CreatedAt", "CreatorId", "Description", "ImageUrl", "IsDeleted", "Price", "RegionId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 3, 31, new DateTime(2025, 11, 1, 11, 30, 0, 0, DateTimeKind.Local), "00000000-0000-0000-0000-000000000001", "Micro-soldering, component replacement, BGA rework (where feasible). 5+ years experience with consumer and industrial PCBs.", "https://cdn.pixabay.com/photo/2017/06/06/16/35/cyber-2377718_1280.jpg", false, 45.00m, 23, "Surface-mount soldering & PCB repair (small boards)", null },
                    { 2, 2, 4, new DateTime(2025, 10, 20, 17, 0, 0, 0, DateTimeKind.Local), "00000000-0000-0000-0000-000000000002", "Fault codes, pump & heater replacement, drum/shaft repair, water inlet and electronic control fixes. Parts quoted separately.", "https://cdn.pixabay.com/photo/2014/03/06/11/30/washing-machine-280752_1280.jpg", false, 80.00m, 2, "Washing machine diagnostics & repair", null },
                    { 3, 4, 23, new DateTime(2025, 12, 5, 13, 15, 0, 0, DateTimeKind.Local), "00000000-0000-0000-0000-000000000003", "Power rails, damaged connectors, short diagnosis. I repair DC jack, blown MOSFETs, burnt traces and do BGA reballing referrals.", "https://cdn.pixabay.com/photo/2014/05/06/16/09/imac-338988_1280.jpg", false, 120.00m, 16, "Laptop motherboard repair — diagnostics + component level", null },
                    { 4, 5, 7, new DateTime(2025, 9, 30, 11, 45, 0, 0, DateTimeKind.Local), "00000000-0000-0000-0000-000000000001", "Original and high-quality aftermarket screens; installation and calibration. Data-safe service — backup recommended before service.", "https://cdn.pixabay.com/photo/2023/10/19/12/01/technician-8326389_1280.jpg", false, 60.00m, 3, "Mobile phone screen replacement (all makes)", null },
                    { 5, 7, 34, new DateTime(2025, 8, 10, 13, 0, 0, 0, DateTimeKind.Local), "00000000-0000-0000-0000-000000000004", "Safe, certified wiring repairs, new socket installs, LED lighting installation and troubleshooting. I follow local electrical codes.", "https://cdn.pixabay.com/photo/2015/12/07/10/49/electrician-1080554_1280.jpg", false, 35.00m, 25, "Home electrical — small jobs, sockets, switches, lighting", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 25);
        }
    }
}
