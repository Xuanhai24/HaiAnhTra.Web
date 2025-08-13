using HaiAnhTra.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaiAnhTra.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signIn;
        public AccountController(SignInManager<ApplicationUser> signIn) => _signIn = signIn;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) => View(model: returnUrl);

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var res = await _signIn.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);
            if (res.Succeeded) return Redirect(returnUrl ?? Url.Action("Index", "Dashboard", new { area = "Admin" })!);
            ModelState.AddModelError("", "Đăng nhập thất bại");
            return View(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
