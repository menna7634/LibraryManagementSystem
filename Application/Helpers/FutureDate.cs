
using System.ComponentModel.DataAnnotations;

namespace Application.Helpers
{

	public class FutureDateAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
		{
			if (value is DateTime date)
			{
				if (date <= DateTime.Now)
				{
					return new ValidationResult(ErrorMessage);
				}
			}
			return ValidationResult.Success!;
		}
	}
}
