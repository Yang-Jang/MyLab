using System.ComponentModel.DataAnnotations;

namespace FastFoodOnline.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Custom Fields
        [Required]
        public string FullName { get; set; } = string.Empty; // FullName instead of UserName
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = "Male";
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    
    public class ProfileViewModel
    {
         public string Email { get; set; } = string.Empty;
         [Required]
         public string FullName { get; set; } = string.Empty;
         [Required]
         [Phone]
         public string Phone { get; set; } = string.Empty;
         public string? Address { get; set; }
         public DateTime? DateOfBirth { get; set; }
         public string? Gender { get; set; }
    }
}
