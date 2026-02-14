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
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Regions_RegionId",
                table: "Listings");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Listings",
                type: "decimal(9,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a6f2c4eb-9d8h-4f6g-2f7g-666666666666", 0, "34c20ba7-7b21-4208-85dc-703cc15bee2a", "radostin.electro@example.com", true, false, null, "RADOSTIN.ELECTRO@EXAMPLE.COM", "RADOSTIN_ELECTRO", null, "+359882678901", false, "99a72d3d-acf2-4821-b920-7b5cde3926c9", false, "radostin_electro" },
                    { "b1a7f9d6-4e3c-4a1b-9a2b-111111111111", 0, "d72ed1e0-8dd5-4594-b9c9-07b01016b9fc", "ivan.petrov@example.com", true, false, null, "IVAN.PETROV@EXAMPLE.COM", "IVAN_PETROV", null, "+359887123001", false, "753ac267-fabc-4565-8a86-0834cbc6a5fd", false, "ivan_petrov" },
                    { "c2b8e0a7-5f4d-4b2c-8b3c-222222222222", 0, "ce002fc3-0f9f-47ac-830b-f7017c25fb12", "maria.georgieva@example.com", true, false, null, "MARIA.GEORGIEVA@EXAMPLE.COM", "MARIA_GEORGIEVA", null, "+359888234567", false, "c1ac5fa4-d732-4e8f-99e8-aa8c5a2f0557", false, "maria_georgieva" },
                    { "d3c9f1b8-6a5e-4c3d-9c4d-333333333333", 0, "4ba121e5-5342-4988-87f4-3bce9787e353", "stoyan.tech@example.com", true, false, null, "STOYAN.TECH@EXAMPLE.COM", "STOYAN_TECH", null, "+359878345678", false, "dad00c05-a154-412d-bcbf-d9e3c33ea397", false, "stoyan_tech" },
                    { "e4d0a2c9-7b6f-4d4e-0d5e-444444444444", 0, "bfbce724-16e8-4852-b5a9-ac3581adec79", "elena.service@example.com", true, false, null, "ELENA.SERVICE@EXAMPLE.COM", "ELENASERVICE", null, "+359889456789", false, "00efec15-ad3a-4d50-8945-f22542a13201", false, "elenaservice" },
                    { "f5e1b3da-8c7g-4e5f-1e6f-555555555555", 0, "6181928c-f1b3-4322-b782-7bf7b8258bb0", "petar.fix@example.com", true, false, null, "PETAR.FIX@EXAMPLE.COM", "PETAR_FIX", null, "+359886567890", false, "5a21b14b-64ec-4bb7-9588-8cc4590cfd65", false, "petar_fix" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Електроника и запояване" },
                    { 2, "Ремонт на домакински уреди" },
                    { 3, "Електротехника и окабеляване" },
                    { 4, "Ремонт на телефони и таблети" },
                    { 5, "Монтаж и сглобяване" },
                    { 6, "Диагностика и сервиз" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "София-град" },
                    { 2, "Пловдив" },
                    { 3, "Варна" },
                    { 4, "Бургас" },
                    { 5, "Русе" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Name", "RegionId" },
                values: new object[,]
                {
                    { 1, "София", 1 },
                    { 2, "Младост", 1 },
                    { 3, "Пловдив", 2 },
                    { 4, "Карлово", 2 },
                    { 5, "Варна", 3 },
                    { 6, "Бургас", 4 },
                    { 7, "Русе", 5 }
                });

            migrationBuilder.InsertData(
                table: "Listings",
                columns: new[] { "Id", "CategoryId", "CityId", "CreatedAt", "CreatorId", "Description", "ImageUrl", "IsDeleted", "Price", "RegionId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2025, 1, 10, 11, 0, 0, 0, DateTimeKind.Local), "b1a7f9d6-4e3c-4a1b-9a2b-111111111111", "Запояване на SMD/TH components, подмяна на чипове, ремонт на дистанционни и малки платки. Работя с стационарна станция за запояване и горещ въздух.", "https://source.unsplash.com/800x450/?soldering,electronics", false, 35.00m, 1, "Запояване на платки и ремонт на дребна електроника", null },
                    { 2, 2, 2, new DateTime(2025, 1, 21, 16, 30, 0, 0, DateTimeKind.Local), "c2b8e0a7-5f4d-4b2c-8b3c-222222222222", "Диагностика за 20 лв, ремонт на помпи, лагер и електроника. Извършвам и монтаж/демонтаж.", "https://source.unsplash.com/800x450/?washing-machine,repair", false, 70.00m, 1, "Ремонт и диагностика на перални", null },
                    { 3, 3, 3, new DateTime(2025, 2, 1, 10, 15, 0, 0, DateTimeKind.Local), "d3c9f1b8-6a5e-4c3d-9c4d-333333333333", "Монтаж на контакти, ключове и осветление. Гаранция 12 месеца за извършената работа.", "https://source.unsplash.com/800x450/?electrician,tools", false, 45.00m, 2, "Инсталация и окабеляване за нови контакти", null },
                    { 4, 4, 5, new DateTime(2025, 1, 18, 13, 45, 0, 0, DateTimeKind.Local), "e4d0a2c9-7b6f-4d4e-0d5e-444444444444", "Бърза смяна на дисплей и батерия на повечето марки. Оригинални/следпазарни части по избор.", "https://source.unsplash.com/800x450/?phone-repair,smartphone", false, 60.00m, 3, "Смяна на дисплей и батерия за смартфони", null },
                    { 5, 2, 6, new DateTime(2025, 1, 5, 12, 0, 0, 0, DateTimeKind.Local), "f5e1b3da-8c7g-4e5f-1e6f-555555555555", "Проверка, смяна на нагревател и термостат. Монтаж и обезвъздушаване.", "https://source.unsplash.com/800x450/?water-heater,repair", false, 50.00m, 4, "Сервиз и монтаж на бойлери", null },
                    { 6, 6, 7, new DateTime(2025, 2, 3, 18, 20, 0, 0, DateTimeKind.Local), "a6f2c4eb-9d8h-4f6g-2f7g-666666666666", "Пълен тест и ремонт на контролни платки за битова и индустриална техника.", "https://source.unsplash.com/800x450/?circuit-board,repair", false, 95.00m, 5, "Диагностика на електронни табла и управление", null },
                    { 7, 2, 1, new DateTime(2025, 1, 29, 11, 50, 0, 0, DateTimeKind.Local), "c2b8e0a7-5f4d-4b2c-8b3c-222222222222", "Излизане на адрес в рамките на деня за малки ремонти на кухни, печки, хладилници.", "https://source.unsplash.com/800x450/?home-appliance,repair", false, 40.00m, 1, "Спешен техник за битови уреди (на място)", null },
                    { 8, 3, 4, new DateTime(2025, 1, 12, 15, 5, 0, 0, DateTimeKind.Local), "d3c9f1b8-6a5e-4c3d-9c4d-333333333333", "Професионален монтаж на електрически табла, автомати и защити. Сертифициран електротехник.", "https://source.unsplash.com/800x450/?electrical-panel,installation", false, 120.00m, 2, "Електрически табла и защити - монтаж", null },
                    { 9, 1, 5, new DateTime(2025, 2, 5, 14, 0, 0, 0, DateTimeKind.Local), "b1a7f9d6-4e3c-4a1b-9a2b-111111111111", "Ремонт на дънни платки и BGA преинсталации (по договаряне).", "https://source.unsplash.com/800x450/?bga,soldering", false, 180.00m, 3, "Запояване на BGA/микропоколения (усложнена електроника)", null },
                    { 10, 5, 6, new DateTime(2025, 1, 8, 17, 40, 0, 0, DateTimeKind.Local), "e4d0a2c9-7b6f-4d4e-0d5e-444444444444", "Сглобяване на мебели, монтиране на осветителни тела, окабеляване за уреди.", "https://source.unsplash.com/800x450/?assembly,tools", false, 30.00m, 4, "Монтаж на мебели и електрически елементи", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Regions_RegionId",
                table: "Listings",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Regions_RegionId",
                table: "Listings");

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
                table: "Listings",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a6f2c4eb-9d8h-4f6g-2f7g-666666666666");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b1a7f9d6-4e3c-4a1b-9a2b-111111111111");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2b8e0a7-5f4d-4b2c-8b3c-222222222222");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d3c9f1b8-6a5e-4c3d-9c4d-333333333333");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e4d0a2c9-7b6f-4d4e-0d5e-444444444444");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f5e1b3da-8c7g-4e5f-1e6f-555555555555");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

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
                keyValue: 6);

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
                keyValue: 4);

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
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 1);

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
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Listings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Listings",
                type: "decimal(9,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Regions_RegionId",
                table: "Listings",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
