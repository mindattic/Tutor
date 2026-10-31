using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tutor.Core.Migrations
{
    /// <inheritdoc />
    public partial class AuthV2DisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "auth",
                table: "AuthUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedDisplayName",
                schema: "auth",
                table: "AuthUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_NormalizedDisplayName",
                schema: "auth",
                table: "AuthUsers",
                column: "NormalizedDisplayName",
                unique: true,
                filter: "[NormalizedDisplayName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthUsers_NormalizedDisplayName",
                schema: "auth",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "auth",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedDisplayName",
                schema: "auth",
                table: "AuthUsers");
        }
    }
}
