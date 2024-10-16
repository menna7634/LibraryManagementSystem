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
        public  string? UserId { get; set; }

        public  string? Token { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W])[A-Za-z\d\W]{8,}$", ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public  string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public  string ConfirmPassword { get; set; }
    }

}
