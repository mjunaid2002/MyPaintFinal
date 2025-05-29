using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    public class StoreNewController : Controller
    {
        private ApplicationDbContext _context;

        public StoreNewController()
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
            var list = _context.Database.SqlQuery<StoreNew>("select * from Store").ToList();
            return View(list);
        }
        public ActionResult Create(StoreNew Store)
        {
            //  Store.id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from Store  ").FirstOrDefault();

            return View(Store);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(StoreNew Store)
        {

            _context.Database.ExecuteSqlCommand("insert into Store(name) values('" + Store.name + "')");


            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var Store = _context.Database.SqlQuery<StoreNew>("select * from Store where id ='" + ID + "'").SingleOrDefault();
            return View(Store);
        }
        [HttpPost]
        public ActionResult Edit(StoreNew Store)
        {
            _context.Database.ExecuteSqlCommand("Update Store set name  = '" + Store.name + "' where id = " + Store.id + "");
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Store where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}