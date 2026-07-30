using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employeecode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fullname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    passwordhash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    jobtitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    employmenttype = table.Column<int>(type: "integer", nullable: true),
                    hiredate = table.Column<DateOnly>(type: "date", nullable: true),
                    workschedule = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    managerid = table.Column<Guid>(type: "uuid", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    lastlogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                    table.ForeignKey(
                        name: "FK_employees_employees_managerid",
                        column: x => x.managerid,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "attendancerecords",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employeeid = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    clockin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    clockout = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    clockinmethod = table.Column<int>(type: "integer", nullable: true),
                    clockoutmethod = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    hoursworked = table.Column<decimal>(type: "numeric", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendancerecords", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendancerecords_employees_employeeid",
                        column: x => x.employeeid,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passwordresettokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employeeid = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expiresat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passwordresettokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_passwordresettokens_employees_employeeid",
                        column: x => x.employeeid,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendancerecords_employeeid_date",
                table: "attendancerecords",
                columns: new[] { "employeeid", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_email",
                table: "employees",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_employeecode",
                table: "employees",
                column: "employeecode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_managerid",
                table: "employees",
                column: "managerid");

            migrationBuilder.CreateIndex(
                name: "IX_passwordresettokens_employeeid",
                table: "passwordresettokens",
                column: "employeeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendancerecords");

            migrationBuilder.DropTable(
                name: "passwordresettokens");

            migrationBuilder.DropTable(
                name: "employees");
        }
    }
}
