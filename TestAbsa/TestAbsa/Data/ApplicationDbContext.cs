using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// 1. ADD THIS USING STATEMENT:
using TestAbsa.Data.Models;
using TestAbsa.Data;
// The original using statement below is likely redundant if you moved your models
// using TestAbsa.Data; 

namespace TestAbsa.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Add your new DbSet properties here to create the Inventory Tables
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Any custom model configurations (e.g., unique constraints) go here.

            // Ensure Product.ItemName is required
            builder.Entity<Product>()
                .Property(p => p.ItemName)
                .IsRequired();

            // Set up relationship between Product and Supplier
            builder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .IsRequired(false); // A product might not have a supplier initially

            // ApplicationUser self-referencing relationship for approval
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApprovedByManager)
                .WithMany()
                .HasForeignKey(u => u.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}