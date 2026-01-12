using System.ComponentModel.DataAnnotations;

namespace Bai4.ViewModels
{
   public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}