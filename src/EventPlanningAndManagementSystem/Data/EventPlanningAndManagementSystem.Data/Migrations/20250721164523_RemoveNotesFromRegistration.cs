using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanningAndManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotesFromRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Registrations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "35ea1a79-2c0e-4267-a1b7-6c82a69f6982", "AQAAAAIAAYagAAAAEHpgwZddS3AfP4vbcGmI/mygUj0RQ1/rNYO+Ob5FeZnDIKzeoffIj9TbPXN5ca9ofA==", "43d1dc39-b049-44d8-a6c2-d30b54805bfd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Registrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7699db7d-964f-4782-8209-d76562e0fece",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "792e9263-125c-447b-b699-b2d7742a7a1b", "AQAAAAIAAYagAAAAELVpPI15KCpDUVfKWYG5M4Q+WOo+E2AvoU73WBwuiAy5G9wtXR5Ken/sJ9KOi+M9pw==", "d5421025-fc9a-4705-857f-be3768a73d8b" });
        }
    }
}
