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
    public class CargoController : Controller
    {
        private ApplicationDbContext _context;

        public CargoController()
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
            var list = _context.Database.SqlQuery<cargo>("select * from cargo").ToList();
            return View(list);
        }
        public ActionResult Create(cargo cargo)
        {
            cargo.id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from cargo  ").FirstOrDefault();
            return View(cargo);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(cargo cargo)
        {
            _context.Database.ExecuteSqlCommand("insert into cargo (id,name,address,cityid,tel,fax,email) values(" + cargo.id + ",N'" + cargo.name + "',N'" + cargo.address + "',N'" + cargo.cityid + "',N'" + cargo.tel + "',N'" + cargo.fax + "',N'" + cargo.email + "')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var cargo = _context.Database.SqlQuery<cargo>("select * from cargo where id =" + ID + "").SingleOrDefault();
            return View(cargo);
        }
        [HttpPost]
        public ActionResult Edit(cargo cargo)
        {
            _context.Database.ExecuteSqlCommand("Update cargo set name  = '" + cargo.name + "',address  = '" + cargo.address + "',cityid  = '" + cargo.cityid + "',tel  = '" + cargo.tel + "',fax  = '" + cargo.fax + "',email  = '" + cargo.email + "' where id = " + cargo.id + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From cargo where id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}