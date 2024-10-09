
using Application.Enums;

namespace Application.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public PenaltyType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        // Relationships
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int CheckoutId { get; set; }
        public Checkout Checkout { get; set; }
    }
}
