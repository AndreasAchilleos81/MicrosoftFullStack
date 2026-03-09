using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_Name",
                table: "InventoryItems",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_Name",
                table: "InventoryItems");
        }
    }
}
