using System.ComponentModel.DataAnnotations;

namespace HaiAnhTra.Web.Models
{
    public class Guide
    {
        public int Id { get; set; }

        [Required, StringLength(180)]
        public string Title { get; set; } = default!;

        [StringLength(200)]
        public string? Slug { get; set; }

        public string? Content { get; set; } // có thể là HTML/Markdown

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;
    }
}
