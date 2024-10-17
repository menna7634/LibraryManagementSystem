using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Book
{
    public class ViewBookVM
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public List<string> Genres { get; set; }
        public int? PublisherId { get; set; }
        public string PublisherName { get; set; }
        public DateTime Date { get; set; }
        public ICollection<ViewBookCopyVM?>? BookCopies { get; set; }
        
    }
}
