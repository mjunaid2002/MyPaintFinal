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
            var list = _context.Database.SqlQuery<GatePass>("select * from GatePass order by status").ToList();
            return View(list);
        }
        public ActionResult Create(GatePass GatePass)
        {
            GatePass.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(id),0)+1 from GatePass  ").FirstOrDefault();
            var salelist = _context.Database.SqlQuery<SaleReturnQuery>("select m.OrderID,m.custname,m.title,m.ntotal,m.date ,SUM(ISNULL(d.qty,0)) AS total from srsm m left join srsdetail d on m.OrderID = d.OrderID AND m.title=d.Status where IsGatepassPrinted=0 AND m.title IN ('WTSINV','TSINV') GROUP BY m.OrderID, m.custname, m.title, m.ntotal, m.date").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            //var Customers = _context.Database.SqlQuery<Customers>("select  customerid,Name from  customers").ToList();
            ViewBag.salelist = salelist;
            ViewBag.cargolist = Cargo_list;
            return View(GatePass);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(GatePass GatePass,List<SaleSelection> Sales)
        {
            var selected = Sales?.Where(x => x.IsSelected).ToList();

            if (selected == null || !selected.Any())
            {
                TempData["succ"] = "Please select at least one invoice.";
                return RedirectToAction("Create");
            }

            var orderIds = string.Join(",", selected.Select(s => s.OrderID));
            var customerNames = string.Join(",", selected.Select(s => s.CustomerName));
            var totalQty = selected.Sum(s => s.TotalQty);
       
            GatePass.Id = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(Id),0)+1 FROM GatePass").FirstOrDefault();
            var PreparedBy = Session["CurrentUserName"]?.ToString();

            _context.Database.ExecuteSqlCommand("INSERT INTO GatePass (date, dono, customerName, materialDetail, driverName, contactNumber, vehicleNo, shippingDetail, timeout, estimateDeliveryTime,PreparedBy,status) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)",
                  GatePass.Date, orderIds, customerNames, GatePass.MaterialDetail, GatePass.DriverName, GatePass.ContactNumber,
                  GatePass.VehicleNo, GatePass.ShippingDetail, GatePass.Timeout, GatePass.EstimateDeliveryTime, PreparedBy,"Pending");

            foreach (var orderId in selected)
            {
                _context.Database.ExecuteSqlCommand("UPDATE srsm SET IsGatepassPrinted = 1 WHERE title=@p1 AND OrderID = @p0", orderId.OrderID,orderId.Title);
            }

            TempData["msg"] = "Gate Pass created successfully.";
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
                                                      WHERE id = @p10", GatePass.Date, GatePass.DoNo, GatePass.CustomerName, GatePass.MaterialDetail, GatePass.DriverName, GatePass.ContactNumber, GatePass.VehicleNo, GatePass.ShippingDetail, GatePass.Timeout, GatePass.EstimateDeliveryTime, GatePass.Id);
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

        public ActionResult SatusUpdate(int? ID)
        {
            if (ID != 0 ) {
                _context.Database.ExecuteSqlCommand("Update  GatePass Set status='Recived' where id = " + ID + "");
            }
            return RedirectToAction("Index");
        }
    }
}