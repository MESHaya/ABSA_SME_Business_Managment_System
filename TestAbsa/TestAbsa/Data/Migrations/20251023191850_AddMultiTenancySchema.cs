using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAbsa.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "TimesheetEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "StockRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "LeaveRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Name", "RegistrationDate" },
                values: new object[] { "Default SME Client", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            // 2. Update ALL existing rows (which currently have OrganizationId = 0) to OrganizationId = 1.
            //    This fixes the foreign key violation for the existing data.
            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [TimesheetEntries] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [Suppliers] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [StockRequests] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [PurchaseOrders] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [Products] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");
            migrationBuilder.Sql("UPDATE [LeaveRequests] SET [OrganizationId] = 1 WHERE [OrganizationId] = 0;");


            migrationBuilder.CreateIndex(
                name: "IX_TimesheetEntries_OrganizationId",
                table: "TimesheetEntries",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_OrganizationId",
                table: "Suppliers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockRequests_OrganizationId",
                table: "StockRequests",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrganizationId",
                table: "PurchaseOrders",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationId",
                table: "Products",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_OrganizationId",
                table: "LeaveRequests",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Organizations_OrganizationId",
                table: "LeaveRequests",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Organizations_OrganizationId",
                table: "Products",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Organizations_OrganizationId",
                table: "PurchaseOrders",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockRequests_Organizations_OrganizationId",
                table: "StockRequests",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Organizations_OrganizationId",
                table: "Suppliers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_Organizations_OrganizationId",
                table: "TimesheetEntries",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Organizations_OrganizationId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Organizations_OrganizationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Organizations_OrganizationId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_StockRequests_Organizations_OrganizationId",
                table: "StockRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Organizations_OrganizationId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_Organizations_OrganizationId",
                table: "TimesheetEntries");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_TimesheetEntries_OrganizationId",
                table: "TimesheetEntries");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_OrganizationId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StockRequests_OrganizationId",
                table: "StockRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OrganizationId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganizationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_OrganizationId",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "StockRequests");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AspNetUsers");
        }
    }
}
