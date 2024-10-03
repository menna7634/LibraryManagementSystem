using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class SignInResponse
    {
        public bool Succeeded { get; set; }
        public  string?  Message { get; set; }
        public  string? Token { get; set; } 
    }

}
