namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // e.g. "Manager", "Employee"
    }
}
