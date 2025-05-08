using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.Controllers
{
    public class GatePassNewController : Controller
    {
        private ApplicationDbContext _context;

        public GatePassNewController()
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
            var list = _context.Database.SqlQuery<GatePass>("select * from GatePass order by id desc" ).ToList();
            return View(list);
        }
        public ActionResult Create(GatePass GatePass)
        {
            GatePass.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(id),0)+1 from GatePass  ").FirstOrDefault();
            var Customers = _context.Database.SqlQuery<Customers>("select  customerid,Name from  customers").ToList();
            ViewBag.Customers = Customers;
            return View(GatePass);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(GatePass GatePass)
        {
            _context.Database.ExecuteSqlCommand("INSERT INTO GatePass (date, dono, customerName, materialDetail, driverName, contactNumber, vehicleNo, shippingDetail, timeout, estimateDeliveryTime) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9)",
                  GatePass.Date,
                  GatePass.DoNo,
                  GatePass.CustomerName,
                  GatePass.MaterialDetail,
                  GatePass.DriverName,
                  GatePass.ContactNumber,
                  GatePass.VehicleNo,
                  GatePass.ShippingDetail,
                  GatePass.Timeout,
                  GatePass.EstimateDeliveryTime);
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var GatePass = _context.Database.SqlQuery<GatePass>("select * from GatePass where id =" + ID + "").SingleOrDefault();
            var Customers = _context.Database.SqlQuery<Customers>("select  customerid,Name from  customers").ToList();
            ViewBag.Customers = Customers;
            return View(GatePass);
        }
        [HttpPost]
        public ActionResult Edit(GatePass GatePass)
        {
            _context.Database.ExecuteSqlCommand(@" UPDATE GatePass SET 
                                                         date = @p0,
                                                         dono = @p1,
                                                         customerName = @p2,
                                                         materialDetail = @p3,
                                                         driverName = @p4,
                                                         contactNumber = @p5,
                                                         vehicleNo = @p6,
                                                         shippingDetail = @p7,
                                                         timeout = @p8,
                                                         estimateDeliveryTime = @p9
                                                      WHERE id = @p10", GatePass.Date, GatePass.DoNo, GatePass.CustomerName,  GatePass.MaterialDetail,  GatePass.DriverName,GatePass.ContactNumber, GatePass.VehicleNo,GatePass.ShippingDetail, GatePass.Timeout, GatePass.EstimateDeliveryTime, GatePass.Id);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From GatePass where id = " + ID + "");
            return RedirectToAction("Index");
        }
        
        public ActionResult Print(int? ID)
        {
            var GatePass = _context.Database.SqlQuery<GatePass>("select * from GatePass where id =" + ID + "").SingleOrDefault();

            return View(GatePass);
        }

        [HttpPost]
        public ActionResult MarkAsDuplicate(int id)
        {
            var gatePass = _context.Database.SqlQuery<GatePass>("select * from GatePass where id =" + id + "").SingleOrDefault();
            if (gatePass != null && !gatePass.Isdublicate)
            {
                _context.Database.ExecuteSqlCommand("UPDATE GatePass SET  Isdublicate = 1 WHERE id = " + id);

            }

           return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}