using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPFCON.Migrations
{
    /// <inheritdoc />
    public partial class AddEventValidationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidationStatus",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationStatus",
                table: "Events");
        }
    }
}
