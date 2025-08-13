using System.ComponentModel.DataAnnotations;

namespace HaiAnhTra.Web.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(160)]
        public string Name { get; set; } = default!;

        [StringLength(180)]
        public string? Slug { get; set; }

        public ProductType Type { get; set; } = ProductType.Tea;

        [Range(0, 1_000_000)]
        public decimal? Price { get; set; } // chỉ hiển thị (không thanh toán)

        [StringLength(40)]
        public string? Unit { get; set; }  // vd: "100g", "ấm", "bộ"

        [StringLength(240)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [StringLength(120)]
        public string? Origin { get; set; } // xuất xứ

        [StringLength(160)]
        public string? FlavorNotes { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        // thuận tiện SEO
        public void EnsureSlug()
        {
            if (string.IsNullOrWhiteSpace(Slug))
                Slug = Name?.Trim().ToLower().Replace(" ", "-");
        }
    }
}
