
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class VerifyOtpViewModel
    {
        public required string Email { get; set; }
        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, ErrorMessage = "OTP must be 6 digits long.", MinimumLength = 6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be numeric and 6 digits.")]
        public required string Otp { get; set; }
    }
}
