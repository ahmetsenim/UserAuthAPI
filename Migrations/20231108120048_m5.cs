using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class m5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempPasswords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "0"),
                    PasswordTemp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumberOfAttempts = table.Column<int>(type: "int", nullable: false, defaultValueSql: "0"),
                    ValidityDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PasswordReset = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempPasswords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempPasswords");
        }
    }
}
