using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Parola en az 5 karakter en fazla 20 karakter olmalıdır")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Parolalar uyuşmuyor")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassord { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; } = string.Empty;
    }
}