using HaiAnhTra.Web.Data;
using HaiAnhTra.Web.Models;
using HaiAnhTra.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var teas = await _db.Products.AsNoTracking()
                .Where(p => p.IsActive && p.Type == ProductType.Tea)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
                .Take(6).ToListAsync();

            var tools = await _db.Products.AsNoTracking()
                .Where(p => p.IsActive && p.Type == ProductType.Tool)
                .OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
                .Take(6).ToListAsync();

            var guides = await _db.Guides.AsNoTracking()
                .Where(g => g.IsPublished)
                .OrderByDescending(g => g.PublishedAt)
                .Take(3).ToListAsync();

            return View(new HomeVM
            {
                FeaturedTeas = teas,
                FeaturedTools = tools,
                LatestGuides = guides
            });
        }

        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();
    }
}
