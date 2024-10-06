using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Return
    {
        public int Id { get; set; }
        public DateTime ReturnDate { get; set; }

        public int CheckoutId { get; set; }
        public Checkout Checkout { get; set; }
    }
}
