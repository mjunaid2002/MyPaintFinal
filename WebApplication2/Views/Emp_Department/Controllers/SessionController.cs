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
    public class SessionController : Controller
    {
        private ApplicationDbContext _context;

        public SessionController()
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
            var list = _context.Database.SqlQuery<Session>("SELECT * FROM Sessions where b_unit ='"+ Bunit + "'").ToList();
            return View(list);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Session session)
        {
            _context.Database.ExecuteSqlCommand("Delete From Sessions where StartDate ='" + session.StartDate + "' and EndDate ='" + session.EndDate + "'");
            _context.Database.ExecuteSqlCommand("insert into Sessions (Name,StartDate,EndDate,B_unit) values(N'" + session.Name + "','" + session.StartDate + "','" + session.EndDate + "',"+Session["BusinessUnit"] +")");
                 return RedirectToAction("Create");

        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Database.SqlQuery<Session>("SELECT * FROM Sessions where id="+ID+" ").FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, Session session)
        {
            
                _context.Database.ExecuteSqlCommand("Update Sessions set Name  = N'" + session.Name + "',StartDate='"+ session .StartDate+ "',Enddate='"+ session.EndDate+ "' where Id = " + ID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Sessions where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}