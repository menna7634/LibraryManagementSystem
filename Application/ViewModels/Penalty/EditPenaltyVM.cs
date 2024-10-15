using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;

namespace Application.ViewModels.Penalty
{
    public class EditPenaltyVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public PenaltyType Type { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Issued Date")]
        public DateTime IssuedDate { get; set; }

        public bool IsPaid { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Paid Date")]
        public DateTime? PaidAt { get; set; }

    }
}
