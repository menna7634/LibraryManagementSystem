using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class BookCopy
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public bool Available { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
