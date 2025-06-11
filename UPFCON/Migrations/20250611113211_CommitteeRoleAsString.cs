using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPFCON.Migrations
{
    /// <inheritdoc />
    public partial class CommitteeRoleAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE [CommitteeMembers] DROP CONSTRAINT CK_AllowedCommitteeMemberRoles");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "CommitteeMembers",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(
                @"ALTER TABLE [CommitteeMembers] ADD CONSTRAINT CK_AllowedCommitteeMemberRoles
          CHECK (Role IN ('ExternalOrganizerChairman','Evaluator','HeadChairman'))");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "CommitteeMembers",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Evaluator");
        }
    }
}
