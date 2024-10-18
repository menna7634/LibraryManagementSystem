using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class WishlistBook
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }

}
