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

    // --- Purchase Order Model ---
    // Represents an order placed with a supplier to replenish inventory
    public class PurchaseOrder
    {
        public int Id { get; set; } // Primary Key

        // Supplier relationship
        [Required(ErrorMessage = "Please select a supplier.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid supplier.")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        // Product relationship
        [Required(ErrorMessage = "Please select a product.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Quantity tracking
        [Range(1, 100000, ErrorMessage = "Ordered quantity must be between 1 and 100,000.")]
        public int OrderedQuantity { get; set; }

        [Range(0, 100000, ErrorMessage = "Received quantity must be between 0 and 100,000.")]
        public int ReceivedQuantity { get; set; } = 0;

        // Cost tracking
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; } // Calculated: OrderedQuantity * UnitCost

        // Status tracking
        [StringLength(20)]
        public string Status { get; set; } = "Ordered";
        // Possible values: "Ordered", "Shipped", "PartiallyReceived", "Delivered", "Issue", "Cancelled"

        // Date tracking
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        // Manager who created the order
        
        public string ManagerId { get; set; } = string.Empty;

        
        [StringLength(100)]
        public string ManagerName { get; set; } = string.Empty;

        // Issue tracking
        [StringLength(500)]
        public string? IssueNotes { get; set; }

        public DateTime? IssueReportedDate { get; set; }

        // Additional notes
        [StringLength(500)]
        public string? Notes { get; set; }

        // Computed properties (not stored in database)
        [NotMapped]
        public bool IsFullyReceived => ReceivedQuantity >= OrderedQuantity;

        [NotMapped]
        public int RemainingQuantity => OrderedQuantity - ReceivedQuantity;
    }
}