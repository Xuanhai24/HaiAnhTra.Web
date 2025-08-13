using HaiAnhTra.Web.Data;
using HaiAnhTra.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) { _db = db; }

        // GET: /Products?type=Tea|Tool&q=...
        public async Task<IActionResult> Index(string? type, string? q)
        {
            var products = _db.Products.AsNoTracking().Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<ProductType>(type, true, out var pt))
                products = products.Where(p => p.Type == pt);

            if (!string.IsNullOrWhiteSpace(q))
                products = products.Where(p => p.Name.Contains(q));

            var list = await products
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
            return View(list);
        }

        // GET: /Products/Details/{slug}
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var product = await _db.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (product == null && Guid.TryParse(slug, out var gid))
                product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == gid);

            if (product == null) return NotFound();
            return View(product);
        }

        // POST: /Products/CreateLead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLead(Lead model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu lỗi, quay lại trang chi tiết sản phẩm (nếu có)
                string backSlug = null;
                if (model.ProductId.HasValue)
                {
                    var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.ProductId.Value);
                    backSlug = p?.Slug ?? p?.Id.ToString();
                }
                if (!string.IsNullOrEmpty(backSlug))
                    return RedirectToAction("Details", new { slug = backSlug });
                return RedirectToAction("Index", "Contact");
            }
            _db.Leads.Add(model);
            await _db.SaveChangesAsync();
            TempData["LeadOk"] = "Đã nhận thông tin. Chúng tôi sẽ liên hệ sớm!";
            return RedirectToAction("Thanks", "Contact");
        }
    }
}