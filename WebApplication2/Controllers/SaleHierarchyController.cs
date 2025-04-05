using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    public class SaleHierarchyController : Controller
    {
        private ApplicationDbContext _context;

        public SaleHierarchyController()
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
            var list = _context.Database.SqlQuery<SaleHierarchy>("Select SH.id,SH.name,SH.parentid,SH1.name AS ParentName from SaleHierarchy SH Left join SaleHierarchy SH1 on SH1.Id = SH.ParentId").ToList();
            return View(list);
        }
        public ActionResult Create(SaleHierarchy SaleHierarchy)
        {
            SaleHierarchy.Id = _context.Database.SqlQuery<int>("Select ISNULL(Max(id),0)+1 from SaleHierarchy  ").FirstOrDefault();
            var SaleHierarchy_list = _context.Database.SqlQuery<SaleHierarchy>("select id,name,parentid from SaleHierarchy").ToList();

            ViewBag.SaleHierarchy = SaleHierarchy_list;
            return View(SaleHierarchy);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(SaleHierarchy SaleHierarchy)
        {
            _context.Database.ExecuteSqlCommand("insert into SaleHierarchy (name,ParentId) values(N'" + SaleHierarchy.Name + "','" + SaleHierarchy.ParentId + "')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var BeltArea = _context.Database.SqlQuery<SaleHierarchy>("select * from SaleHierarchy where id =" + ID + "").SingleOrDefault();
            var SaleHierarchy_list = _context.Database.SqlQuery<SaleHierarchy>("select id,name,parentid from SaleHierarchy").ToList();

            ViewBag.SaleHierarchy = SaleHierarchy_list;
            return View(BeltArea);
        }
        [HttpPost]
        public ActionResult Edit(SaleHierarchy SaleHierarchy)
        {
            _context.Database.ExecuteSqlCommand("Update SaleHierarchy set name  = '" + SaleHierarchy.Name + "',ParentId='"+ SaleHierarchy.ParentId + "' where id = " + SaleHierarchy.Id + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From SaleHierarchy where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}