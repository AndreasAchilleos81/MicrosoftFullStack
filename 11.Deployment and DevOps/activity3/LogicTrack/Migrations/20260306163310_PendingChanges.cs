using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicTrack.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserRole",
                table: "IdentityUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserClaimString",
                table: "IdentityUserClaimString");

            migrationBuilder.RenameTable(
                name: "IdentityUserRole",
                newName: "IdentityUserRoles");

            migrationBuilder.RenameTable(
                name: "IdentityUserClaimString",
                newName: "IdentityUserClaims");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserRoles",
                table: "IdentityUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserClaims",
                table: "IdentityUserClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IdentityRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserRoles",
                table: "IdentityUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserClaims",
                table: "IdentityUserClaims");

            migrationBuilder.RenameTable(
                name: "IdentityUserRoles",
                newName: "IdentityUserRole");

            migrationBuilder.RenameTable(
                name: "IdentityUserClaims",
                newName: "IdentityUserClaimString");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserRole",
                table: "IdentityUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserClaimString",
                table: "IdentityUserClaimString",
                column: "Id");
        }
    }
}
