using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class BranchController : Controller
    {
        private ApplicationDbContext _context;

        public BranchController()
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
            var list = _context.Database.SqlQuery<Branch>("SELECT b.id, b.name, b.regionid, r.name AS region_name FROM Branch AS b INNER JOIN Region AS r ON b.regionid = r.id").ToList();

            return View(list);
        }
        public ActionResult Create(Branch Branch)
        {
            var Region_list = _context.Database.SqlQuery<Region>("select * from Region").ToList();

            ViewBag.Region = Region_list;
            return View(Branch);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Branch Branch)
        {

            _context.Database.ExecuteSqlCommand("insert into Branch(regionid,name) values('" + Branch.regionid + "','" + Branch.name + "')");


            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var Branch = _context.Database.SqlQuery<Branch>("select * from Branch where id ='" + ID + "'").SingleOrDefault();
            var Region_list = _context.Database.SqlQuery<Region>("select * from Region").ToList();

            ViewBag.Region = Region_list;
            return View(Branch);
        }
        [HttpPost]
        public ActionResult Edit(Branch Branch)
        {
            _context.Database.ExecuteSqlCommand("Update Branch set name  = '" + Branch.name + "',regionid  = '" + Branch.regionid + "' where id = " + Branch.id + "");
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Branch where id = " + ID + "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetBranchesByRegion(int regionId)
        {
         
            var branches=_context.Database.SqlQuery<Branch>("select id,name from Branch where regionid ='" + regionId + "'").ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
    }
}