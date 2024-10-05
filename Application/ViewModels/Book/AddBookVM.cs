using Application.Models;
using Application.ViewModels.Publisher;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Book
{
    public class AddBookVM
    {
        public string ISBN { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }


        public int Number { get; set; }
        public string Description { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();

        // Selected publisher ID
        [Required]
        public int PublisherId { get; set; }

        // For dropdown lists in the form
        public IEnumerable<Genre> AvailableGenres { get; set; } = new List<Genre>();
        public IEnumerable<ViewPublisherVM> AvailablePublishers { get; set; } = new List<ViewPublisherVM>();
    }
}

