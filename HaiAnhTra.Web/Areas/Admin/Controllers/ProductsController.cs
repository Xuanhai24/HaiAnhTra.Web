using HaiAnhTra.Web.Data;
using HaiAnhTra.Web.Helpers;
using HaiAnhTra.Web.Models;
using HaiAnhTra.Web.Services;
using HaiAnhTra.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IImageStorage _storage;
        public ProductsController(AppDbContext db, IImageStorage storage) { _db = db; _storage = storage; }

        public async Task<IActionResult> Index(string? type, string? q, int page = 1, int pageSize = 12)
        {
            var data = _db.Products.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<ProductType>(type, true, out var pt))
                data = data.Where(x => x.Type == pt);
            if (!string.IsNullOrWhiteSpace(q))
                data = data.Where(x => x.Name.Contains(q));
            data = data.OrderBy(x => x.SortOrder).ThenBy(x => x.Name);

            var list = await PaginatedList<Product>.CreateAsync(data, Math.Max(page, 1), Math.Clamp(pageSize, 6, 60));
            ViewBag.Type = type; ViewBag.Q = q;
            return View(list);
        }

        public IActionResult Create() => View(new ProductFormVM());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var img = await _storage.SaveAsync(vm.ImageFile);
            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = vm.Name,
                Slug = vm.Slug,
                Type = vm.Type,
                Price = vm.Price,
                Unit = vm.Unit,
                ShortDescription = vm.ShortDescription,
                Description = vm.Description,
                Origin = vm.Origin,
                FlavorNotes = vm.FlavorNotes,
                ImageUrl = img ?? vm.ImageUrl,
                IsActive = vm.IsActive,
                SortOrder = vm.SortOrder
            };
            p.EnsureSlug();
            _db.Add(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            var vm = new ProductFormVM
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Type = p.Type,
                Price = p.Price,
                Unit = p.Unit,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                Origin = p.Origin,
                FlavorNotes = p.FlavorNotes,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductFormVM vm)
        {
            if (id != vm.Id) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = vm.Name; p.Slug = vm.Slug; p.Type = vm.Type; p.Price = vm.Price; p.Unit = vm.Unit;
            p.ShortDescription = vm.ShortDescription; p.Description = vm.Description; p.Origin = vm.Origin;
            p.FlavorNotes = vm.FlavorNotes; p.IsActive = vm.IsActive; p.SortOrder = vm.SortOrder;

            if (vm.ImageFile != null)
            {
                var img = await _storage.SaveAsync(vm.ImageFile);
                if (img != null) p.ImageUrl = img;
            }

            p.EnsureSlug();
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            p.IsActive = !p.IsActive;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p != null)
            {
                _db.Products.Remove(p);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
