using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invest.Api.Migrations
{
    public partial class AddUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    username = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false),

                    password_hash = table.Column<string>(
                        name: "password_hash",
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false),

                    role = table.Column<string>(
                        name: "role",
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    is_active = table.Column<bool>(
                        name: "is_active",
                        type: "bit",
                        nullable: false,
                        defaultValue: true),

                    created_at = table.Column<DateTime>(
                        name: "created_at",
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"),

                    updated_at = table.Column<DateTime>(
                        name: "updated_at",
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}