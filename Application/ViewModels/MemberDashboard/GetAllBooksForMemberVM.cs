using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.MemberDashboard
{
    public class GetAllBooksForMemberVM
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public List<string> Genres { get; set; }
        public int? PublisherId { get; set; }
        public string PublisherName { get; set; }
        public bool IsInWishlist { get; set; } = false;

    }
}
