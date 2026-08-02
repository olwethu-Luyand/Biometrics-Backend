using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiometricClockingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeFingerprintFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Employees",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<bool>(
                name: "FingerprintEnrolled",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FingerprintEnrolledAt",
                table: "Employees",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FingerprintTemplate",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScannerDeviceId",
                table: "Employees",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FingerprintEnrolled",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FingerprintEnrolledAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FingerprintTemplate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ScannerDeviceId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Employees",
                newName: "Password");
        }
    }
}
