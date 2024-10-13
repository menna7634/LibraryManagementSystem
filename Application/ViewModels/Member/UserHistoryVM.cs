using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Member
{
    public class UserHistoryVM
    {
        public string UserName { get; set; }
        public DateTime JoinedAt { get; set; }
        public List<string> HistoryEvents { get; set; } // Event list (e.g., activities)
    }

}
