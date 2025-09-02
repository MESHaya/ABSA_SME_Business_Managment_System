namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string AssignedTo { get; set; }

        public string Description { get; set; }
    }
}
