using CRM.Models;
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
    public class BrandsController : Controller
    {
        private ApplicationDbContext _context;

        public BrandsController()
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
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            return View(list);
        }
        public ActionResult Create(Brands Brands)
        {
            Brands.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Brands ").FirstOrDefault();
            return View(Brands);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase file, Brands Brands)
        {
                _context.Database.ExecuteSqlCommand("insert into Brands (Id,Name,StartDate,EndDate,b_unit) values(" + Brands.Id + ",N'" + Brands.Name + "','" + Brands.StartDate + "','" + Brands.EndDate + "','" + Session["BusinessUnit"] + "')");
            TempData["succ"] = "Record Saved Successfully";
            return RedirectToAction("Create");
            

        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Brands.SingleOrDefault(c => c.Id == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, Brands Brands, HttpPostedFileBase file)
        {
            
                _context.Database.ExecuteSqlCommand("Update Brands set Id =" + Brands.Id + ",Name  = N'" + Brands.Name + "',StartDate = '" + Brands.StartDate + "',EndDate = '" + Brands.EndDate + "' where Id = " + ID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Brands where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}