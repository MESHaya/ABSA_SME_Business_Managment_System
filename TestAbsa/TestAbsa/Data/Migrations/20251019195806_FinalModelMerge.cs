using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace TestAbsa.Migrations
{
    /// <inheritdoc />
    public partial class FinalModelMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Suppliers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "CurrentStock");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StockRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "StockRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "StockRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewDate",
                table: "StockRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "StockRequests");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "StockRequests");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "StockRequests");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Suppliers",
                newName: "SupplierId");

            migrationBuilder.RenameColumn(
                name: "CurrentStock",
                table: "Products",
                newName: "Quantity");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StockRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
