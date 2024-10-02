using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public ICollection<BookCopy> BookCopies { get; set; }
        public ICollection<Penalty> Penalties { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}
