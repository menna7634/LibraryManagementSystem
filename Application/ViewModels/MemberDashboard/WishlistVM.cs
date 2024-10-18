using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.MemberDashboard
{
    public class WishlistVM
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public List<string> Genres { get; set; }

    }
}
