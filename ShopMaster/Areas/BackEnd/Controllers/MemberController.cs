using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class MemberController : BaseController
    {
        private readonly shopmasterdbContext _db;
        private readonly IWebHostEnvironment _env;

        public MemberController(shopmasterdbContext db, IWebHostEnvironment env): base(db)
        {
            _db = db;
            _env = env;
        }

        // 會員列表
        public async Task<IActionResult> Index()
        {
            var members = await _db.Members.Include(m => m.MemberType).OrderBy(m => m.CreatedAt).ToListAsync();
            return View(members);
        }

        // 新增 - GET
        public async Task<IActionResult> Create()
        {
            ViewBag.MemberTypes = await _db.MemberTypes.ToListAsync();
            return View();
        }

        // 新增 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member, IFormFile? avatarFile)
        {
            ModelState.Remove(nameof(member.MemberType));

            if (ModelState.IsValid)
            {
                member.CreatedAt = DateTime.UtcNow;

                // 儲存會員
                _db.Members.Add(member);
                await _db.SaveChangesAsync(); // 取得會員 ID

                //  儲存頭像
                if (avatarFile != null)
                {
                    string memberFolder = Path.Combine(_env.WebRootPath, "upload", "member", member.Id.ToString());
                    if (!Directory.Exists(memberFolder))
                    {
                        Directory.CreateDirectory(memberFolder);
                    }

                    string fileName = $"avatar{Path.GetExtension(avatarFile.FileName)}";
                    string filePath = Path.Combine(memberFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    member.Avatar = $"/upload/member/{member.Id}/{fileName}";
                    _db.Members.Update(member);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            ViewBag.MemberTypes = await _db.MemberTypes.ToListAsync();
            return View(member);
        }

        // 編輯 - GET
        public async Task<IActionResult> Edit(long id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();

            ViewBag.MemberTypes = await _db.MemberTypes.ToListAsync();
            return View(member);
        }

        // 編輯 - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Member member, IFormFile? avatarFile)
        {
            ModelState.Remove(nameof(member.MemberType));

            if (ModelState.IsValid)
            {
                var existingMember = await _db.Members.FindAsync(member.Id);
                if (existingMember == null) return NotFound();

                existingMember.Name = member.Name;
                existingMember.Email = member.Email;
                existingMember.Phone = member.Phone;
                existingMember.Address = member.Address;
                existingMember.Birthday = member.Birthday;
                existingMember.MemberTypeId = member.MemberTypeId;
                existingMember.Active = member.Active;

                // 更新頭像
                if (avatarFile != null)
                {
                    string memberFolder = Path.Combine(_env.WebRootPath, "upload", "member", existingMember.Id.ToString());
                    if (!Directory.Exists(memberFolder))
                    {
                        Directory.CreateDirectory(memberFolder);
                    }

                    string fileName = $"avatar{Path.GetExtension(avatarFile.FileName)}";
                    string filePath = Path.Combine(memberFolder, fileName);

                    // 刪除舊頭像
                    if (!string.IsNullOrEmpty(existingMember.Avatar))
                    {
                        string oldImgPath = Path.Combine(_env.WebRootPath, existingMember.Avatar.TrimStart('/'));
                        if (System.IO.File.Exists(oldImgPath)) System.IO.File.Delete(oldImgPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    existingMember.Avatar = $"/upload/member/{existingMember.Id}/{fileName}";
                }

                _db.Members.Update(existingMember);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MemberTypes = await _db.MemberTypes.ToListAsync();
            return View(member);
        }

        // 刪除
        public async Task<IActionResult> Delete(long id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();

            // 刪除會員資料夾
            string memberFolder = Path.Combine(_env.WebRootPath, "upload", "member", member.Id.ToString());
            if (Directory.Exists(memberFolder))
            {
                Directory.Delete(memberFolder, true);
            }

            _db.Members.Remove(member);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
