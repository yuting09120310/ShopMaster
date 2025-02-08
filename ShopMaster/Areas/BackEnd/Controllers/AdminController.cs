using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using System.Security.Cryptography;
using System.Text;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ShopmasterdbContext _db;

        public AdminController(ShopmasterdbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _db.Admins
                .Include(a => a.Group)
                .ToListAsync();

            return View(admins);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = _db.AdminGroups.Select(group => new SelectListItem { Value = group.Id.ToString(), Text = group.Name }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = _db.AdminGroups.Select(group => new SelectListItem { Value = group.Id.ToString(), Text = group.Name }).ToList();
                return View(model);
            }

            var admin = new Admin
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                GroupId = model.GroupId
            };

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var admin = await _db.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            var model = new AdminViewModel
            {
                Id = admin.Id,
                Username = admin.Username,
                Email = admin.Email,
                GroupId = admin.GroupId
            };

            // 將 AdminGroup 資料轉換為 SelectList
            ViewBag.Groups = _db.AdminGroups.Select(group => new SelectListItem { Value = group.Id.ToString(), Text = group.Name }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = _db.AdminGroups.Select(group => new SelectListItem { Value = group.Id.ToString(), Text = group.Name }).ToList();
                return View(model);
            }

            var admin = await _db.Admins.FindAsync(model.Id);
            if (admin == null) return NotFound();

            admin.Username = model.Username;
            admin.Email = model.Email;
            admin.GroupId = model.GroupId;

            // 如果密碼欄位有填寫才更新
            if (!string.IsNullOrEmpty(model.Password))
            {
                admin.PasswordHash = HashPassword(model.Password);
            }

            _db.Admins.Update(admin);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var admin = await _db.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            _db.Admins.Remove(admin);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
