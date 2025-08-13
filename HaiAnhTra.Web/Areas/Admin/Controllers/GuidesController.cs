using HaiAnhTra.Web.Data;
using HaiAnhTra.Web.Models;
using HaiAnhTra.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GuidesController : Controller
    {
        private readonly AppDbContext _db;
        public GuidesController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? q, bool? published, int page = 1, int pageSize = 15)
        {
            var data = _db.Guides.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) data = data.Where(x => x.Title.Contains(q));
            if (published.HasValue) data = data.Where(x => x.IsPublished == published);
            data = data.OrderByDescending(x => x.PublishedAt);
            var list = await PaginatedList<Guide>.CreateAsync(data, page, pageSize);
            ViewBag.Q = q; ViewBag.Published = published;
            return View(list);
        }

        public IActionResult Create() => View(new Guide { IsPublished = true, PublishedAt = DateTime.UtcNow });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guide m)
        {
            if (!ModelState.IsValid) return View(m);
            if (string.IsNullOrWhiteSpace(m.Slug))
                m.Slug = m.Title?.Trim().ToLower().Replace(" ", "-");
            _db.Add(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var m = await _db.Guides.FindAsync(id);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Guide m)
        {
            if (id != m.Id) return NotFound();
            if (!ModelState.IsValid) return View(m);
            if (string.IsNullOrWhiteSpace(m.Slug))
                m.Slug = m.Title?.Trim().ToLower().Replace(" ", "-");
            _db.Update(m);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var m = await _db.Guides.FindAsync(id);
            if (m == null) return NotFound();
            m.IsPublished = !m.IsPublished;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.Guides.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (m == null) return NotFound();
            return View(m);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var m = await _db.Guides.FindAsync(id);
            if (m != null)
            {
                _db.Guides.Remove(m);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
