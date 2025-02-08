using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    public class AdminGroupController : BaseController
    {
        private readonly ShopmasterdbContext _db;

        public AdminGroupController(ShopmasterdbContext db)
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
            var menuSubs = await _db.MenuSubs
            .Include(m => m.Group)
            .ToListAsync();

                var viewModel = new AdminGroupPermissionViewModel
                {
                    MenuPermissions = menuSubs.Select(menu => new MenuPermission
                    {
                        MenuId = menu.Id,
                        MenuName = menu.Name,
                        MenuGroupName = menu.Group.Name,
                        CanCreate = false,
                        CanRead = true,
                        CanUpdate = false,
                        CanDelete = false
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

            var menuSubs = await _db.MenuSubs
                .Include(m => m.Group)
                .ToListAsync();
            var rules = await _db.Rules.Where(r => r.GroupId == id).ToListAsync();

            var viewModel = new AdminGroupPermissionViewModel
            {
                GroupId = group.Id,
                GroupName = group.Name,
                MenuPermissions = menuSubs.Select(menu => new MenuPermission
                {
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    MenuGroupName = menu.Group.Name,
                    CanCreate = rules.Any(r => r.MenuId == menu.Id && r.CanCreate == true),
                    CanRead = rules.Any(r => r.MenuId == menu.Id && r.CanRead == true),
                    CanUpdate = rules.Any(r => r.MenuId == menu.Id && r.CanUpdate == true),
                    CanDelete = rules.Any(r => r.MenuId == menu.Id && r.CanDelete == true)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminGroupPermissionViewModel model)
        {
            var existingRules = await _db.Rules.Where(r => r.GroupId == model.GroupId).ToListAsync();
            _db.Rules.RemoveRange(existingRules);

            foreach (var menu in model.MenuPermissions)
            {
                var newRule = new Rule
                {
                    GroupId = model.GroupId,
                    MenuId = menu.MenuId,
                    CanCreate = menu.CanCreate ? true : false,
                    CanRead = menu.CanRead ? true : false,
                    CanUpdate = menu.CanUpdate ? true : false,
                    CanDelete = menu.CanDelete ? true : false,
                };
                _db.Rules.Add(newRule);
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
