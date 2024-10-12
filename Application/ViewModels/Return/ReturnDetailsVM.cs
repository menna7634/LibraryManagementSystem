using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Return
{
    public class ReturnDetailsVM
    {
        public int CheckoutId { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; }
        public int BookCopyId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } 

        // Calculate overdue days
        public int OverdueDays => (ReturnDate.HasValue && ReturnDate > DueDate)
                                  ? (ReturnDate.Value - DueDate).Days
                                  : 0;
        public bool IsOverdue => OverdueDays > 0;
    }

}
