using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data.Models;

namespace TestAbsa.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Inventory Management DbSets
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        // HR Management DbSets
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Multi-Tenancy Configuration ---
            // Enforce the foreign key relationship for organization
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Organization)
                .WithMany(o => o.Employees) // Link to the Employees collection in Organization model
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            // Use Restrict or NoAction to prevent accidentally deleting an entire org if users exist.

            // Configure the Product-Organization relationship
            builder.Entity<Product>()
                .HasOne(p => p.Organization)
                .WithMany(o => o.Products)
                .HasForeignKey(p => p.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier -> Organization
            builder.Entity<Supplier>()
                .HasOne(s => s.Organization)
                .WithMany() // Assuming we need a collection back on Organization model for Suppliers
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // StockRequest -> Organization 
            builder.Entity<StockRequest>()
                .HasOne(sr => sr.Organization)
                .WithMany()
                .HasForeignKey(sr => sr.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseOrder -> Organization
            builder.Entity<PurchaseOrder>()
                .HasOne(po => po.Organization)
                .WithMany()
                .HasForeignKey(po => po.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // TimesheetEntry -> Organization
            builder.Entity<TimesheetEntry>()
                .HasOne(t => t.Organization)
                .WithMany()
                .HasForeignKey(t => t.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // LeaveRequest -> Organization 
            builder.Entity<LeaveRequest>()
                .HasOne(l => l.Organization)
                .WithMany()
                .HasForeignKey(l => l.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);


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


        }
    }
}