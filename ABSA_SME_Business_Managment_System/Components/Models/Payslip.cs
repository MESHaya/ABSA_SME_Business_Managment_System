namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Payslip
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public string FileUrl { get; set; } = string.Empty; // Path to PDF or download file
    }
}
