using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq; // Add this

namespace TestAbsa.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string Phone { get; set; } = "";


        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        [Required]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }

    public enum InvoiceStatus
    {
        Draft = 0,
        Sent = 1,
        Paid = 2,
        Overdue = 3,
        Cancelled = 4
    }

    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? PaidDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = "";

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        // Computed fields for the dashboard
        [NotMapped]
        public decimal TotalAmount => InvoiceItems?.Sum(i => i.Total) ?? 0;

        [NotMapped]
        public bool IsPaid => Status == InvoiceStatus.Paid;

        [NotMapped]
        public DateTime DateIssued => CreatedDate;

        [Required]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }


    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = "";

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        // This should be [NotMapped] since it's computed
        [NotMapped]
        public decimal Total => Quantity * UnitPrice;

        [Required]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }
    public enum ExpenseCategory
    {
        None = -1,  // Default unselected
        Office = 0,
        Travel = 1,
        Utilities = 2,
        Supplies = 3,
        Marketing = 4,
        Salaries = 5,
        Other = 6
    }

    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ExpenseCategory Category { get; set; }

        public string? Vendor { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
    }
} // Only one closing brace