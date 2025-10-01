using System;
using System.ComponentModel.DataAnnotations;

namespace ABSA_SME_Business_Managment_System.Components.Models
{
    public class Income
    {
        public int IncomeId { get; set; } // Primary Key

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(250)]
        public string? Description { get; set; }
    }
}
