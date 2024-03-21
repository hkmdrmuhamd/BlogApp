using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Parola en az 5 karakter en fazla 20 karakter olmalıdır")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}