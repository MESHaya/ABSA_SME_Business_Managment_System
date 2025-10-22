using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TestAbsa.Data.Models
{
    // --- Leave Request Model ---
    // Represents an employee's request for time off
    public class LeaveRequest
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Leave type is required")]
        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty; // Annual, Sick, Family, etc.

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public string? ManagerId { get; set; } // Set upon review

        public DateTime? ReviewedDate { get; set; }

        [StringLength(500)]
        public string? ManagerComments { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

       
        public int TotalDays { get; set; }

        // Navigation properties
        public ApplicationUser? Employee { get; set; }
        public ApplicationUser? Manager { get; set; }
    }

    // --- Timesheet Entry Model ---
    // Represents daily work hours logged by an employee
    public class TimesheetEntry
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Work date is required")]
        public DateTime WorkDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.5, 24, ErrorMessage = "Hours must be between 0.5 and 24")]
        [Column(TypeName = "decimal(4,1)")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task type is required")]
        [StringLength(50)]
        public string TaskType { get; set; } = string.Empty; // Development, Meeting, Testing, etc.

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsApproved { get; set; } = false;

        public string? ApprovedByManagerId { get; set; } // Set upon approval

        public DateTime? ApprovedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("EmployeeId")]
        public virtual ApplicationUser? Employee { get; set; }


        public ApplicationUser? ApprovedByManager { get; set; }

        public bool IsRejected { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string? RejectedByManagerId { get; set; }
    }

    // --- Employee Performance Review Model ---
    // Optional: Consider adding this for complete HR functionality
    /*
    public class PerformanceReview
    {
        public int Id { get; set; }
        
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        
        [Required]
        public string ReviewerId { get; set; } = string.Empty; // Manager conducting review
        
        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ReviewPeriodStart { get; set; }
        
        [Required]
        public DateTime ReviewPeriodEnd { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int OverallRating { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Strengths { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string AreasForImprovement { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Goals { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? EmployeeComments { get; set; }
        
        public bool EmployeeAcknowledged { get; set; } = false;
        
        public DateTime? EmployeeAcknowledgedDate { get; set; }
        
        // Navigation properties
        public ApplicationUser? Employee { get; set; }
        public ApplicationUser? Reviewer { get; set; }
    }
    */
}