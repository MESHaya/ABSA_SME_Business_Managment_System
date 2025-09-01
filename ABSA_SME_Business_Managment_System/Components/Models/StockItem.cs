namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class StockItem
    {
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public int StockLevel { get; set; }

        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
