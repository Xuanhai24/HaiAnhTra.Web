using HaiAnhTra.Web.Data;
using HaiAnhTra.Web.Models;
using HaiAnhTra.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaiAnhTra.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        public DashboardController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var startOfWeek = now.Date.AddDays(-(int)now.Date.DayOfWeek + (int)DayOfWeek.Monday);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var vm = new AdminDashboardVM
            {
                TotalProducts = await _db.Products.CountAsync(),
                TotalTea = await _db.Products.CountAsync(p => p.Type == ProductType.Tea),
                TotalTools = await _db.Products.CountAsync(p => p.Type == ProductType.Tool),
                TotalGuides = await _db.Guides.CountAsync(),
                PublishedGuides = await _db.Guides.CountAsync(g => g.IsPublished),
                NewLeads24h = await _db.Leads.CountAsync(l => l.CreatedAt >= now.AddDays(-1)),
                LeadsThisWeek = await _db.Leads.CountAsync(l => l.CreatedAt >= startOfWeek),
                LeadsThisMonth = await _db.Leads.CountAsync(l => l.CreatedAt >= startOfMonth),
                ContactedLeads = await _db.Leads.CountAsync(l => l.Status == LeadStatus.Contacted),
                ClosedLeads = await _db.Leads.CountAsync(l => l.Status == LeadStatus.Closed),
            };

            // Leads last 7 days (UTC date)
            var from7 = now.Date.AddDays(-6);
            var last7 = await _db.Leads.AsNoTracking()
                .Where(l => l.CreatedAt >= from7)
                .ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var d = from7.AddDays(i).Date;
                vm.LeadDays.Add(d.ToString("dd/MM"));
                vm.LeadCounts.Add(last7.Count(x => x.CreatedAt.Date == d));
            }

            // Top products by leads (30d)
            var since30 = now.AddDays(-30);
            vm.TopProducts30d = await _db.Leads.AsNoTracking()
                .Where(l => l.ProductId != null && l.CreatedAt >= since30)
                .Include(l => l.Product)
                .GroupBy(l => l.Product!.Name)
                .Select(g => new AdminDashboardVM.TopProductRow { Name = g.Key, Leads = g.Count() })
                .OrderByDescending(x => x.Leads).Take(5).ToListAsync();

            // Recent leads
            vm.RecentLeads = await _db.Leads.AsNoTracking()
                .Include(l => l.Product)
                .OrderByDescending(l => l.CreatedAt).Take(8)
                .Select(l => new AdminDashboardVM.RecentLeadRow
                {
                    Id = l.Id,
                    CreatedAt = l.CreatedAt,
                    Name = l.Name,
                    Phone = l.Phone,
                    Email = l.Email,
                    ProductName = l.Product != null ? l.Product.Name : null,
                    Status = l.Status
                }).ToListAsync();

            return View(vm);
        }
    }
}
