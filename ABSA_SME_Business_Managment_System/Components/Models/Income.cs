using System;
using System.ComponentModel.DataAnnotations;

namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Income
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string? Description { get; set; }
    }
}
