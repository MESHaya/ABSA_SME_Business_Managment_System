using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TestAbsa.Data.Models
{
    public class Organization
    {
        public int Id { get; set; } // Primary Key for the organization (Tenant ID)

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty; // Full company name

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation property to link users to this organization (Optional, but useful)
        public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();

        // Navigation property for other organization-specific entities (Optional, but good practice)
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}