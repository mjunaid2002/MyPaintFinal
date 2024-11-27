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
    public class SaleInvoiceNewController : Controller
    {
        private ApplicationDbContext _context;
        private List<PoDetail> getgrosspackage;
        public SaleInvoiceNewController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by OrderID";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "' order by OrderID ";
            var list = _context.Database.SqlQuery<OrderMasterQuery>("SELECT OrderID,date,custname,total,req_status  from orderm" + strquery).ToList();
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
            var list = _context.Database.SqlQuery<SaleReturnDetailQuery>("select packing,disc_val as dsicval,total_after as totalafterdisc,sr,prid,prname,sp,ctn,qty,total,OrderID,barcode,pr from Orderdetail  where OrderID =" + ID + "").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM orderm where OrderID =" + ID + " ").FirstOrDefault();
            var Regionid = _context.Database.SqlQuery<decimal>("SELECT RegionId FROM orderm where OrderId=" + ID + "").FirstOrDefault();
            var region = _context.Database.SqlQuery<string>("SELECT Name FROM Region where id=" + Regionid + "").FirstOrDefault();

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

            var customer = _context.Database.SqlQuery<string>("SELECT isnull((Select name From Customers where customerid=orderm.custid  ),0) as CustomerName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT isnull((Select email From Customers where customerid=orderm.custid  ),0) as CustomerName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT isnull((Select Phone From Customers where customerid=orderm.custid  ),0) as CustomerName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT isnull((Select Address From Customers where customerid=orderm.custid  ),0) as CustomerName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptsaleinv_final_raw.rpt"));

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
                rd.SetParameterValue("cargo", _context.Database.SqlQuery<string>("SELECT isnull((Select name From cargo where id=orderm.cargoid),'') as CargoName  FROM orderm where OrderID =" + ID + "").FirstOrDefault());

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "SaleInvoice.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("dd-MMM-yyyy");

                ViewData["OrderId"] = ID;
                ViewData["Region"] = region;
                ViewData["customername"] = customer;
                ViewData["cusemail"] = cusemail;
                ViewData["cusphone"] = cusphone;
                ViewData["cusaddress"] = cusaddress;
                ViewData["cargo"] = _context.Database.SqlQuery<string>("SELECT isnull((Select name From cargo where id=orderm.cargoid),'') as CargoName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();

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
        public ActionResult Create(OrderMasterQuery orderMasterQuery )
        {
            orderMasterQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from orderm").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                orderMasterQuery = orderMasterQuery,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = Cargo_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] packing, decimal[] disc_amount,  decimal[] disc_val, decimal[] total_after,string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, OrderMasterQuery orderMasterQuery)
        {
            orderMasterQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from orderm").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Orderdetail (dsicval,packing,disc_val,total_after,sr,prid,prname,sp,ctn,qty,total,OrderID,barcode,pr ) VALUES (" + disc_amount[i] + ",'" + packing[i] + "'," + disc_val[i] + "," + total_after[i] + "," + i + "," + id[i] + ",'" + item_name[i] + "',"+sp[i]+",0,"+qty[i]+","+n_total[i]+","+ orderMasterQuery.OrderID+",0,0)");
            }
            orderMasterQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid="+ orderMasterQuery.custid+"").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO orderm (RegionId,ntotal,note,custid,OrderID,date,total,custname,discount,cargoid,req_status) VALUES (" + orderMasterQuery.RegionId + "," + orderMasterQuery.ntotal + ",'" + orderMasterQuery.note+"'," + orderMasterQuery.custid + "," + orderMasterQuery.OrderID+",'"+ orderMasterQuery.date+"',"+ orderMasterQuery.total+",'"+ orderMasterQuery.custname + "'," + orderMasterQuery.discount + "," + orderMasterQuery.cargoid + ",'Request')");

            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Customer'," + accountno + ",'" + orderMasterQuery.ntotal + "',0," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + orderMasterQuery.discount + "',0," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + orderMasterQuery.ntotal + "'," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Customer'," + accountno + ",0,'" + orderMasterQuery.discount + "'," + orderMasterQuery.OrderID + ",'SINV')");
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
        public ActionResult Edit(int? ID)
        {
            var orderMasterQuery = _context.Database.SqlQuery<OrderMasterQuery>("select RegionId,discount,ntotal,OrderID,date,custname,total,note,custid,req_status from orderm where OrderID =" + ID + " ").SingleOrDefault();
            var OrderDetailQuery = _context.Database.SqlQuery<OrderDetailQuery>("select * from Orderdetail where OrderID =" + ID + "").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                OrderDetailQuery = OrderDetailQuery,
                orderMasterQuery = orderMasterQuery,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(decimal[] disc_amount, string[] packing, decimal[] disc_val, decimal[] total_after, string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, OrderMasterQuery orderMasterQuery)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + orderMasterQuery.OrderID + " and Vtype = 'SINV'");
            _context.Database.ExecuteSqlCommand("Delete From orderm where OrderID =" + orderMasterQuery.OrderID + "");
            _context.Database.ExecuteSqlCommand("Delete From Orderdetail where OrderID =" + orderMasterQuery.OrderID + "");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Orderdetail (dsicval,packing,disc_val,total_after,sr,prid,prname,sp,ctn,qty,total,OrderID,barcode,pr ) VALUES (" + disc_amount[i] + ",'" + packing[i] + "'," + disc_val[i] + "," + total_after[i] + "," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + orderMasterQuery.OrderID + ",0,0)");
            }
            orderMasterQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO orderm (RegionId,ntotal,note,custid,OrderID,date,total,custname,discount,req_status) VALUES (" + orderMasterQuery.RegionId + "," + orderMasterQuery.ntotal + ",'" + orderMasterQuery.note + "'," + orderMasterQuery.custid + "," + orderMasterQuery.OrderID + ",'" + orderMasterQuery.date + "'," + orderMasterQuery.total + ",'" + orderMasterQuery.custname + "'," + orderMasterQuery.discount + ",'Request')");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Customer'," + accountno + ",'" + orderMasterQuery.ntotal + "',0," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + orderMasterQuery.discount + "',0," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + orderMasterQuery.ntotal + "'," + orderMasterQuery.OrderID + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Customer'," + accountno + ",0,'" + orderMasterQuery.discount + "'," + orderMasterQuery.OrderID + ",'SINV')");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(string code, int code1)
        {
            if(code == "Dubbi")
            {
                 getgrosspackage = _context.Database.SqlQuery<PoDetail>("select sp from Product where productid="+ code1 + "").ToList();
            }
            if (code == "Quarter")
            {
                 getgrosspackage = _context.Database.SqlQuery<PoDetail>("select splower as sp from Product where productid=" + code1 + "").ToList();
            }
            if (code == "Drum")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("select drum as sp from Product where productid=" + code1 + "").ToList();
            }
            if (code == "Gallon")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("select gallon as sp from Product where productid=" + code1 + "").ToList();
            }

            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult ActionRate(string code, int code1,int regionid)
        {
          
            if (code == "Dubbi")
            {
                 getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  dubi as sp FROM ProductPricingRegion WHERE (RegionID = "+ regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = "+ code1 + ")))").ToList();
                
            }
            if (code == "Quarter")
            {
                 getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  quarter as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
          }
            if (code == "Drum")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  drum as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
            }
            if (code == "Gallon")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  gallon as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
            }
           
           
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActionRate1(string code, int code1, int regionid)
        {
            var StartDate ="2017-01-01";
            var Enddate = DateTime.Today;
            var region_id = regionid;
            var stock = 0;
            var query = "SELECT ProductID, ProductName, StkOpenDubbi, StkOpenQuarter, StkOpenGallon, StkOpenDrum, StkInDubbi, StkInQuarter, StkInGallon, StkInDrum, StkOutDubbi, StkOutQuarter, StkOutGallon, StkOutDrum, StkOpenDubbi + StkInDubbi - StkOutDubbi AS StkDubbi, StkOpenQuarter + StkInQuarter - StkOutQuarter AS StkQuarter, StkOpenGallon + StkInGallon - StkOutGallon AS StkGallon, StkOpenDrum + StkInDrum - StkOutDrum AS StkDrum FROM (SELECT ProductID, ProductName, OpeningDubbi + BatchFillingDubbiBefore + SaleReturnDubbiBefore - SaleDubbiBefore - SaleTwoDubbiBefore - DemageDubbiBefore + InDubbiBefore - OutDubbiBefore - StockDistDubbiBefore AS StkOpenDubbi, OpeningQuarter + BatchFillingQuarterBefore + SaleReturnQuarterBefore - SaleQuarterBefore - SaleTwoQuarterBefore - DemageQuarterBefore + InQuarterBefore - OutQuarterBefore -StockDistQuarterBefore AS StkOpenQuarter, OpeningGallon + BatchFillingGallonBefore + SaleReturnGallonBefore - SaleGallonBefore - SaleTwoGallonBefore - DemageGallonBefore + InGallonBefore - OutGallonBefore - StockDistGallonBefore AS StkOpenGallon, OpeningDrum + BatchFillingDrumBefore + SaleReturnDrumBefore - SaleDrumBefore - SaleTwoDrumBefore - DemageDrumBefore + InDrumBefore - OutDrumBefore -StockDistDrumBefore AS StkOpenDrum, BatchFillingDubbiAfter + SaleReturnDubbiAfter + InDubbiAfter AS StkInDubbi, BatchFillingQuarterAfter + SaleReturnQuarterAfter + InQuarterAfter AS StkInQuarter, BatchFillingGallonAfter + SaleReturnGallonAfter + InGallonAfter AS StkInGallon, BatchFillingDrumAfter + SaleReturnDrumAfter + InDrumAfter AS StkInDrum, SaleDubbiAfter + SaleTwoDubbiAfter + DemageDubbiAfter + OutDubbiAfter + StockDistDubbiAfter AS StkOutDubbi, SaleQuarterAfter + SaleTwoQuarterAfter + DemageQuarterAfter + OutQuarterAfter + StockDistQuarterAfter AS StkOutQuarter, SaleGallonAfter + SaleTwoGallonAfter + DemageGallonAfter + OutGallonAfter + StockDistGallonAfter AS StkOutGallon, SaleDrumAfter + SaleTwoDrumAfter + DemageDrumAfter + OutDrumAfter + StockDistDrumAfter AS StkOutDrum FROM (SELECT ProductID, ProductName, (SELECT ISNULL(SUM(dubi_o), 0) AS Expr1 FROM ProductFinishedRegion WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningDubbi, (SELECT ISNULL(SUM(quarter_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_3 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningQuarter, (SELECT ISNULL(SUM(gallon_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_2 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningGallon, (SELECT ISNULL(SUM(drum_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_1 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningDrum, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistDubbiBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistQuarterBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistGallonBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistDrumBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDubbiAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistQuarterAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistGallonAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDrumAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InDubbiAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InQuarterAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InGallonAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InDrumAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutDubbiAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutQuarterAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutGallonAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutDrumAfter, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDubbiBefore, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDubbiAfter, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingQuarterBefore, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingQuarterAfter, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingGallonBefore, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingGallonAfter, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDrumBefore, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDrumAfter, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID AND srsdetail.Status = srsm.title WHERE (srsdetail.packing = 'Dubbi') AND (srsdetail.Status IN ('SRINV', 'TSRINV')) AND (srsdetail.prid = Product.ProductID) AND (srsm.RegionId =" + region_id + ") AND (srsm.date < '" + StartDate + "')) AS SaleReturnDubbiBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Dubbi') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T) AS SaleReturnDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_4.prid = Product.ProductID) AND (srsm_4.RegionId =" + region_id + ") AND (srsm_4.date < '" + StartDate + "')) AS SaleReturnQuarterBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Quarter') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_7) AS SaleReturnQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_3.prid = Product.ProductID) AND (srsm_3.RegionId =" + region_id + ") AND (srsm_3.date < '" + StartDate + "')) AS SaleReturnGallonBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Gallon') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_6) AS SaleReturnGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_2.prid = Product.ProductID) AND (srsm_2.RegionId =" + region_id + ") AND (srsm_2.date < '" + StartDate + "')) AS SaleReturnDrumBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Drum') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_5) AS SaleReturnDrumAfter, (SELECT ISNULL(SUM(srsdetail_5.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_5 INNER JOIN srsm AS srsm_5 ON srsdetail_5.OrderID = srsm_5.OrderID AND srsdetail_5.Status = srsm_5.title WHERE (srsdetail_5.packing = 'Dubbi') AND (srsdetail_5.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_5.prid = Product.ProductID) AND (srsm_5.RegionId =" + region_id + ") AND (srsm_5.date < '" + StartDate + "')) AS SaleDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_9 WHERE (packing = 'Dubbi') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_9 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_9.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_4.prid = Product.ProductID) AND (srsm_4.RegionId =" + region_id + ") AND (srsm_4.date < '" + StartDate + "')) AS SaleQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_8 WHERE (packing = 'Quarter') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_8 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_8.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_3.prid = Product.ProductID) AND (srsm_3.RegionId =" + region_id + ") AND (srsm_3.date < '" + StartDate + "')) AS SaleGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_7 WHERE (packing = 'Gallon') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_7 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_7.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_2.prid = Product.ProductID) AND (srsm_2.RegionId =" + region_id + ") AND (srsm_2.date < '" + StartDate + "')) AS SaleDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 WHERE (packing = 'Drum') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_1 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_1.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDrumAfter, (SELECT ISNULL(SUM(Orderdetail.qty), 0) AS Expr1 FROM Orderdetail INNER JOIN orderm ON Orderdetail.OrderID = orderm.OrderID WHERE (Orderdetail.packing = 'Dubbi') AND (Orderdetail.prid = Product.ProductID) AND (orderm.RegionId =" + region_id + ") AND (orderm.date < '" + StartDate + "')) AS SaleTwoDubbiBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Dubbi') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDubbiAfter, (SELECT ISNULL(SUM(Orderdetail_4.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_4 INNER JOIN orderm AS orderm_4 ON Orderdetail_4.OrderID = orderm_4.OrderID WHERE (Orderdetail_4.packing = 'Quarter') AND (Orderdetail_4.prid = Product.ProductID) AND (orderm_4.RegionId =" + region_id + ") AND (orderm_4.date < '" + StartDate + "')) AS SaleTwoQuarterBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Quarter') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoQuarterAfter, (SELECT ISNULL(SUM(Orderdetail_3.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_3 INNER JOIN orderm AS orderm_3 ON Orderdetail_3.OrderID = orderm_3.OrderID WHERE (Orderdetail_3.packing = 'Gallon') AND (Orderdetail_3.prid = Product.ProductID) AND (orderm_3.RegionId =" + region_id + ") AND (orderm_3.date < '" + StartDate + "')) AS SaleTwoGallonBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Gallon') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoGallonAfter, (SELECT ISNULL(SUM(Orderdetail_2.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_2 INNER JOIN orderm AS orderm_2 ON Orderdetail_2.OrderID = orderm_2.OrderID WHERE (Orderdetail_2.packing = 'Drum') AND (Orderdetail_2.prid = Product.ProductID) AND (orderm_2.RegionId =" + region_id + ") AND (orderm_2.date < '" + StartDate + "')) AS SaleTwoDrumBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Drum') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDrumAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (sampledetail2.packing = 'Dubbi') AND (sampledetail2.pid = Product.ProductID) AND (samplem2.regionid =" + region_id + ") AND (sampledetail2.date < '" + StartDate + "')) AS DemageDubbiBefore, (SELECT ISNULL(SUM(sampledetail2_7.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_7 INNER JOIN samplem2 AS samplem2_7 ON sampledetail2_7.invid = samplem2_7.invid WHERE (sampledetail2_7.packing = 'Dubbi') AND (sampledetail2_7.pid = Product.ProductID) AND (samplem2_7.regionid =" + region_id + ") AND (sampledetail2_7.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDubbiAfter, (SELECT ISNULL(SUM(sampledetail2_6.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_6 INNER JOIN samplem2 AS samplem2_6 ON sampledetail2_6.invid = samplem2_6.invid WHERE (sampledetail2_6.packing = 'Quarter') AND (sampledetail2_6.pid = Product.ProductID) AND (samplem2_6.regionid =" + region_id + ") AND (sampledetail2_6.date < '" + StartDate + "')) AS DemageQuarterBefore, (SELECT ISNULL(SUM(sampledetail2_5.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_5 INNER JOIN samplem2 AS samplem2_5 ON sampledetail2_5.invid = samplem2_5.invid WHERE (sampledetail2_5.packing = 'Quarter') AND (sampledetail2_5.pid = Product.ProductID) AND (samplem2_5.regionid =" + region_id + ") AND (sampledetail2_5.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageQuarterAfter, (SELECT ISNULL(SUM(sampledetail2_4.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_4 INNER JOIN samplem2 AS samplem2_4 ON sampledetail2_4.invid = samplem2_4.invid WHERE (sampledetail2_4.packing = 'Gallon') AND (sampledetail2_4.pid = Product.ProductID) AND (samplem2_4.regionid =" + region_id + ") AND (sampledetail2_4.date < '" + StartDate + "')) AS DemageGallonBefore, (SELECT ISNULL(SUM(sampledetail2_3.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_3 INNER JOIN samplem2 AS samplem2_3 ON sampledetail2_3.invid = samplem2_3.invid WHERE (sampledetail2_3.packing = 'Gallon') AND (sampledetail2_3.pid = Product.ProductID) AND (samplem2_3.regionid =" + region_id + ") AND (sampledetail2_3.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageGallonAfter, (SELECT ISNULL(SUM(sampledetail2_2.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_2 INNER JOIN samplem2 AS samplem2_2 ON sampledetail2_2.invid = samplem2_2.invid WHERE (sampledetail2_2.packing = 'Drum') AND (sampledetail2_2.pid = Product.ProductID) AND (samplem2_2.regionid =" + region_id + ") AND (sampledetail2_2.date < '" + StartDate + "')) AS DemageDrumBefore, (SELECT ISNULL(SUM(sampledetail2_1.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_1 INNER JOIN samplem2 AS samplem2_1 ON sampledetail2_1.invid = samplem2_1.invid WHERE (sampledetail2_1.packing = 'Drum') AND (sampledetail2_1.pid = Product.ProductID) AND (samplem2_1.regionid =" + region_id + ") AND (sampledetail2_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDrumAfter FROM Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))) AS derivedtbl_1) AS derivedtbl_2 where ProductID='" + code1 + "' ORDER BY ProductName";
            var item_ledger = _context.Database.SqlQuery<FinishedGoodsQuery2>(query).ToList();
            if (code == "Dubbi")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  dubi as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();

                stock = (int)item_ledger.Where(x => x.ProductID == code1).Select(x => x.StkDubbi).FirstOrDefault();
            }
            if (code == "Quarter")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  quarter as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
                stock = (int)item_ledger.Where(x => x.ProductID == code1).Select(x => x.StkQuarter).FirstOrDefault();
            }
            if (code == "Drum")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  drum as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
                stock = (int)item_ledger.Where(x => x.ProductID == code1).Select(x => x.StkDrum).FirstOrDefault();
            }
            if (code == "Gallon")
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT  gallon as sp FROM ProductPricingRegion WHERE (RegionID = " + regionid + ") AND (CategoryID = (SELECT CategoryID FROM Product WHERE (ProductID = " + code1 + ")))").ToList();
                stock = (int)item_ledger.Where(x => x.ProductID == code1).Select(x => x.StkGallon).FirstOrDefault();

            }
            var result = new
            {
                stock = stock,
                getgrosspackage = getgrosspackage
            };
            ViewBag.stock = stock;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Action2(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select disc from tbl_Customer_Item_Discount where customerid="+ code1 + " and CategoryID in (select CategoryID from Product where ProductID="+ code + ")").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From orderm where OrderID =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From Orderdetail where OrderID =" + ID + "");
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

        public ActionResult GetSaleCount()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Sale").Count();
            ViewBag.BatchCount = batchCount;
            return PartialView("GetSaleCount");
        }
        [HttpGet]
        public JsonResult GetSaleCountJson()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Sale").Count();
            return Json(new { batchCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Indexstatus()
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                var list = _context.Database.SqlQuery<tbl_BatchRequest>("SELECT *  from tbl_BatchRequest where (tbl_BatchRequest.status = 'Requested') AND department='Sale'").ToList();
                return View(list);

            }
            return RedirectToAction("Login", "Home");
        }


        public ActionResult statusrequest(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("UPDATE  orderm SET  req_status ='Pending'  where orderid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','SaleInvoiceNew','Sale')");
            return RedirectToAction("Index");
        }


        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  orderm SET req_status = 'Complete' where orderid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Sale' AND batchno='SaleInvoiceNew' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  orderm SET req_status = 'Request' where   orderid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Sale' AND batchno='SaleInvoiceNew' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus", "SaleInvoiceNew");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}