using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Customer
    {
        [Key] //marks as primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment in DB
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;
    }
}
