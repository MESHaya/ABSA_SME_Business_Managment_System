using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAbsa.Migrations
{
    /// <inheritdoc />
    public partial class AddFullMultiTenancyAndFixCycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RejectedByManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_AspNetUsers_EmployeeId",
                table: "TimesheetEntries");

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

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "InvoiceItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FiredByManagerId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FiredDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFired",
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
                name: "IX_Invoices_OrganizationId",
                table: "Invoices",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_OrganizationId",
                table: "InvoiceItems",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_OrganizationId",
                table: "Expenses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OrganizationId",
                table: "Customers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FiredByManagerId",
                table: "AspNetUsers",
                column: "FiredByManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_FiredByManagerId",
                table: "AspNetUsers",
                column: "FiredByManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RejectedByManagerId",
                table: "AspNetUsers",
                column: "RejectedByManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Organizations_OrganizationId",
                table: "Customers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Organizations_OrganizationId",
                table: "Expenses",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Organizations_OrganizationId",
                table: "InvoiceItems",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Organizations_OrganizationId",
                table: "Invoices",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
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
                name: "FK_TimesheetEntries_AspNetUsers_EmployeeId",
                table: "TimesheetEntries",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
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
                name: "FK_AspNetUsers_AspNetUsers_FiredByManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RejectedByManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Organizations_OrganizationId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Organizations_OrganizationId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Organizations_OrganizationId",
                table: "InvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Organizations_OrganizationId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

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
                name: "FK_TimesheetEntries_AspNetUsers_EmployeeId",
                table: "TimesheetEntries");

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
                name: "IX_Invoices_OrganizationId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_OrganizationId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_OrganizationId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Customers_OrganizationId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FiredByManagerId",
                table: "AspNetUsers");

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
                name: "OrganizationId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FiredByManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FiredDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsFired",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RejectedByManagerId",
                table: "AspNetUsers",
                column: "RejectedByManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_AspNetUsers_EmployeeId",
                table: "TimesheetEntries",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
