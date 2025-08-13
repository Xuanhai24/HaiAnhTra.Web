using System.Text;
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
    public class LeadsController : Controller
    {
        private readonly AppDbContext _db;
        public LeadsController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? q, int? status, DateTime? from, DateTime? to, int page = 1, int pageSize = 20)
        {
            var data = _db.Leads.AsNoTracking().Include(l => l.Product).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                data = data.Where(x => x.Name.Contains(q) || x.Phone.Contains(q) || (x.Email != null && x.Email.Contains(q)));
            if (status.HasValue) data = data.Where(x => (int)x.Status == status);
            if (from.HasValue) data = data.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) data = data.Where(x => x.CreatedAt <= to.Value);
            data = data.OrderByDescending(x => x.CreatedAt);

            var list = await PaginatedList<Lead>.CreateAsync(data, page, pageSize);
            ViewBag.Q = q; ViewBag.Status = status; ViewBag.From = from; ViewBag.To = to;
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(int[] selectedIds, LeadStatus status)
        {
            if (selectedIds.Length == 0) return RedirectToAction(nameof(Index));
            var leads = await _db.Leads.Where(x => selectedIds.Contains(x.Id)).ToListAsync();
            foreach (var l in leads) l.Status = status;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? q, int? status, DateTime? from, DateTime? to)
        {
            var data = _db.Leads.AsNoTracking().Include(l => l.Product).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                data = data.Where(x => x.Name.Contains(q) || x.Phone.Contains(q) || (x.Email != null && x.Email.Contains(q)));
            if (status.HasValue) data = data.Where(x => (int)x.Status == status);
            if (from.HasValue) data = data.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) data = data.Where(x => x.CreatedAt <= to.Value);

            var list = await data.OrderByDescending(x => x.CreatedAt).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Id,CreatedAt,Name,Phone,Email,Product,Status,Message");
            foreach (var l in list)
            {
                string Esc(string s) => "\"" + s.Replace("\"", "\"\"") + "\"";
                sb.AppendLine(string.Join(',', new[]{
                    l.Id.ToString(),
                    l.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    Esc(l.Name),
                    Esc(l.Phone),
                    Esc(l.Email??""),
                    Esc(l.Product?.Name??""),
                    l.Status.ToString(),
                    Esc(l.Message??"")
                }));
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"leads_{DateTime.UtcNow:yyyyMMddHHmm}.csv");
        }
    }
}
