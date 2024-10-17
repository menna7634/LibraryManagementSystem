using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Book
{
    public class ViewBookCopyVM
    {
        public int? Id { get; set; }
        public string Location { get; set; }
        public bool Available { get; set; }
        public int BookId { get; set; }
        public string? BookName { get; set; }



    }
}
