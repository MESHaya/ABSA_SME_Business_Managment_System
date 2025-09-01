namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty; // e.g. "Sale" or "Expense"
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // e.g. "Cash", "Card"
        public string Category { get; set; } = string.Empty; // e.g. "Stock Purchase"
        public string CreatedBy { get; set; } = string.Empty; // employee/manager name or ID
        public string? ReferenceNumber { get; set; }
    }
}
