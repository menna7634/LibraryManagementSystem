using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
   static public class Generateotp
    {
       static public string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); 
        }
    }
}
