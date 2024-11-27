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
using System;
namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class SaleWtaxController : Controller
    {
        private ApplicationDbContext _context;
        public SaleWtaxController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: SaleWtax
        public ActionResult Index()
        {
            bool b_status =Convert.ToBoolean(Session["Bunit_status"]);
            var query = "";
            if(b_status == false)
            {
                  query = "AND b_unit = '" + Session["BusinessUnit"] + "' ";
            }
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWTAX' "+ query +" ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult Create(SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasters where InvType = 'SINVWTAX'").FirstOrDefault();
            var cus_list = _context.Customer.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWTAX' ORDER BY InvID DESC").ToList();
            var SaleInvVM = new SaleInvVM
            {
                SaleDetail = SaleDetail,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                sale_list_woc = sale_list_woc,
                pro_list = pro_list
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save( string[] tax_per, string[] tax_amount, string[] n_totalwt, int[] p_box, int[] b_no, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, SaleMaster saleMaster , SaleDetail saleDetail , TransactionDetail TransactionDetail)
        {
            
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO saleDetails (b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType,tax_item,tax_amount,NetTotal_wtax) VALUES ('" + Session["BusinessUnit"] + "'," + saleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + saleMaster.Date + "',0," + p_box[i] + ",'SINVWTAX'," + tax_per[i] + "," + tax_amount[i] + "," + n_totalwt[i] + ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO saleMasters (b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType,Total_Wtax,NetTotal_Wtax,tax_amount) VALUES ('" + Session["BusinessUnit"] + "'," + saleMaster.InvID + "," + saleMaster.CustomerId + "," + saleMaster.DsrId + ",N'" + saleMaster.Address + "','" + saleMaster.Phone + "','" + saleMaster.Date + "',N'" + saleMaster.CargoName + "'," + saleMaster.CargoCharges + "," + saleMaster.NetAmount + "," + saleMaster.DiscountAmount + "," + saleMaster.GrandTotal + "," + saleMaster.Total + "," + saleMaster.Rtotal + "," + saleMaster.BTotal + ",'SINVWTAX'," + saleMaster.Total_Wtax + "," + saleMaster.NetTotal_Wtax + "," + saleMaster.tax_amount + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMaster.Date + "','Customer'," + saleMaster.CustomerId + ",'" + saleMaster.GrandTotal + "',0," + saleMaster.InvID + ",'SINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMaster.Date + "','Sale','222222',0,'" + saleMaster.GrandTotal + "'," + saleMaster.InvID + ",'SINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMaster.Date + "','CGS',333333,'" + saleMaster.GrandTotal + "',0," + saleMaster.InvID + ",'SINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMaster.Date + "','Stock',111111,0,'" + saleMaster.GrandTotal + "'," + saleMaster.InvID + ",'SINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMaster.Date + "','Tax Payable',131313,0,'" + saleMaster.tax_amount + "'," + saleMaster.InvID + ",'SINVWTAX')");

            return RedirectToAction("Create");
        }
    }
}