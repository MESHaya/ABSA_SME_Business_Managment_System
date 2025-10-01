namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class StockItem
    {
        public int StockItemId { get; set; } // Primary Key
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int MinLevel { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
