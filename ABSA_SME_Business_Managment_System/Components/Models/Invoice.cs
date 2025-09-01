namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);
        public string CustomerName { get; set; } = string.Empty;
        public List<InvoiceItem> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
        public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Overdue
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
