using System;
using System.ComponentModel.DataAnnotations;

namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee Name is required")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Leave Type is required")]
        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty; // e.g., Annual, Sick

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [StringLength(250)]
        public string? Reason { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied
    }
}
