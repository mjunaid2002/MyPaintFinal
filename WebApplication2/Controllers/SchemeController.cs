using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    public class SchemeController : Controller
    {
        private ApplicationDbContext _context;
        public SchemeController()
        {
            _context = new ApplicationDbContext();
        }
        //protected override void Dispose(bool disposing)
        //{
        //    _context.Dispose();
        //}
        // GET: Scheme
        public ActionResult Index()
        {
            var Scheme = _context.Database.SqlQuery<Scheme>("select * from Scheme").ToList();

            return View(Scheme);
        }
        
        public ActionResult Create(Scheme Scheme)
        {
          
            Scheme.date = DateTime.Today;
            Scheme.startdate = DateTime.Today;
            Scheme.enddate = DateTime.Today;
            var list = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();

            var viewmodel = new SchemeVM()
            {
                Scheme = Scheme,
                Categorylist= list

            };
            return View(viewmodel);
        }
        

        [HttpPost, ActionName("Create")]
        public ActionResult Save(SchemeVM SchemeVM,string[] Categoryname,int[] Categoryid)
        {
            
            _context.Database.ExecuteSqlCommand("insert into Scheme (Name, date, startdate, enddate, status) values(N'" + SchemeVM.Scheme.Name + "',N'" + SchemeVM.Scheme.date + "','" + SchemeVM.Scheme.startdate + "','" + SchemeVM.Scheme.enddate + "',N'" + SchemeVM.Scheme.status + "')");
            SchemeVM.Scheme.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(Id),0) from Scheme").FirstOrDefault();
            if (Categoryname != null && Categoryname.Length > 0) {

                for (int i = 0; i < Categoryname.Length; i++)
                {
                    _context.Database.ExecuteSqlCommand("insert into SchemeDetail (Scheme_Id, Cat_id, Cat_Name) values(N'" + SchemeVM.Scheme.Id + "',N'" + Categoryid[i] + "','" + Categoryname[i] + "')");

                }
               
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var Scheme = _context.Database.SqlQuery<Scheme>("select * from Scheme where id =" + ID + "").SingleOrDefault();
            var SchemeDetail = _context.Database.SqlQuery<SchemeDetail>("select * from SchemeDetail where Scheme_Id =" + ID + "").ToList();
            var list = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();

            var viewmodel = new SchemeVM
            {
                Scheme = Scheme,
                SchemeDetaillist = SchemeDetail,
                Categorylist = list
            };
            return View(viewmodel);
       
        }
        [HttpPost]
        public ActionResult Edit(SchemeVM SchemeVM,string[] Categoryname, int[] Categoryid)
        {
            
            if (Categoryname != null && Categoryname.Length > 0)
            {
                _context.Database.ExecuteSqlCommand("Delete From SchemeDetail where Scheme_Id = " + SchemeVM.Scheme.Id + "");
                for (int i = 0; i < Categoryname.Length; i++)
                {
                    _context.Database.ExecuteSqlCommand("insert into SchemeDetail (Scheme_Id, Cat_id, Cat_Name) values(N'" + SchemeVM.Scheme.Id + "',N'" + Categoryid[i] + "','" + Categoryname[i] + "')");

                }

            }
            _context.Database.ExecuteSqlCommand("Update Scheme set name  = '" + SchemeVM.Scheme.Name + "',date  = '" + SchemeVM.Scheme.date + "',startdate  = '" + SchemeVM.Scheme.startdate + "',enddate  = '" + SchemeVM.Scheme.enddate + "',status  = '" + SchemeVM.Scheme.status + "' where id = " + SchemeVM.Scheme.Id + "");

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Scheme where id = " + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From SchemeDetail where Scheme_Id = " + ID + "");
            return RedirectToAction("Index");
        }

    }
}