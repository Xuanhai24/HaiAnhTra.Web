using System.ComponentModel.DataAnnotations;

namespace HaiAnhTra.Web.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = default!;

        [Required, StringLength(30)]
        public string Phone { get; set; } = default!;

        [EmailAddress, StringLength(160)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public LeadStatus Status { get; set; } = LeadStatus.New;

        [StringLength(500)]
        public string? AdminNote { get; set; }
    }
}
