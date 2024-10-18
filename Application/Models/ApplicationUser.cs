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

        //relationships
        public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

        public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

        public Wishlist Wishlist { get; set; }


    }
}
