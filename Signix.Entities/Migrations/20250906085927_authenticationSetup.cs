using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Signix.Entities.Migrations
{
    /// <inheritdoc />
    public partial class authenticationSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AzureADUserId",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "users",
                type: "character varying(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzureADUserId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "type",
                table: "users");
        }
    }
}
