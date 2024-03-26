using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostCreateViewModel
    {
        public int PostId { get; set; }
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
        public string? Image { get; set; }

        public bool IsActive { get; set; }

        public List<Tag> Tags { get; set; } = new();
    }
}