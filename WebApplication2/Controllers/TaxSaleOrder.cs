using System;
using System.Collections.Generic;
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

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class TaxSaleOrderController : Controller
    {
        private ApplicationDbContext _context;

        public TaxSaleOrderController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: SaleWctn
        public ActionResult Index()
        {
            string strquery = " AND date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " AND date between '" + StartDate + "' and '" + Enddate + "'  ";
         
            var branchid = Request["branchid"];
            //var branch = Session["Branch"];
            //if (branchid == null && branch != "All")
            //{
            //    branchid = Session["BranchId"].ToString();
            //}
            var branch = Session["Branch"] as List<string>;
            if (branchid == null && !branch.Contains("All"))
            {
                var branchids = Session["BranchId"] as List<int>;
                strquery += " AND branchid IN (" + string.Join(",", branchids) + ")";
            }
            else if (!string.IsNullOrEmpty(branchid))
            {
                strquery += " and branchid = " + branchid;
            }
            strquery += " order by invid";

            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();
            ViewBag.BranchList = Branch;

            var list = _context.Database.SqlQuery<PoMaster>("SELECT invid,date,total,supname,status,req_status,makeSaleInvoice  from qtnm_so where status='TSO'" + strquery).ToList();
            return View(list);
        }
        public ActionResult SaleWctnSS(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptSaleWctnSS.rpt"));

            rd.SetDataSource(list);
            rd.SetParameterValue("compname", CompanyName);
            //rd.SetParameterValue("email", Email);
            rd.SetParameterValue("phone", Phone);
            rd.SetParameterValue("address", Address);
            rd.SetParameterValue("date", date);
            rd.SetParameterValue("invid", ID);
            rd.SetParameterValue("customername", customer);
            rd.SetParameterValue("cargo", cargo);
            rd.SetParameterValue("dis", dis);
            rd.SetParameterValue("grandtotal", grandtotal);
            //rd.SetParameterValue("cusemail", cusemail);C:\Users\Bahrooz Bakht\source\repos\WebApplication2\WebApplication2\Controllers\SaleWctnController.cs
            //rd.SetParameterValue("cusphone", cusphone);
            //rd.SetParameterValue("cusaddress", cusaddress);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptSaleWctnSS.pdf");
        }
        public ActionResult InvoiceReport(int ID, string type)
        {
            var list = _context.Database.SqlQuery<PoDetail>("select *,total+gst as inctax from qtndetail_so where Orderid =" + ID + " and Status='TSO'").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT total FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();

            //var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            //var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            //var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            //var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            //var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<string>("SELECT company FROM   tbl_setting ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<string>("SELECT email FROM   tbl_setting ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<string>("SELECT telephone FROM   tbl_setting ").FirstOrDefault();
            byte[] Image = _context.Database.SqlQuery<byte[]>("SELECT logo FROM tbl_setting").FirstOrDefault();
            var Address = _context.Database.SqlQuery<string>("SELECT address FROM   tbl_setting ").FirstOrDefault();
            var comntn = _context.Database.SqlQuery<string>("SELECT ntn FROM   tbl_setting ").FirstOrDefault();
            var STRN = _context.Database.SqlQuery<string>("SELECT strn FROM   tbl_setting ").FirstOrDefault();

            var customer = _context.Database.SqlQuery<string>("SELECT isnull((Select name From Customers where customerid=qtnm_so.supid  ),0) as CustomerName  FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT isnull((Select email From Customers where customerid=qtnm_so.supid  ),0) as CustomerName  FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT isnull((Select Phone From Customers where customerid=qtnm_so.supid  ),0) as CustomerName  FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT isnull((Select Address From Customers where customerid=qtnm_so.supid  ),0) as CustomerName  FROM qtnm_so where InvID =" + ID + " and status='TSO'").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptTaxSO.rpt"));

                rd.SetDataSource(list);
                rd.SetParameterValue("compname", "MyPaint");
                rd.SetParameterValue("email", "a@a.com");
                rd.SetParameterValue("phone", "03318025500");
                rd.SetParameterValue("address", "Testing Address");

                //rd.SetParameterValue("date", date);
                // rd.SetParameterValue("invid", ID);
                rd.SetParameterValue("date", date);
                rd.SetParameterValue("customername", customer);
                rd.SetParameterValue("cusemail", cusemail);
                rd.SetParameterValue("cusphone", cusphone);
                rd.SetParameterValue("cusaddress", cusaddress);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "TaxSaleInvoice.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("dd-MMM-yyyy");

                ViewData["OrderId"] = ID;
                ViewData["customername"] = customer;
                ViewData["cusemail"] = cusemail;
                ViewData["cusphone"] = cusphone;
                ViewData["cusaddress"] = cusaddress;
                // ViewData["cargo"] = _context.Database.SqlQuery<string>("SELECT isnull((Select name From cargo where id=orderm.cargoid),'') as CargoName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();

                ViewData["compname"] = CompanyName;
                ViewData["email"] = Email;
                ViewData["phone"] = Phone;
                ViewData["address"] = Address;
                ViewData["cusntn"] = comntn;
                if (Image != null)
                {
                    ViewData["logo"] = Convert.ToBase64String(Image);
                }
                else
                {
                    ViewData["logo"] = "";
                }


                return View("InvoiceReport", list);
            }
        }
        public ActionResult Create(PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from qtnm_so where status='TSO'").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                Branch_list = Branch,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        public ActionResult Create1(PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from qtnm_so where status='TSO'").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                Branch_list = Branch,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] packing, string[] item_name, int[] id, decimal[] tax, decimal[] sp, string[] qty, string[] n_total, PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from qtnm_so where status='TSO'").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO qtndetail_so (tax,Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll,psrn ) VALUES (" + tax[i] + ",'TSO'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0,'"+ packing[i] + "')");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO qtnm_so (BranchId,RegionId,Invoicestatus,tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status) VALUES (" + poMaster.BranchId + "," + poMaster.RegionId + ",'" + poMaster.Invoicestatus + "'," + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','TSO','" + DateTime.Now + "',0,0,0,0,0,'Request')");
            return RedirectToAction("Index");
        }
        [HttpPost, ActionName("Create1")]
        public ActionResult Save1(PoMaster poMaster, string[] packing, string[] item_name, int[] id, decimal[] n_total, decimal[] tax_val, decimal[] tax_amount, decimal[] disc_val, decimal[] disc_value, decimal[] disc_amount, decimal[] wht, decimal[] sp, string[] qty, string[] net)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from qtnm_so where status='TSO'").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO qtndetail_so (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
                     "VALUES (" + tax_val[i] + ",0," + tax_amount[i] + ",0," + wht[i] + ",'TSO'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + n_total[i] + "," + qty[i] + "," + disc_val[i] + "," + disc_value[i] + "," + poMaster.invid + ",'" + packing[i] + "'," + net[i] + "," + disc_amount[i] + ")");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO qtnm_so (BranchId,RegionId,Invoicestatus,tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status,wht,ntotal,afterdisc) VALUES (" + poMaster.BranchId + "," + poMaster.RegionId + ",'" + poMaster.Invoicestatus + "'," + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','TSO','" + DateTime.Now + "',0,0,0,0,0,'Request','"+poMaster.wht+"','"+poMaster.ntotal+"','"+poMaster.afterdisc+"')");
            return RedirectToAction("Index");
        }
        public ActionResult SINVWCTNReport(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select NetAmount from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.CustomerId = _context.Database.SqlQuery<int>("Select CustomerId from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.Date = _context.Database.SqlQuery<string>("Select Date from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            //SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID );
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var sett = _context.Settings.SingleOrDefault(c => c.ID == 2);
            var customer = _context.Customer.SingleOrDefault(d => d.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var q_detail = _context.Database.SqlQuery<QuotationQuery>("SELECT *,(Select Munit From Products where id=QuotationDetail.ItemID) as Munit from QuotationDetail where InvID=" + ID + "").ToList();
            var SaleInvVM = new SaleInvVM
            {
                sett = sett,
                q_detail = q_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }
        //public ActionResult Edit(int? ID)
        //{
        //    var poMaster = _context.Database.SqlQuery<PoMaster>("select * from qtnm_so where invid =" + ID + " and status='TSO'").SingleOrDefault();
        //    var poDetail = _context.Database.SqlQuery<PoDetail>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();
        //    var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
        //    var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

        //    //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
        //    var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
        //    var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

        //    var SaleInvVM = new SaleInvVM
        //    {
        //        Branch_list = Branch,
        //        Region_list = Region,
        //        poDetail = poDetail,
        //        poMaster = poMaster,
        //        pro_listsss = pro_listsss,
        //        Cus_list = Cus_list,
        //    };
        //    return View(SaleInvVM);
        //}
        public ActionResult Edit1(int? ID)
        {
            var poMaster = _context.Database.SqlQuery<PoMaster>("select * from qtnm_so where invid =" + ID + " and status='TSO'").SingleOrDefault();
            var poDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Branch_list = Branch,
                Region_list = Region,
                saleReturnQueryDetail= poDetail,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        //public ActionResult Update(decimal[] tax, string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, PoMaster poMaster)
        public ActionResult Update(PoMaster poMaster, string[] packing, string[] item_name, int[] id, decimal[] n_total, decimal[] tax_val, decimal[] tax_amount, decimal[] disc_val, decimal[] disc_value, decimal[] disc_amount, decimal[] wht, decimal[] sp, string[] qty, string[] net)
        {
            _context.Database.ExecuteSqlCommand("Delete From qtnm_so where InvId =" + poMaster.invid + " and status='TSO' ");
            _context.Database.ExecuteSqlCommand("Delete From qtndetail_so where OrderId =" + poMaster.invid + "  and Status='TSO' ");
            //for (int i = 0; i < item_name.Count(); i++)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO qtndetail_so (tax,Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES (" + tax[i] + ",'TSO'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0)");
            //}
            //poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            //_context.Database.ExecuteSqlCommand("INSERT INTO qtnm_so (BranchId,RegionId,Invoicestatus,tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status) VALUES (" + poMaster.BranchId + "," + poMaster.RegionId + ",'" + poMaster.Invoicestatus + "'," + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','TSO','" + DateTime.Now + "',0,0,0,0,0,'Request')");

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO qtndetail_so (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
                     "VALUES (" + tax_val[i] + ",0," + tax_amount[i] + ",0," + wht[i] + ",'TSO'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + n_total[i] + "," + qty[i] + "," + disc_val[i] + "," + disc_value[i] + "," + poMaster.invid + ",'" + packing[i] + "'," + net[i] + "," + disc_amount[i] + ")");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO qtnm_so (BranchId,RegionId,Invoicestatus,tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status,wht,ntotal,afterdisc) VALUES (" + poMaster.BranchId + "," + poMaster.RegionId + ",'" + poMaster.Invoicestatus + "'," + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','TSO','" + DateTime.Now + "',0,0,0,0,0,'Request','" + poMaster.wht + "','" + poMaster.ntotal + "','" + poMaster.afterdisc + "')");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code,int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select TOP(1) isnull(cp,0) as cp from qtndetail_so inner join qtnm_so ON qtndetail_so.invid = qtnm_so.invid where supid="+ code1 + " and pid="+code+ " and  qtnm_so.status='TSO' order by qtnm_so.invid desc").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + ID + " and InvType = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetails where InvId =" + ID + " and InvType = 'SINVWCTN'");
            return RedirectToAction("Index");
        }
        private object NumberToWords(string number)
        {
            throw new NotImplementedException();
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            string new_word = Regex.Replace(words, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            return new_word;
        }
        public ActionResult statusrequest(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("UPDATE  qtnm_so SET  req_status ='Pending'  where status='TSO' AND invid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','TaxSaleOrder','Sale')");
            return RedirectToAction("Index");
        }


        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  qtnm_so SET req_status = 'Complete' where status='TSO' AND invid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Sale' AND batchno='TaxSaleOrder' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  qtnm_so SET req_status = 'Request' where status='TSO' AND   invid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Sale' AND batchno='TaxSaleOrder' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus", "SaleInvoiceNew");
            }
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult RemStock(string code, int code1, int regionid)
        {
             var stock = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(qtndetail_So.qty), 0) AS Expr1 FROM qtnm_so INNER JOIN qtndetail_So ON qtnm_so.invid = qtndetail_So.invid WHERE (qtnm_so.Invoicestatus = 'Pending') AND (qtnm_so.status = 'TSO') AND (qtnm_so.RegionId = "+ regionid + ") AND (qtndetail_So.psrn = '"+ code + "') AND (qtndetail_So.pid = '"+code1+"')");

            var result = new
            {
                stock = stock,
            
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakeSaleInvoice(int? ID)
        {
            //var poMaster = _context.Database.SqlQuery<PoMaster>("select * from qtnm_so where invid =" + ID + " and status='TSO'").SingleOrDefault();
            //var poDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();

            //_context.Database.ExecuteSqlCommand("Update qtnm_so Set makeSaleInvoice = 1 where invid =" + ID + " and status='TSO'  ");

            //SaleReturnQuery saleReturnQuery = new SaleReturnQuery();
            //saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();
            //poMaster.date = DateTime.Today;
            //foreach (var item in poDetail)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO srsdetail (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
            //  "VALUES (" + item.gst + ",0," + item.totalgst + ",0," + item.wht + ",'TSINV'," + item.sr+ "," + item.prid + ",'" + item.prname + "'," + item.sp + "," + item.total + "," + item.qty + ", " + item.dsicval + "," + item.totalafterdisc + "," + saleReturnQuery.OrderID + ",'" + item.packing + "'," + item.ntotal + ",'" + item.disc_amount + "')");

            //}


            // _context.Database.ExecuteSqlCommand("INSERT INTO srsm (BranchId,RegionId,gst,OrderID,empname,cargoid,custid,date,total,discount,wht,cargocharges,ntotal,custname,bal,note,pono,custntn,custst,title,time,req_status,cargo ) " +
            //    "VALUES ('" + poMaster.BranchId + "','" + poMaster.RegionId + "'," + poMaster.tax + "," + saleReturnQuery.OrderID + ",'0'," + poMaster.tax_amount + "," + poMaster.supid + ",'" + poMaster.date + "'," + poMaster.total + "," + poMaster.discount + "," + poMaster.wht + "," + poMaster.afterdisc + "," + poMaster.ntotal + ",'" + poMaster.supname + "',0,'','',0,0,'TSINV',0,'Request','0')");


            //decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            //int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + poMaster.ntotal + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + poMaster.discount + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + poMaster.total + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Advance tax payable',2100005,0,'" + poMaster.tax + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales tax payable',2100004,0,'" + poMaster.wht + "'," + saleReturnQuery.OrderID + ",'TSINV')");

            var saleReturnQuery = _context.Database.SqlQuery<SaleReturnQuery>("select invid as custntn,Cast ([RegionId] AS Decimal) AS RegionId,[BranchId],tax_amount as inctax ,supid as custid ,date ,total ,tax as gst ,discount,wht ,afterdisc ,ntotal,supname as custname,[note],[req_status] from qtnm_so where invid =" + ID + " and status='TSO' ").SingleOrDefault();
            var saleReturnQueryDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();
            saleReturnQuery.date = DateTime.Today;
            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                Branch_list = Branch,
                saleReturnQuery = saleReturnQuery,
                saleReturnQueryDetail = saleReturnQueryDetail,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = Cargo_list
            };
            return View("~/Views/TaxSaleInvoice/Edit.cshtml", SaleInvVM);
        }

        public ActionResult MakeWHTSaleInvoice(int? ID)
        {
            //var poMaster = _context.Database.SqlQuery<PoMaster>("select * from qtnm_so where invid =" + ID + " and status='TSO'").SingleOrDefault();
            //var poDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();

            //_context.Database.ExecuteSqlCommand("Update qtnm_so Set makeSaleInvoice = 1 where invid =" + ID + " and status='TSO'  ");

            // SaleReturnQuery saleReturnQuery = new SaleReturnQuery();
            //saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();
            //poMaster.date = DateTime.Today;
            //foreach (var item in poDetail)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO srsdetail (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
            //  "VALUES (" + item.gst + ",0," + item.totalgst + ",0," + item.wht + ",'WTSINV'," + item.sr + "," + item.prid + ",'" + item.prname + "'," + item.sp + "," + item.total + "," + item.qty + ", " + item.dsicval + "," + item.totalafterdisc + "," + saleReturnQuery.OrderID + ",'" + item.packing + "'," + item.ntotal + ",'" + item.disc_amount + "')");

            //}


            //_context.Database.ExecuteSqlCommand("INSERT INTO srsm (BranchId,RegionId,gst,OrderID,empname,cargoid,custid,date,total,discount,wht,cargocharges,ntotal,custname,bal,note,pono,custntn,custst,title,time,req_status,cargo ) " +
            //   "VALUES ('" + poMaster.BranchId + "','" + poMaster.RegionId + "'," + poMaster.tax + "," + saleReturnQuery.OrderID + ",'0'," + poMaster.tax_amount + "," + poMaster.supid + ",'" + poMaster.date + "'," + poMaster.total + "," + poMaster.discount + "," + poMaster.wht + "," + poMaster.afterdisc + "," + poMaster.ntotal + ",'" + poMaster.supname + "',0,'','',0,0,'WTSINV',0,'Request','0')");


            //decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            //int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + poMaster.ntotal + "',0," + saleReturnQuery.OrderID + ",'WTSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + poMaster.discount + "',0," + saleReturnQuery.OrderID + ",'WTSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + poMaster.total + "'," + saleReturnQuery.OrderID + ",'WTSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Advance tax payable',2100005,0,'" + poMaster.tax + "'," + saleReturnQuery.OrderID + ",'WTSINV')");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales tax payable',2100004,0,'" + poMaster.wht + "'," + saleReturnQuery.OrderID + ",'WTSINV')");


            var saleReturnQuery = _context.Database.SqlQuery<SaleReturnQuery>("select invid as invno,Cast ([RegionId] AS Decimal) AS RegionId,[BranchId],tax_amount as inctax ,supid as custid ,date ,total ,tax as gst ,discount,wht ,afterdisc ,ntotal,supname as custname,[note],[req_status] from qtnm_so where invid =" + ID + " and status='TSO' ").SingleOrDefault();
            var saleReturnQueryDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from qtndetail_so where OrderID =" + ID + " and Status='TSO'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();
            saleReturnQuery.date = DateTime.Today;
             var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                Branch_list = Branch,
                saleReturnQuery = saleReturnQuery,
                saleReturnQueryDetail = saleReturnQueryDetail,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = Cargo_list
            };
            return View("~/Views/WHTSaleInvoice/Edit.cshtml", SaleInvVM);
        }
    }
}