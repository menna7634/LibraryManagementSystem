using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Checkout
    {
        public int Id { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }

        //relationships
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int BookCopyId { get; set; }
        public BookCopy BookCopy { get; set; }

        public int ReturnId { get; set; }
        public Return Return { get; set; }

        public ICollection<Penalty> Penalties { get; set; }= new List<Penalty>();
    }
}
