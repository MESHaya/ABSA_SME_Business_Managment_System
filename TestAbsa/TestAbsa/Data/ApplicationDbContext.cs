using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data.Models;

namespace TestAbsa.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Inventory Management DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        // HR Management DbSets
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        // Finance Management DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Product Configuration ---
            builder.Entity<Product>()
                .Property(p => p.ItemName)
                .IsRequired();

            builder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- StockRequest Configuration ---
            builder.Entity<StockRequest>()
                .HasOne(sr => sr.Product)
                .WithMany()
                .HasForeignKey(sr => sr.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- PurchaseOrder Configuration ---
            builder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany()
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>()
                .HasOne(po => po.Product)
                .WithMany()
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- ApplicationUser Configuration ---
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApprovedByManager)
                .WithMany()
                .HasForeignKey(u => u.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- TimesheetEntry Configuration ---
            builder.Entity<TimesheetEntry>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TimesheetEntry>()
                .HasOne(t => t.ApprovedByManager)
                .WithMany()
                .HasForeignKey(t => t.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- LeaveRequest Configuration ---
            builder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LeaveRequest>()
                .HasOne(l => l.Manager)
                .WithMany()
                .HasForeignKey(l => l.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Customer Configuration ---
            builder.Entity<Customer>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Customer>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // --- Invoice Configuration ---
            builder.Entity<Invoice>()
                .Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            builder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- InvoiceItem Configuration ---
            builder.Entity<InvoiceItem>()
                .Property(ii => ii.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // Total is now computed, so don't configure it for the database
            // builder.Entity<InvoiceItem>().Ignore(ii => ii.Total); // Optional: explicitly ignore

            builder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Expense Configuration ---
            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Expense>()
                .Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}