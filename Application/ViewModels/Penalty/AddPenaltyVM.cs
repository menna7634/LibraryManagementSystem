using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;

namespace Application.ViewModels.Penalty
{
    public class AddPenaltyVM
    {
        public PenaltyType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string ApplicationUserId { get; set; }
        public int CheckoutId { get; set; }
    }
}
