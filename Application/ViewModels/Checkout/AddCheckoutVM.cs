using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.ViewModels.Checkout
{
    public class AddCheckoutVM
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please select a book.")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Please select a book copy.")]
        public int BookCopyID { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Please provide a due date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [FutureDate(ErrorMessage = "Due date must be a future date.")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Checkout Date")]
        [DataType(DataType.Date)]
        public DateTime CheckoutDate { get; set; } = DateTime.Now;

        public IEnumerable<SelectListItem>? Books { get; set; }
        public IEnumerable<SelectListItem>? BookCopies { get; set; }
        public IEnumerable<SelectListItem>? Users { get; set; }

        public string? BookName { get; set; }
        public string? UserName { get; set; }
    }




}
