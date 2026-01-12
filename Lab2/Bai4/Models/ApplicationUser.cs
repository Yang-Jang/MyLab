using Microsoft.AspNetCore.Identity;
using System;

namespace Bai4.Models
{
    public class ApplicationUser : IdentityUser
    {
       public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
