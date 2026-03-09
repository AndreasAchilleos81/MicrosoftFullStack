using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerName",
                table: "Orders",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DatePlaced",
                table: "Orders",
                column: "DatePlaced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_CustomerName",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Order_DatePlaced",
                table: "Orders");
        }
    }
}
