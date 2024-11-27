using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class ProvincesController : Controller
    {
        private ApplicationDbContext _context;

        public ProvincesController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Provinces
        public ActionResult Index()
        {
            var list = _context.Provinces.ToList();
            return View(list);
        }
        public ActionResult Create(Provinces provinces)
        {
            provinces.ID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ID),0)+1 from Provinces").FirstOrDefault();
            return View(provinces);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(Provinces provinces)
        {
            _context.Database.ExecuteSqlCommand("insert into Provinces (Id,Name) values("+provinces.ID+ ",N'" + provinces.Name + "')");
            // ViewBag.Message = "Data Updated Successfully.";
            return RedirectToAction("Create");
        }

        public ActionResult Edit(decimal? ID)
        {
            var data = _context.Provinces.SingleOrDefault(c => c.ID == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(decimal ID, Provinces provinces)
        {
            _context.Database.ExecuteSqlCommand("UPDATE Provinces set Name=N'" + provinces.Name + "' where ID = "+ID+"");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                //return NotFound();
            }
            _context.Database.ExecuteSqlCommand("Delete From Provinces where ID =" + ID + "");
            return RedirectToAction("Index");
        }

    }
}