using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPFCON.Migrations
{
    /// <inheritdoc />
    public partial class passwordChangedFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordChanged",
                table: "BoardDirectors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PasswordChanged",
                table: "Admins",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordChanged",
                table: "BoardDirectors");

            migrationBuilder.DropColumn(
                name: "PasswordChanged",
                table: "Admins");
        }
    }
}
