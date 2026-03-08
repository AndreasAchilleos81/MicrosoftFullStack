using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityFixesLeftOverMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "ApplicationUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ApplicationUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
