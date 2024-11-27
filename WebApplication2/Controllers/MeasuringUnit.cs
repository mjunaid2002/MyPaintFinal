using Antlr.Runtime.Tree;
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
    public class MeasuringUnitController : Controller
    {
        private ApplicationDbContext _context;
        public MeasuringUnitController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: BusinessUnit
        public ActionResult Index()
        {
            string b_unit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.tbl_MeasuringUnit.Where(c=>c.b_unit == b_unit).ToList();
            return View(list);
        }
        public ActionResult Create(BusinessUnit bus)
        {
            return View();
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(MeasuringUnit cat)
        {
            _context.Database.ExecuteSqlCommand("insert into MeasuringUnits (Name,b_unit) values(N'" + cat.Name + "','" + Session["BusinessUnit"] + "')");
            TempData["succ"] = "Record Saved Successfully";
            return RedirectToAction("Create");
        }
        public ActionResult Edit(MeasuringUnit cat, int? ID)
        {
            var data = _context.tbl_MeasuringUnit.SingleOrDefault(c => c.Id == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(MeasuringUnit cat)
        {
                _context.Database.ExecuteSqlCommand("Update MeasuringUnits set Name  = N'" + cat.Name + "' where Id = " + cat.Id + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From MeasuringUnits where Id = " + ID + "");
            return RedirectToAction("Index");
        }

    }
}