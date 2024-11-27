using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class QualityCategoryController : Controller
    {
        private ApplicationDbContext _context;

        public QualityCategoryController()
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
            var list = _context.Database.SqlQuery<QualityCategories>("SELECT * from QualityCategories").ToList();
            return View(list);
        }
        public ActionResult Create(QualityCategories qualityCategories)
        {
            qualityCategories.ID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ID),0)+1 from QualityCategories  ").FirstOrDefault();
            return View(qualityCategories);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(QualityCategories qualityCategories)
        {
              _context.Database.ExecuteSqlCommand("insert into QualityCategories (ID,CategoryName) values(" + qualityCategories.ID + ",N'" + qualityCategories.CategoryName + "')");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Database.SqlQuery<QualityCategories>("SELECT * from QualityCategories where ID=" + ID+"").SingleOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(QualityCategories qualityCategories)
        {
                _context.Database.ExecuteSqlCommand("Update QualityCategories set CategoryName  = N'" + qualityCategories.CategoryName + "' where ID = " + qualityCategories.ID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From QualityCategories where ID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}