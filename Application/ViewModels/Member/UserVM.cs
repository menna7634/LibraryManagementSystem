using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Member
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime JoinedAt { get; set; }
        public Gender Gender { get; set; }
        public required string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}
