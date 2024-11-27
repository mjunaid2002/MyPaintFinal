using System;
using System.Linq;
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
    public class TaxSaleInvoiceController : Controller
    {
        private ApplicationDbContext _context;
        public TaxSaleInvoiceController()
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
            string strquery = " AND date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by OrderID";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " AND date between '" + StartDate + "' and '" + Enddate + "' order by OrderID ";

            var list = _context.Database.SqlQuery<SaleReturnQuery>("SELECT OrderID,date,custname,title,total,ntotal,req_status  from srsm where title='TSINV'" + strquery).ToList();
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
           // var list = _context.Database.SqlQuery<SaleReturnDetailQuery>("select  packing,prname,qty,sp,total,dsicval,totalafterdisc,gst  from srsdetail where OrderID =" + ID + " and Status='TSINV'").ToList();
            var list = _context.Database.SqlQuery<SaleReturnDetailQuery>("SELECT packing, prname, qty, sp, total, dsicval, totalafterdisc, gst,ntotal,(SELECT Categories.CategoryName AS productname FROM Product INNER JOIN Categories ON Product.CategoryID = Categories.CategoryID WHERE (Product.ProductID = srsdetail.prid)) AS catagoryname, (SELECT CapDubbi FROM Product AS Product_1 WHERE (ProductID = srsdetail.prid)) AS CapDubbi, (SELECT CapQuarter FROM Product AS Product_1 WHERE (ProductID = srsdetail.prid)) AS CapQuarter, (SELECT CapGallon FROM Product AS Product_1 WHERE (ProductID = srsdetail.prid)) AS CapGallon, (SELECT CapDrum FROM Product AS Product_1 WHERE (ProductID = srsdetail.prid)) AS CapDrum FROM srsdetail WHERE (OrderID = " + ID +") AND (Status = 'TSINV')").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var Regionid = _context.Database.SqlQuery<decimal>("SELECT RegionId FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var region = _context.Database.SqlQuery<string>("SELECT Name FROM Region where id=" + Regionid + "").FirstOrDefault();
            
            
            var cargoid = _context.Database.SqlQuery<decimal>("SELECT cargo FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<string>("SELECT Name FROM cargo where id=" + cargoid + "").FirstOrDefault();
            
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT total FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();

            //var CompanyName = _context.Database.SqlQuery<String>("SELECT company FROM   tbl_setting ").FirstOrDefault();
            //var Email = _context.Database.SqlQuery<String>("SELECT email FROM   tbl_setting ").FirstOrDefault();
            //var Phone = _context.Database.SqlQuery<String>("SELECT telephone FROM   tbl_setting ").FirstOrDefault();
            ////var Image = _context.Database.SqlQuery<String>("SELECT logo FROM   tbl_setting ").FirstOrDefault();
            //var Address = _context.Database.SqlQuery<String>("SELECT address FROM   tbl_setting ").FirstOrDefault();

            var CompanyName = _context.Database.SqlQuery<string>("SELECT company FROM   tbl_setting ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<string>("SELECT email FROM   tbl_setting ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<string>("SELECT telephone FROM   tbl_setting ").FirstOrDefault();
            byte[] Image = _context.Database.SqlQuery<byte[]>("SELECT logo FROM tbl_setting").FirstOrDefault();
            var Address = _context.Database.SqlQuery<string>("SELECT address FROM   tbl_setting ").FirstOrDefault();
            var comntn = _context.Database.SqlQuery<string>("SELECT ntn FROM   tbl_setting ").FirstOrDefault();
            var STRN = _context.Database.SqlQuery<string>("SELECT strn FROM   tbl_setting ").FirstOrDefault();


            var customer = _context.Database.SqlQuery<string>("SELECT isnull((Select name From Customers where customerid=srsm.custid  ),0) as CustomerName  FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT isnull((Select email From Customers where customerid=srsm.custid  ),0) as CustomerName  FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT isnull((Select Phone From Customers where customerid=srsm.custid  ),0) as CustomerName  FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT isnull((Select Address From Customers where customerid=srsm.custid  ),0) as CustomerName  FROM srsm where OrderID =" + ID + " and title='TSINV'").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptTaxSaleReturn.rpt"));

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
                return File(stream, "application/pdf", "SaleReturnInvoice.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("dd-MMM-yyyy");
                
                ViewData["OrderId"] = ID;
               
                ViewData["Region"] = region;
                ViewData["cargo"] = cargo;
                ViewData["customername"] = customer;
                ViewData["cusemail"] = cusemail;
                ViewData["cusphone"] = cusphone;
                ViewData["cusaddress"] = cusaddress;

                ViewData["compname"] = CompanyName;
                ViewData["email"] = Email;
                ViewData["phone"] = Phone;
                ViewData["address"] = Address;
                ViewData["cusntn"] = comntn;
                ViewData["STRN"] = STRN;
                if (Image != null)
                {
                    ViewData["logo"] = Convert.ToBase64String(Image);
                }
                else
                {
                    ViewData["logo"] = "";
                }


                ViewData["type"] = type;

                return View("InvoiceReport", list);
            }
        }
        public ActionResult Create(SaleReturnQuery saleReturnQuery)
        {
            saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                saleReturnQuery = saleReturnQuery,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = Cargo_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Save(SaleReturnQuery saleReturnQuery, string[] packing, string[] item_name, int[] id, decimal[] n_total, decimal[] tax_val, decimal[] tax_amount, decimal[] disc_val, decimal[] disc_value, decimal[] disc_amount, decimal[] wht, decimal[] sp, string[] qty, string[] net)
        {
            saleReturnQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from srsm where title='TSINV'").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srsdetail (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
                    "VALUES (" + tax_val[i] + ",0," + tax_amount[i] + ",0," + wht[i] + ",'TSINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + n_total[i] + "," + qty[i] + "," + disc_val[i] + "," + disc_value[i] + "," + saleReturnQuery.OrderID + ",'" + packing[i] + "'," + net[i] + "," + disc_amount[i] + ")");
            }
            saleReturnQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + saleReturnQuery.custid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srsm (RegionId,gst,OrderID,empname,cargoid,custid,date,total,discount,wht,cargocharges,ntotal,custname,bal,note,pono,custntn,custst,title,time,req_status,cargo ) " +
                "VALUES ('" + saleReturnQuery.RegionId + "'," + saleReturnQuery.gst + "," + saleReturnQuery.OrderID + ",'0'," + saleReturnQuery.inctax + "," + saleReturnQuery.custid + ",'" + saleReturnQuery.date + "'," + saleReturnQuery.total + "," + saleReturnQuery.discount + "," + saleReturnQuery.wht + "," + saleReturnQuery.afterdisc + "," + saleReturnQuery.ntotal + ",'" + saleReturnQuery.custname + "',0,'" + saleReturnQuery.note + "','',0,0,'TSINV',0,'Request','"+ saleReturnQuery.cargo + "')");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + saleReturnQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + saleReturnQuery.ntotal + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + saleReturnQuery.discount + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + saleReturnQuery.total + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Advance tax payable',2100005,0,'" + saleReturnQuery.wht + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales tax payable',2100004,0,'" + saleReturnQuery.gst + "'," + saleReturnQuery.OrderID + ",'TSINV')");

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
            var saleReturnQuery = _context.Database.SqlQuery<SaleReturnQuery>("select [OrderID],[RegionId] ,[empname],[cargo] ,[cargoid] as inctax ,[custid] ,[date] ,[total] ,[gst] ,[discount] ,[wht] ,[cargocharges] as afterdisc ,[ntotal] ,[custname] ,[bal] ,[note] ,[pono] ,[custntn] ,[custst] ,[title] ,[time],[req_status] from srsm where OrderID =" + ID + " and title='TSINV'").SingleOrDefault();
            var saleReturnQueryDetail = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from srsdetail where OrderID =" + ID + " and Status='TSINV'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from Cargo").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                saleReturnQuery = saleReturnQuery,
                saleReturnQueryDetail = saleReturnQueryDetail,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = Cargo_list
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(SaleReturnQuery saleReturnQuery, string[] packing, string[] item_name, int[] id, decimal[] n_total, decimal[] tax_val, decimal[] tax_amount, decimal[] disc_val, decimal[] disc_value, decimal[] disc_amount, decimal[] wht, decimal[] sp, string[] qty, string[] net)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + saleReturnQuery.OrderID + " and Vtype = 'TSINV'");
            _context.Database.ExecuteSqlCommand("Delete From srsm where OrderID =" + saleReturnQuery.OrderID + " and title='TSINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srsdetail where OrderID =" + saleReturnQuery.OrderID + "  and Status='TSINV' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srsdetail (gst,ltrkg,totalgst,peritmdisc,wht,Status,sr,prid,prname,sp,total,qty,dsicval,totalafterdisc,OrderID,packing,ntotal,disc_amount) " +
                    "VALUES (" + tax_val[i] + ",0," + tax_amount[i] + ",0," + wht[i] + ",'TSINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + n_total[i] + "," + qty[i] + "," + disc_val[i] + "," + disc_value[i] + "," + saleReturnQuery.OrderID + ",'" + packing[i] + "'," + net[i] + "," + disc_amount[i] + ")");
            }
            saleReturnQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + saleReturnQuery.custid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srsm (RegionId,gst,OrderID,empname,cargoid,custid,date,total,discount,wht,cargocharges,ntotal,custname,bal,note,pono,custntn,custst,title,time,req_status,cargo) " +
                "VALUES ('" + saleReturnQuery.RegionId + "'," + saleReturnQuery.gst + "," + saleReturnQuery.OrderID + ",'0'," + saleReturnQuery.inctax + "," + saleReturnQuery.custid + ",'" + saleReturnQuery.date + "'," + saleReturnQuery.total + "," + saleReturnQuery.discount + "," + saleReturnQuery.wht + "," + saleReturnQuery.afterdisc + "," + saleReturnQuery.ntotal + ",'" + saleReturnQuery.custname + "',0,'" + saleReturnQuery.note + "','',0,0,'TSINV',0,'Request','"+ saleReturnQuery.cargo + "')");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + saleReturnQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + saleReturnQuery.ntotal + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + saleReturnQuery.discount + "',0," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + saleReturnQuery.total + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Advance tax payable',2100005,0,'" + saleReturnQuery.wht + "'," + saleReturnQuery.OrderID + ",'TSINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + saleReturnQuery.date.ToString("yyyy-MM-dd") + "','Sales tax payable',2100004,0,'" + saleReturnQuery.gst + "'," + saleReturnQuery.OrderID + ",'TSINV')");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select TOP(1) isnull(cp,0) as cp from srpdetail inner join srpm ON srpdetail.invid = srpm.invid where supid=" + code1 + " and pid=" + code + " and  srpm.status='PRINV' order by srpm.invid desc").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActionTax(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT TOP (1) Categories.Description as tax_per FROM Product INNER JOIN Categories ON Product.CategoryID = Categories.CategoryID WHERE     (Product.ProductID = " + code + ")").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }   
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + ID + " and status='PRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + ID + "  and Stauts='PRINV' ");
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
            _context.Database.ExecuteSqlCommand("UPDATE  srsm SET  req_status ='Pending'  where title='TSINV' AND orderid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','TaxSaleInvoice','Sale')");
            return RedirectToAction("Index");
        }


        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  srsm SET req_status = 'Complete' where title='TSINV' AND orderid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Sale' AND batchno='TaxSaleInvoice' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  srsm SET req_status = 'Request' where title='TSINV' AND   orderid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Sale' AND batchno='TaxSaleInvoice' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus", "SaleInvoiceNew");
            }
            return RedirectToAction("Login", "Home");
        }

    }
}