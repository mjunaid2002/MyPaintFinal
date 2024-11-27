using CRM.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class MainCategoryController : Controller
    {
        private ApplicationDbContext _context;

        public MainCategoryController()
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
            var list = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            return View(list);
        }
        public ActionResult Create(Categories categories)
        {
            categories.CategoryID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(CategoryID),0)+1 from Categories  ").FirstOrDefault();
            var listss = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                categories = categories,
                listss = listss,
            };
            return View(ProductVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Categories categories)
        {
              _context.Database.ExecuteSqlCommand("insert into Categories (RawProductCheck,CategoryID,CategoryName,Description,MainCategoryID) values('"+Request["RawProductCheck"] +"'," + categories.CategoryID + ",N'" + categories.CategoryName + "',N'" + categories.Description + "'," + categories.MainCategoryID + ")");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var categories = _context.Database.SqlQuery<Categories>("SELECT * from Categories where CategoryID=" + ID+"").SingleOrDefault();
            var listss = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                categories = categories,
                listss = listss,
            };
            return View(ProductVM);
        }
        [HttpPost]
        public ActionResult Edit(Categories categories)
        {
                _context.Database.ExecuteSqlCommand("Update Categories set CategoryName  = N'" + categories.CategoryName + "', Description  = N'" + categories.Description + "',MainCategoryID="+ categories.MainCategoryID + " where CategoryID = " + categories.CategoryID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Categories where CategoryID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}