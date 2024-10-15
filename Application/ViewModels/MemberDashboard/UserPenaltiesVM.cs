using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;

namespace Application.ViewModels.MemberDashboard
{
    public class UserPenaltiesVM
    {
        public PenaltyType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsPaid { get; set; }
        public int CheckoutId { get; set; }
        public string? BookTitle { get; set; }

        public int BookCopyID {  get; set; }
    }
}
