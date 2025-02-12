using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

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

        protected List<MenuGroupViewModel> GetMenu()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(adminId)) return new List<MenuGroupViewModel>();

            var admin = _db.Admins.FirstOrDefault(a => a.Id == Convert.ToInt64(adminId));
            if (admin == null) return new List<MenuGroupViewModel>();

            // 取得 AdminGroup 的所有 Rules
            var rules = _db.Rules.Where(r => r.GroupId == admin.GroupId).ToList();

            // 取得所有 MenuGroups
            var menuGroups = _db.MenuGroups.OrderBy(g => g.SortOrder).ToList();

            // 取得所有 MenuSubs
            var menuSubs = _db.MenuSubs.OrderBy(m => m.SortOrder).ToList();

            // 手動關聯 MenuGroup 和 MenuSubs
            var result = menuGroups
                .Select(group => new MenuGroupViewModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    MenuSubs = menuSubs
                        .Where(menu => menu.GroupId == group.Id && rules.Any(rule => rule.MenuId == menu.Id && rule.CanRead == true))
                        .ToList()
                })
                .Where(g => g.MenuSubs.Any()) // 移除沒有 `MenuSub` 的 `MenuGroup`
                .ToList();

            return result;
        }



    }
}
