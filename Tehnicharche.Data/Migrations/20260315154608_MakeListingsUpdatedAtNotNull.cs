using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tehnicharche.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeListingsUpdatedAtNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 15, 15, 46, 8, 331, DateTimeKind.Utc).AddTicks(2572));

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 15, 15, 46, 8, 331, DateTimeKind.Utc).AddTicks(2678));

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 15, 15, 46, 8, 331, DateTimeKind.Utc).AddTicks(2690));

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 15, 15, 46, 8, 331, DateTimeKind.Utc).AddTicks(2698));

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 3, 15, 15, 46, 8, 331, DateTimeKind.Utc).AddTicks(2703));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Listings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: null);
        }
    }
}
