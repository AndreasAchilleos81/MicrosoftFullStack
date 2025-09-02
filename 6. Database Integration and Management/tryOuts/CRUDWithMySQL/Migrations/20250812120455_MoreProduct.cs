using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUDWithMySQL.Migrations
{
    /// <inheritdoc />
    public partial class MoreProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ArrivalDate",
                table: "Product",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "Product");
        }
    }
}
