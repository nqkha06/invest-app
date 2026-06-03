using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockExchange.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    symbol = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    company_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    sector = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    current_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stocks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "User"),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_prices_day",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_id = table.Column<long>(type: "INTEGER", nullable: false),
                    open_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    high_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    low_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    close_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    volume = table.Column<long>(type: "INTEGER", nullable: false),
                    trading_date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_prices_day", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_prices_day_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_prices_minute",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_id = table.Column<long>(type: "INTEGER", nullable: false),
                    open_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    high_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    low_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    close_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    volume = table.Column<long>(type: "INTEGER", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_prices_minute", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_prices_minute_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_simulations",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stock_id = table.Column<long>(type: "INTEGER", nullable: false),
                    algorithm_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    volatility = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    trend_factor = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    min_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    max_price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    update_speed = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    jump_probability = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_simulations", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_simulations_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_watchlists",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<long>(type: "INTEGER", nullable: false),
                    stock_id = table.Column<long>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_watchlists", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_watchlists_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_watchlists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_prices_day_stock_id_trading_date",
                table: "stock_prices_day",
                columns: new[] { "stock_id", "trading_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_prices_minute_stock_id_recorded_at",
                table: "stock_prices_minute",
                columns: new[] { "stock_id", "recorded_at" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_simulations_stock_id",
                table: "stock_simulations",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_stocks_sector",
                table: "stocks",
                column: "sector");

            migrationBuilder.CreateIndex(
                name: "IX_stocks_symbol",
                table: "stocks",
                column: "symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_watchlists_stock_id",
                table: "user_watchlists",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_watchlists_user_id_stock_id",
                table: "user_watchlists",
                columns: new[] { "user_id", "stock_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_prices_day");

            migrationBuilder.DropTable(
                name: "stock_prices_minute");

            migrationBuilder.DropTable(
                name: "stock_simulations");

            migrationBuilder.DropTable(
                name: "user_watchlists");

            migrationBuilder.DropTable(
                name: "stocks");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
