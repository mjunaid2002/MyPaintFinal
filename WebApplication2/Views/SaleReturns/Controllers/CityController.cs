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
    public class CityController : Controller
    {
        private ApplicationDbContext _context;

        public CityController()
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
            var list = _context.Database.SqlQuery<Cityquery>("select * from tbl_city").ToList();
            return View(list);
        }
        public ActionResult Create(Cityquery cityquery)
        {
            cityquery.id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from tbl_city  ").FirstOrDefault();
            return View(cityquery);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Cityquery cityquery)
        {
            _context.Database.ExecuteSqlCommand("insert into tbl_city (id,name) values(" + cityquery.id + ",N'" + cityquery.name + "')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var cargo = _context.Database.SqlQuery<Cityquery>("select * from tbl_city where id =" + ID + "").SingleOrDefault();
            return View(cargo);
        }
        [HttpPost]
        public ActionResult Edit(Cityquery cityquery)
        {
            _context.Database.ExecuteSqlCommand("Update tbl_city set name  = '" + cityquery.name + "' where id = " + cityquery.id + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From tbl_city where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}