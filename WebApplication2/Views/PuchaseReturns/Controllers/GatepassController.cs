using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text.RegularExpressions;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class GatepassController : Controller
    {
        private ApplicationDbContext _context;
        public GatepassController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Gatepass
        public ActionResult Index()
        {
            var gate_list = _context.Database.SqlQuery<GatepassQuery>("SELECT *,(Select name From Suppliers where AccountNo=GatepassMaster.supid) as SupplierName,(Select ISNULL(SUM(NetTotal),0) From PurDetails where g_passid=GatepassMaster.invid) as PurchaseAmount  FROM [GatepassMaster] where  b_unit = '" + Session["BusinessUnit"] + "' ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                gate_list = gate_list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Create(GatepassMaster gatepassMaster , Gatepassdetail gatepassdetail)
        {
            
            gatepassMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from [GatepassMaster]").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit="+ Bunit + "").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                Store = Store,
                emp_list = emp_list,
                sup_list = sup_list,
                gatepassMaster = gatepassMaster,
                pro_list = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save( string[] item_name, int[] id, string[] qty, GatepassMaster gatepassMaster, Gatepassdetail gatepassdetail)
        {
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            for (int i = 0; i < id.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO GatepassDetail (item_iden,InvId,ItemID,ItemName,Qty,date) VALUES ('0'," + gatepassMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + qty[i] + ",'" + gatepassMaster.Date + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO GatepassMaster (Store,DsrId,CargoName,b_unit,InvID,supid,Address,PhoneNo,Date) VALUES (" + gatepassMaster.Store + "," + gatepassMaster.DsrId + ",'"+ gatepassMaster.CargoName + "'," + b_unit + "," + gatepassMaster.InvID + "," + gatepassMaster.supid + ",'" + gatepassMaster.Address + "','" + gatepassMaster.PhoneNo + "','" + gatepassMaster.Date + "')");
            return RedirectToAction("Create");
        }
        public ActionResult Edit(int? ID, GatepassMaster gatepassMaster, Gatepassdetail gatepassdetail)
        {
            gatepassMaster.Store = _context.Database.SqlQuery<int>("Select Store from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.supid = _context.Database.SqlQuery<decimal>("Select supid from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.Address = _context.Database.SqlQuery<string>("Select Address from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.PhoneNo = _context.Database.SqlQuery<string>("Select PhoneNo from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.DsrId = _context.Database.SqlQuery<int>("Select DsrId from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.CargoName = _context.Database.SqlQuery<string>("Select CargoName from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit=" + Bunit + "").ToList();
            // gatepassMaster = _context.GatepassMaster.SingleOrDefault(c => c.InvID == ID);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var g_detail = _context.Database.SqlQuery<Gatepassdetail>("SELECT * from GatepassDetail where InvID=" + ID + " ").ToList();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            var SaleInvVM = new SaleInvVM
            {
                Store = Store,
                g_detail = g_detail,
                gatepassMaster = gatepassMaster,
                sup_list = sup_list,
                emp_list = emp_list,
                pro_list = pro_list
            };
            return View(SaleInvVM);
        }
        public ActionResult Update( string[] item_name, int[] id, string[] qty, GatepassMaster gatepassMaster, Gatepassdetail gatepassdetail)
        {
            _context.Database.ExecuteSqlCommand("Delete From GatepassDetail where InvId =" + gatepassMaster.InvID + " ");
            _context.Database.ExecuteSqlCommand("Delete From GatepassMaster where InvId =" + gatepassMaster.InvID + "  ");
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            for (int i = 0; i < id.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO GatepassDetail (item_iden,InvId,ItemID,ItemName,Qty,date) VALUES ('0'," + gatepassMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + qty[i] + ",'" + gatepassMaster.Date + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO GatepassMaster (Store,DsrId,CargoName,b_unit,InvID,supid,Address,PhoneNo,Date) VALUES (" + gatepassMaster.Store + "," + gatepassMaster.DsrId + ",'" + gatepassMaster.CargoName + "'," + b_unit + "," + gatepassMaster.InvID + "," + gatepassMaster.supid + ",'" + gatepassMaster.Address + "','" + gatepassMaster.PhoneNo + "','" + gatepassMaster.Date + "')");
            return RedirectToAction("Index");
        }
        public ActionResult GatepassReport(int? ID, GatepassMaster gatepassMaster, Gatepassdetail gatepassdetail)
        {
            gatepassMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.Date = _context.Database.SqlQuery<string>("Select Date from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.supid = _context.Database.SqlQuery<decimal>("Select supid from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.DsrId = _context.Database.SqlQuery<int>("Select DsrId from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            gatepassMaster.CargoName = _context.Database.SqlQuery<string>("Select CargoName from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            ViewBag.Receiver = _context.Database.SqlQuery<string>("Select (Select name From Employees where AccountNo=GatepassMaster.DsrId) as EmployeesName from GatepassMaster where InvID =" + ID + "").FirstOrDefault();
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var supplier = _context.Supplier.SingleOrDefault(c => c.AccountNo == gatepassMaster.supid);
            var employees = _context.Employee.SingleOrDefault(c => c.AccountNo == gatepassMaster.DsrId);
            var gp_detail = _context.Database.SqlQuery<Gatepassdetailquery>("SELECT *,(Select Munit From Products where id=GatepassDetail.ItemID) as Munit from GatepassDetail where InvID=" + ID + "").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                gp_detail = gp_detail,
                gatepassMaster = gatepassMaster,
                Settings = Settings,
                supplier = supplier
            };
            return View(PurInvoiceVM);
        }
    }
}