using HaiAnhTra.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Controllers
{
    public class GuidesController : Controller
    {
        private readonly AppDbContext _db;
        public GuidesController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var list = await _db.Guides.AsNoTracking()
                .Where(g => g.IsPublished)
                .OrderByDescending(g => g.PublishedAt)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();
            var guide = await _db.Guides.AsNoTracking().FirstOrDefaultAsync(g => g.Slug == slug);
            if (guide == null && int.TryParse(slug, out var id))
                guide = await _db.Guides.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
            if (guide == null || !guide.IsPublished) return NotFound();
            return View(guide);
        }
    }
}