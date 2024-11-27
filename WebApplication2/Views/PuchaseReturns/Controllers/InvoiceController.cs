using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(int[] id,string[] item_name, int[] sp, int[] qty, int[] n_total)
        {
            return View();
        }
    }
}