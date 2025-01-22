using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    public class BeltAreaController : Controller
    {
        private ApplicationDbContext _context;

        public BeltAreaController()
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
            var list = _context.Database.SqlQuery<BeltArea>("select * from BeltArea").ToList();
            return View(list);
        }
        public ActionResult Create(BeltArea BeltArea)
        {
            BeltArea.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(id),0)+1 from BeltArea  ").FirstOrDefault();
            return View(BeltArea);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(BeltArea BeltArea)
        {
            _context.Database.ExecuteSqlCommand("insert into BeltArea (name) values(N'" + BeltArea.Name + "')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var BeltArea = _context.Database.SqlQuery<BeltArea>("select * from BeltArea where id =" + ID + "").SingleOrDefault();
            return View(BeltArea);
        }
        [HttpPost]
        public ActionResult Edit(BeltArea BeltArea)
        {
            _context.Database.ExecuteSqlCommand("Update BeltArea set name  = '" + BeltArea.Name + "' where id = " + BeltArea.Id + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From BeltArea where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}