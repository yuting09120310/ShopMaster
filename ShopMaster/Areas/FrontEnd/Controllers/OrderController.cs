using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.FrontEnd.ViewModelsF;
using ShopMaster.Areas.FrontEnd.Utility;
using static ShopMaster.Areas.FrontEnd.EnumUtility.PaymentEnum;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class OrderController : Controller
    {
        private readonly shopmasterdbContext _db;
        List<Areas.FrontEnd.ViewModelsF.Cart> tempCart = new List<Areas.FrontEnd.ViewModelsF.Cart>();

        public OrderController(shopmasterdbContext db)
        {
            _db = db;
        }

        // GET: OrderController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            // 取付款方式下拉選單資料
            var payInfo = _db.PayInfos.ToList();
            
            var paymentMethodList = Enum.GetValues(typeof(Pay))
                           .Cast<Pay>()
                           .Select(x => new SelectListItem
                           {
                               Value = ((int)x).ToString(),
                               Text = x.ToString()

                           }).ToList();

            // 預設信用卡
            //Pay selectedPaymentMethod = Pay.信用卡;

            //if (payInfo.Any())
            //{
            //    selectedPaymentMethod = (Pay)payInfo.First().Id;
            //}
            //

            paymentMethodList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "請選擇付款方式"
            });

            var model = new FrontEnd.ViewModelsF.PayInfo
            {
                PaymentMethodList = paymentMethodList
            };

            // 取購物車
            tempCart = HttpContext.Session.Get<List<Areas.FrontEnd.ViewModelsF.Cart>>("tempCart") ?? new List<Areas.FrontEnd.ViewModelsF.Cart>();
            Dictionary<long, long> totalInput = HttpContext.Session.Get<Dictionary<long, long>>("totalInput") ?? new Dictionary<long, long>();
            Dictionary<long, decimal> price = HttpContext.Session.Get<Dictionary<long, decimal>>("priceDy") ?? new Dictionary<long, decimal>();

            var getCartTemp = tempCart.DistinctBy(p => p.ProductId).ToList();

            var orderViewModel = new OrderViewModel
            {
                cartList = getCartTemp,
                total = totalInput,
                price = price,
                payInfo = model
            };

            return View(orderViewModel);
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {








                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
