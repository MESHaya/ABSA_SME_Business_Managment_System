using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAbsa.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvoiceItemTotalColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "InvoiceItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "InvoiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
