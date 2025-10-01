using Microsoft.EntityFrameworkCore;
using ABSA_SME_Business_Managment_System.Components.Models;

namespace ABSA_SME_Business_Managment_System
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Only models that currently exist in your Models folder
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Customer> Customers { get; set; } // Added Customer table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: unique index on InvoiceNumber if the Invoice class has one
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            // Ensure primary keys are recognized
            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);
        }
    }
}
