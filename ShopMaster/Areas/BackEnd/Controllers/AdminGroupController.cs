using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class AdminGroupController : BaseController
    {
        private readonly shopmasterdbContext _db;

        public AdminGroupController(shopmasterdbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _db.AdminGroups.ToListAsync();
            return View(groups);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // 先取得 MenuGroup
            var menuGroups = await _db.MenuGroups.ToListAsync();

            // 取得 MenuSub 並手動關聯到 MenuGroup
            var menuSubs = await _db.MenuSubs.ToListAsync();

            var viewModel = new AdminGroupPermissionViewModel
            {
                MenuPermissions = menuSubs.Select(menu =>
                {
                    var menuGroup = menuGroups.FirstOrDefault(g => g.Id == menu.GroupId);
                    return new MenuPermission
                    {
                        MenuId = menu.Id,
                        MenuName = menu.Name,
                        MenuGroupName = menuGroup?.Name ?? "未知分類", // 🔹 手動關聯 Group
                        CanCreate = false,
                        CanRead = true,
                        CanUpdate = false,
                        CanDelete = false
                    };
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(AdminGroupPermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newGroup = new AdminGroup
                {
                    Name = model.GroupName,
                    CreatedAt = DateTime.UtcNow
                };

                _db.AdminGroups.Add(newGroup);
                await _db.SaveChangesAsync();

                foreach (var menu in model.MenuPermissions)
                {
                    var rule = new Rule
                    {
                        GroupId = newGroup.Id,
                        MenuId = menu.MenuId,
                        CanCreate = menu.CanCreate ? true : false,
                        CanRead = menu.CanRead ? true : false,
                        CanUpdate = menu.CanUpdate ? true : false,
                        CanDelete = menu.CanDelete ? true : false
                    };
                    _db.Rules.Add(rule);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _db.AdminGroups.FindAsync(id);
            if (group == null) return NotFound();

            // 先查詢所有 MenuGroup
            var menuGroups = await _db.MenuGroups.ToListAsync();

            // 查詢所有 MenuSub
            var menuSubs = await _db.MenuSubs.ToListAsync();

            // 查詢該群組的權限
            var rules = await _db.Rules.Where(r => r.GroupId == id).ToListAsync();

            var viewModel = new AdminGroupPermissionViewModel
            {
                GroupId = group.Id,
                GroupName = group.Name,
                MenuPermissions = menuSubs.Select(menu =>
                {
                    // 手動找到對應的 MenuGroup
                    var menuGroup = menuGroups.FirstOrDefault(g => g.Id == menu.GroupId);

                    return new MenuPermission
                    {
                        MenuId = menu.Id,
                        MenuName = menu.Name,
                        MenuGroupName = menuGroup != null ? menuGroup.Name : "未知分類", // 如果找不到就顯示 "未知分類"
                        CanCreate = rules.Any(r => r.MenuId == menu.Id && r.CanCreate == true),
                        CanRead = rules.Any(r => r.MenuId == menu.Id && r.CanRead == true),
                        CanUpdate = rules.Any(r => r.MenuId == menu.Id && r.CanUpdate == true),
                        CanDelete = rules.Any(r => r.MenuId == menu.Id && r.CanDelete == true)
                    };
                }).ToList()
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(AdminGroupPermissionViewModel model)
        {
            var existingRules = await _db.Rules
                .Where(r => r.GroupId == model.GroupId)
                .ToListAsync();

            var menuIdsFromForm = model.MenuPermissions.Select(m => m.MenuId).ToHashSet();

            foreach (var menu in model.MenuPermissions)
            {
                var rule = existingRules.FirstOrDefault(r => r.MenuId == menu.MenuId);

                if (rule != null)
                {
                    // 更新現有權限
                    rule.CanCreate = menu.CanCreate;
                    rule.CanRead = menu.CanRead;
                    rule.CanUpdate = menu.CanUpdate;
                    rule.CanDelete = menu.CanDelete;

                    _db.Entry(rule).State = EntityState.Modified;
                }
                else
                {
                    // 新增新的權限
                    var newRule = new Rule
                    {
                        GroupId = model.GroupId,
                        MenuId = menu.MenuId,
                        CanCreate = menu.CanCreate,
                        CanRead = menu.CanRead,
                        CanUpdate = menu.CanUpdate,
                        CanDelete = menu.CanDelete
                    };
                    _db.Rules.Add(newRule);
                }
            }

            // 🛑 避免 "會員列表" 這類選單被刪掉
            var missingRules = existingRules
                .Where(r => !menuIdsFromForm.Contains(r.MenuId))
                .ToList();

            foreach (var rule in missingRules)
            {
                rule.CanCreate = false;
                rule.CanRead = false;
                rule.CanUpdate = false;
                rule.CanDelete = false;
                _db.Entry(rule).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _db.AdminGroups.FindAsync(id);
            if (group == null) return NotFound();

            bool hasAdmins = await _db.Admins.AnyAsync(a => a.GroupId == id);
            if (hasAdmins)
            {
                TempData["Error"] = "❌ 此群組仍有管理員，無法刪除！";
                return RedirectToAction("Index");
            }

            _db.AdminGroups.Remove(group);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
