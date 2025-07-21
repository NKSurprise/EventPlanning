using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanningAndManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeniedToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDenied",
                table: "Registrations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "792e9263-125c-447b-b699-b2d7742a7a1b", "AQAAAAIAAYagAAAAELVpPI15KCpDUVfKWYG5M4Q+WOo+E2AvoU73WBwuiAy5G9wtXR5Ken/sJ9KOi+M9pw==", "d5421025-fc9a-4705-857f-be3768a73d8b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDenied",
                table: "Registrations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6e7ddc79-6b86-49ef-bff4-c06eb0e89e83", "AQAAAAIAAYagAAAAEBl3QXXnUHvN0MM7jZGHwhQghn2aWGrJEEuVP/ozYhYIutordcw3n4RUUPTSJX9HNA==", "ab161654-3a67-4df8-8be8-ba5207174b22" });
        }
    }
}
