using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class m7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TempPasswords",
                table: "TempPasswords");

            migrationBuilder.RenameTable(
                name: "TempPasswords",
                newName: "ResetPasswordOTPs");

            migrationBuilder.RenameColumn(
                name: "PasswordTemp",
                table: "ResetPasswordOTPs",
                newName: "OTP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordOTPs",
                table: "ResetPasswordOTPs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordOTPs",
                table: "ResetPasswordOTPs");

            migrationBuilder.RenameTable(
                name: "ResetPasswordOTPs",
                newName: "TempPasswords");

            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "TempPasswords",
                newName: "PasswordTemp");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TempPasswords",
                table: "TempPasswords",
                column: "Id");
        }
    }
}
