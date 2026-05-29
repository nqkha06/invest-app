using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invest.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStockPricesMinuteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_prices_minute",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    open_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    high_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    low_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    close_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_prices_minute", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_prices_minute");
        }
    }
}
