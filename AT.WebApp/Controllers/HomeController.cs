using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AT.WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
           
            return View();
        }

       // [HttpPost]
        public ActionResult CheckAccessibility(string url)
        {
           return View("AccessibilityForm");
        }


        public ActionResult MakeAdaptation(string url)
        {

           // return View("AdaptationForm");
            return View("ProductView");
        }
    }
}