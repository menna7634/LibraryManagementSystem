using Application.Enums;
using Microsoft.AspNetCore.Identity;

namespace Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Gender Gender { get; set; }
        public required string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime JoinedAT { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    }
}
