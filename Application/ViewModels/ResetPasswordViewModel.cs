using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public  string UserId { get; set; }

        [Required]
        public  string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }

}
