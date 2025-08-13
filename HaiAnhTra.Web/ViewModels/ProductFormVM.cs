using System.ComponentModel.DataAnnotations;
using HaiAnhTra.Web.Models;

namespace HaiAnhTra.Web.ViewModels
{
    public class ProductFormVM
    {
        public Guid Id { get; set; }
        [Required, StringLength(160)] public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public ProductType Type { get; set; } = ProductType.Tea;
        [Range(0, 1_000_000)] public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Origin { get; set; }
        public string? FlavorNotes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
