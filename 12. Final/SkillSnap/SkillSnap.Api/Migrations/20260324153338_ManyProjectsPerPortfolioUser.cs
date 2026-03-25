using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillSnap.Api.Migrations
{
    /// <inheritdoc />
    public partial class ManyProjectsPerPortfolioUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PortfolioUsers_PortfolioUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PortfolioUserId",
                table: "Projects");

            migrationBuilder.CreateTable(
                name: "PortfolioUserProject",
                columns: table => new
                {
                    PortfolioUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioUserProject", x => new { x.PortfolioUsersId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_PortfolioUserProject_PortfolioUsers_PortfolioUsersId",
                        column: x => x.PortfolioUsersId,
                        principalTable: "PortfolioUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioUserProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioUserProject_ProjectsId",
                table: "PortfolioUserProject",
                column: "ProjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioUserProject");

            migrationBuilder.AddColumn<int>(
                name: "PortfolioUserId",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PortfolioUserId",
                table: "Projects",
                column: "PortfolioUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PortfolioUsers_PortfolioUserId",
                table: "Projects",
                column: "PortfolioUserId",
                principalTable: "PortfolioUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
