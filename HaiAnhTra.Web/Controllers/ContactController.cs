using HaiAnhTra.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HaiAnhTra.Web.Controllers
{
    public class ContactController : Controller
    {
        // GET: /Contact
        [HttpGet]
        public IActionResult Index()
        {
            // Hiển thị form liên hệ chung (dùng partial _LeadForm)
            return View(new Lead());
        }

        // GET: /Contact/Thanks
        public IActionResult Thanks()
        {
            // Trang cảm ơn sau khi tạo Lead thành công
            return View();
        }
    }
}