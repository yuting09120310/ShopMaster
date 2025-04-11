using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    public class PavementController : Controller
    {
        // GET: PavementController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PavementController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PavementController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PavementController/Create
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

        // GET: PavementController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PavementController/Edit/5
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

        // GET: PavementController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PavementController/Delete/5
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
