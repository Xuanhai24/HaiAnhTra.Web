using HaiAnhTra.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Guide> Guides => Set<Guide>();
        public DbSet<Lead> Leads => Set<Lead>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Product>()
                .HasIndex(x => x.Slug)
                .IsUnique(false);

            b.Entity<Guide>()
                .HasIndex(x => x.Slug)
                .IsUnique(false);

            b.Entity<Lead>()
                .HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
