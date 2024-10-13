using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int TotalCount { get; set; }
    }

}
