using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        //relationships
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
