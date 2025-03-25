using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShopMaster.Areas.BackEnd.Models;
using ShopMaster.Areas.BackEnd.ViewModels;

namespace ShopMaster.Areas.FrontEnd.Controllers
{
    [Area("FrontEnd")]
    public class BaseController : Controller
    {
        protected readonly shopmasterdbContext _db;

        public BaseController(shopmasterdbContext db)
        {
            _db = db;
        }

        
    }
}
