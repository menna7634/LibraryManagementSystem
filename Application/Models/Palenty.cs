using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        // Relationships
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
