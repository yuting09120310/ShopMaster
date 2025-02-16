using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class LoginController : Controller
    {
        private readonly shopmasterdbContext _db;

        public LoginController(shopmasterdbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "❌ 帳號與密碼不得為空！";
                return View();
            }

            // 加密密碼（假設密碼是 Hash 儲存）
            string hashedPassword = HashPassword(Password);

            // 查詢 MySQL 是否有這個帳號
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Username == Username && a.PasswordHash == hashedPassword);

            if (admin == null)
            {
                ViewBag.Error = "❌ 帳號或密碼錯誤，請再試一次！";
                return View();
            }

            // 設定登入 Session
            HttpContext.Session.SetString("AdminId", admin.Id.ToString());
             return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToAction("Index");
        }

        // 密碼加密 (SHA256)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
