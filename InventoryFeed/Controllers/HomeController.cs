using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryFeed.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
       // [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Feed", "Customer");
        }

    }
}
