namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class StockRequest
    {
        public int Id { get; set; }  // unique request ID
        public string EmployeeName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied
        public string? Notes { get; set; }
    }
}
