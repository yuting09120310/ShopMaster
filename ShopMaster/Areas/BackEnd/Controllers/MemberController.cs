using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;


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
        public async Task<IActionResult> Index(string? Name, string? Id, string? Phone, string? MemberTypeId, string? Active, int? page)
        {
            ViewBag.MemberTypes = _db.MemberTypes.ToList();
            var members = _db.Members.Include(m => m.MemberType).OrderBy(m => m.CreatedAt).AsQueryable();

            if (!string.IsNullOrEmpty(Name))
            {
                members = members.Where(e => e.Name.Contains(Name));
            }

            if (!string.IsNullOrEmpty(Id) && long.TryParse(Id, out long memberId))
            {
                members = members.Where(e => e.Id == memberId);
            }

            if (!string.IsNullOrEmpty(Phone))
            {
                members = members.Where(e => e.Phone == Phone);
            }

            if (!string.IsNullOrEmpty(MemberTypeId) && int.TryParse(MemberTypeId, out int memberTypeId))
            {
                members = members.Where(e => e.MemberTypeId == memberTypeId);
            }

            if (!string.IsNullOrEmpty(Active) && int.TryParse(Active, out int activeStatus))
            {
                members = members.Where(e => e.Active == activeStatus);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedMembers = members.AsEnumerable().ToPagedList(pageNumber, pageSize);

            return View(pagedMembers);
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
            ModelState.Remove(nameof(member.PasswordHash));

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
