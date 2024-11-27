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
    public class StoreController : Controller
    {
        private ApplicationDbContext _context;

        public StoreController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Store
        public ActionResult Index()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            return View(list);
        }
        public ActionResult Create(Store Store)
        {
            Store.ID= _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Stores").FirstOrDefault();
            return View(Store);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase file, Store Store)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;

            if (file != null)
            {
                ImageName = System.IO.Path.GetFileName(file.FileName);
                string img = num + ImageName;
                physicalPath = Server.MapPath("~/Uploads/" + img);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("insert into Stores values(" + Store.ID + ",N'" + Store.Name + "','" + img + "',N'" + Store.Address + "','" + Store.Phone + "','"+ Session["BusinessUnit"] + "')");
                return RedirectToAction("Create");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("insert into Stores values(" + Store.ID + ",N'" + Store.Name + "','NUll',N'" + Store.Address + "','" + Store.Phone + "','" + Session["BusinessUnit"] + "')");
                return RedirectToAction("Create");
            }

        }

        public ActionResult Edit(int? ID)
        {
            var data = _context.Store.SingleOrDefault(c => c.ID == ID);
            return View(data);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Save(int ID, Store Store, HttpPostedFileBase file)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            if (file != null)
            {
                ImageName = System.IO.Path.GetFileName(file.FileName);
                string img = num + ImageName;
                physicalPath = Server.MapPath("~/Uploads/" + img);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("Update Stores set Image='" + img + "',Name=N'" + Store.Name + "',Address='" + Store.Address + "',Phone='" + Store.Phone + "' where ID =" + ID+"");
                return RedirectToAction("Index");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("Update Stores set Name=N'" + Store.Name + "',Address=N'" + Store.Address + "',Phone='" + Store.Phone + "' where ID =" + ID + "");
                return RedirectToAction("Index");
            }

        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Stores where ID = " + ID + "");
            return RedirectToAction("Index");
        }

    }
}