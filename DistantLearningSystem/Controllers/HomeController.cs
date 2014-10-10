using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DistantLearningSystem.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(int? result)
        {
            if (result.HasValue)
                ViewBag.Result = ProcessResults.GetById(result.Value);
            return View();
        }
    }
}
