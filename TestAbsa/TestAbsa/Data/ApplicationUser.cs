using Microsoft.AspNetCore.Identity;
using TestAbsa.Data.Models;
using TestAbsa.Data;
using System.ComponentModel.DataAnnotations;

namespace TestAbsa.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Role selection
        public string UserRole { get; set; } = "Employee"; // "Manager" or "Employee"

        // Approval system for employees
        public bool IsApproved { get; set; } = false;
        public string? ApprovedByManagerId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Rejected 
        public bool IsRejected { get; set; }
        public string? RejectedByManagerId { get; set; }
        public DateTime? RejectedDate { get; set; }
        public ApplicationUser? RejectedByManager { get; set; }

        // Navigation property for the manager who approved
        public virtual ApplicationUser? ApprovedByManager { get; set; }

        // --- Organization Fields ---
        [Required] // Ensure every user must belong to an organization
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!; // Navigation property

        // --- Future Account Status Field ---
        // Field for whether the user is still active or not
        public bool IsActive { get; set; } = true;
    }

}
