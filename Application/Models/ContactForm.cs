using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
	public class ContactForm
	{
		public int Id { get; set; }
		[Required]
		public required string Name { get; set; }

		[EmailAddress]
		[Required]

		public required string Email { get; set; }
		[Required]

		public required string Message { get; set; }
	}
}
