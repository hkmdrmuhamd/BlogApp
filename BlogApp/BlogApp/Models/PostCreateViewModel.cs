using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Başlık alanı gereklidir.")]
        [Display(Name = "Başlık")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "İçerik alanı gereklidir.")]
        [Display(Name = "İçerik")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Açıklama alanı gereklidir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Url alanı gereklidir.")]
        [Display(Name = "Url")]
        public string? Url { get; set; }

        [Display(Name = "Resim")]
        public string Image { get; set; } = string.Empty;
    }
}