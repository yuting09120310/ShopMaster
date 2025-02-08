using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopMaster.Areas.BackEnd.Controllers
{
    [Area("BackEnd")]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 檢查 Session 是否有登入管理員
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser")))
            {
                // 若未登入，導向後台登入頁
                context.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
