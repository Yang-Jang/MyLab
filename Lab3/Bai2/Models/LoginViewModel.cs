using System.ComponentModel.DataAnnotations;

namespace Bai2.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage =" Vui lòng nhập Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required (ErrorMessage =" Vui lòng nhập Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}