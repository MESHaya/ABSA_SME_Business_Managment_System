using System;
using System.Collections.Generic;
using System.Linq;

namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; } // Primary Key
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Overdue
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation property
        public List<InvoiceItem> Items { get; set; } = new();

        // Not mapped to DB, just computed
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
    }

    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; } // Primary Key
        public int InvoiceId { get; set; }     // Foreign Key
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation
        public Invoice? Invoice { get; set; }
    }
}
