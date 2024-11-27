using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class MainCatController : Controller
    {
        private ApplicationDbContext _context;

        public MainCatController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: MainCat
        public ActionResult Index()
        {
            var list = _context.Category.ToList();
            return View(list);
        }
    }
}