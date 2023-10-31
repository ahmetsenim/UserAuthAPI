using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidityDate",
                table: "RefreshTokens",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidityDate",
                table: "OTPs",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidityDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ValidityDate",
                table: "OTPs");
        }
    }
}
