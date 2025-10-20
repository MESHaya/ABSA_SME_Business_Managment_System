using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

// FIX: Standardizing the namespace to match ApplicationDbContext's using statement
namespace TestAbsa.Data.Models
{
    // --- Product/Stock Item Model ---
    // Represents a unique product that is stored in inventory.
    public class Product
    {
        public int Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        // FIX: Renamed from Quantity to CurrentStock for consistency with service logic
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int CurrentStock { get; set; } // Current stock level

        [Range(1, int.MaxValue, ErrorMessage = "Minimum level must be at least 1")]
        public int MinLevel { get; set; } // Low stock threshold

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; } // Unit selling price

        // Foreign Key to Supplier
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }

    // --- Supplier Model ---
    public class Supplier
    {
        public int Id { get; set; } // Primary Key (using standard 'Id' convention)

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        public List<Product> Products { get; set; } = new List<Product>(); // Navigation property
    }

    // --- Stock Request Model ---
    // Represents a request for stock to be either purchased or moved.
    public class StockRequest
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public string EmployeeId { get; set; } = string.Empty; // Use UserId from Identity
        public string EmployeeName { get; set; } = string.Empty;

        // Foreign Key to the Product being requested
        [Required(ErrorMessage = "Please select a product.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
        public int Quantity { get; set; } = 1;

        public DateTime RequestDate { get; set; } = DateTime.UtcNow; // Changed to UtcNow for consistency

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

        public string? Notes { get; set; }

        // Added Manager fields to match IInventoryService signature
        public string? ManagerId { get; set; } // Nullable, set upon review
        public string? ManagerName { get; set; } // Nullable, set upon review
        public DateTime? ReviewDate { get; set; } 
    }
}
