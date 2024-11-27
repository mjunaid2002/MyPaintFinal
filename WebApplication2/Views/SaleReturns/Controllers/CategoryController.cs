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
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context;

        public CategoryController()
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
            var list = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            return View(list);
        }
        public ActionResult Create(MianCategories mianCategories)
        {
            mianCategories.MainCategoryID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(MainCategoryID),0)+1 from MianCategories  ").FirstOrDefault();
            return View(mianCategories);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(MianCategories mianCategories)
        {
              _context.Database.ExecuteSqlCommand("insert into MianCategories (MainCategoryID,MainCategoryName) values(" + mianCategories.MainCategoryID + ",N'" + mianCategories.MainCategoryName + "')");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories where MainCategoryID="+ID+"").SingleOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(MianCategories mianCategories)
        {
                _context.Database.ExecuteSqlCommand("Update MianCategories set MainCategoryName  = N'" + mianCategories.MainCategoryName + "' where MainCategoryID = " + mianCategories.MainCategoryID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From MianCategories where MainCategoryID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}