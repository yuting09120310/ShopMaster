using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class BaseController : Controller
    {
        protected readonly ShopmasterdbContext _db;

        public BaseController(ShopmasterdbContext db)
        {
            _db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // 確保 Admin 已登入
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                context.Result = RedirectToAction("Index", "Login");
                return;
            }

            // 取得選單資料
            ViewBag.MenuList = GetMenu();
        }

        protected List<MenuGroup> GetMenu()
        {
            var adminId = HttpContext.Session.GetString("AdminId");

            var admin = _db.Admins
                .Include(a => a.Group)
                .ThenInclude(g => g.Rules)
                .FirstOrDefault(a => a.Id == Convert.ToInt64(adminId));

            if (admin == null) return new List<MenuGroup>();

            var menuGroups = _db.MenuGroups
                .Include(g => g.MenuSubs)
                .OrderBy(g => g.SortOrder)
                .ToList();

            foreach (var group in menuGroups)
            {
                group.MenuSubs = group.MenuSubs
                    .Where(menu => admin.Group.Rules.Any(rule => rule.MenuId == menu.Id && (rule.CanRead ?? false)))
                    .OrderBy(menu => menu.SortOrder)
                    .ToList();
            }

            // 移除沒有 `MenuSub` 的 `MenuGroup`
            menuGroups = menuGroups.Where(g => g.MenuSubs.Any()).ToList();

            return menuGroups;
        }
    }
}
