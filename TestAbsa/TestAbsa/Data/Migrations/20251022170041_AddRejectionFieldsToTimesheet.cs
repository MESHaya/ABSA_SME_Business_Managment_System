using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAbsa.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionFieldsToTimesheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "TimesheetEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectedByManagerId",
                table: "TimesheetEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "TimesheetEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "TimesheetEntries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "RejectedByManagerId",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "TimesheetEntries");
        }
    }
}
