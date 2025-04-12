using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopMaster.Areas.BackEnd.Models;
using static ShopMaster.Areas.FrontEnd.EnumUtility.PaymentEnum;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class OrderController : Controller
    {
        private readonly shopmasterdbContext _db;

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
            var payInfo = _db.PayInfos.ToList();

            // 建立 Enum 對應的下拉選單資料
            var paymentMethodList = Enum.GetValues(typeof(Pay))
                           .Cast<Pay>()
                           .Select(x => new SelectListItem
                           {
                               Value = ((int)x).ToString(),
                               Text = x.ToString()

                           }).ToList();

            // 預設信用卡
            Pay selectedPaymentMethod = Pay.信用卡;

            if (payInfo.Any())
            {
                selectedPaymentMethod = (Pay)payInfo.First().Id;
            }

            var model = new FrontEnd.ViewModelsF.PayInfo
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                PaymentMethodList = paymentMethodList
            };

            return View(model);
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
