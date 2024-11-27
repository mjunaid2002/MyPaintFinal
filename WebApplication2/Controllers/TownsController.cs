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
    public class TownsController : Controller
    {
        private ApplicationDbContext _context;
        public TownsController()
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
            var list = _context.Town.ToList();
            return View(list);
        }
        public ActionResult Create(Town Town)
        {
            Town.ID = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Towns").FirstOrDefault();
            return View(Town);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(City city)
        {
            _context.Database.ExecuteSqlCommand("insert into Towns (Name) values(N'" + city.Name + "')");
            // ViewBag.Message = "Data Updated Successfully.";
            return RedirectToAction("Create");
        }
        public ActionResult Edit(decimal? ID)
        {
            var data = _context.Town.SingleOrDefault(c => c.ID == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(decimal ID, Town Town)
        {
            _context.Database.ExecuteSqlCommand("UPDATE Towns set Name=N'" + Town.Name + "' where ID = " + ID + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                //return NotFound();
            }
            _context.Database.ExecuteSqlCommand("Delete From Towns where ID =" + ID + "");
            return RedirectToAction("Index");
        }

    }
}