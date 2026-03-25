using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSnap.Api.Migrations
{
    /// <inheritdoc />
    public partial class ManyPortfolioUsersPerSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_PortfolioUsers_PortfolioUserId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_PortfolioUserId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "PortfolioUserId",
                table: "Skills");

            migrationBuilder.CreateTable(
                name: "PortfolioUserSkill",
                columns: table => new
                {
                    PortfolioUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioUserSkill", x => new { x.PortfolioUsersId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_PortfolioUserSkill_PortfolioUsers_PortfolioUsersId",
                        column: x => x.PortfolioUsersId,
                        principalTable: "PortfolioUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioUserSkill_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioUserSkill_SkillsId",
                table: "PortfolioUserSkill",
                column: "SkillsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioUserSkill");

            migrationBuilder.AddColumn<int>(
                name: "PortfolioUserId",
                table: "Skills",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_PortfolioUserId",
                table: "Skills",
                column: "PortfolioUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_PortfolioUsers_PortfolioUserId",
                table: "Skills",
                column: "PortfolioUserId",
                principalTable: "PortfolioUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
