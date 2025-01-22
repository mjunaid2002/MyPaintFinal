using CRM.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;
using WebApplication1.Models;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class RegionController : Controller
    {
        private ApplicationDbContext _context;

        public RegionController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Category
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<Region>("select * from Region").ToList();
            return View(list);
        }
        public ActionResult Create(Region Region)
        {
          //  Region.id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from Region  ").FirstOrDefault();

            return View(Region);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Region Region)
        {

            _context.Database.ExecuteSqlCommand("insert into Region(id,name) values('" + Region.id + "','" + Region.name + "')");


            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var Region = _context.Database.SqlQuery<Region>("select * from Region where id ='" + ID + "'").SingleOrDefault();
            return View(Region);
        }
        [HttpPost]
        public ActionResult Edit(Region Region)
        {
            _context.Database.ExecuteSqlCommand("Update Region set name  = '" + Region.name + "' where id = " + Region.id + "");
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Region where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}