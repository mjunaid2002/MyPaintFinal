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
    public class Emp_DepartmentController : Controller
    {
        private ApplicationDbContext _context;

        public Emp_DepartmentController()
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
            var list = _context.Database.SqlQuery<Emp_Department>("select * from Emp_Department").ToList();
            return View(list);
        }
        public ActionResult Create(Emp_Department Emp_Department)
        {
            Emp_Department.ID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from Emp_Department ").FirstOrDefault();
            return View(Emp_Department);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Emp_Department Emp_Department)
        {
            _context.Database.ExecuteSqlCommand("insert into Emp_Department (id,name) values(" + Emp_Department.ID + ",N'" + Emp_Department.Name + "' ");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var Emp_Department = _context.Database.SqlQuery<Emp_Department>("select * from Emp_Department where id =" + ID + "").SingleOrDefault();
            return View(Emp_Department);
        }
        [HttpPost]
        public ActionResult Edit(Emp_Department Emp_Department)
        {
            _context.Database.ExecuteSqlCommand("Update Emp_Department set name  = '" + Emp_Department.Name + "' where id = " + Emp_Department.ID + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Emp_Department where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}