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
    public class TaxPuchaseReturnsController : Controller
    {
        private ApplicationDbContext _context;

        public TaxPuchaseReturnsController()
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
            string strquery = " AND date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by invid";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " AND date between '" + StartDate + "' and '" + Enddate + "' order by invid ";
            var list = _context.Database.SqlQuery<PoMaster>("SELECT invid,date,total,supname,status,req_status  from srpm where status='TPRINV'" + strquery).ToList();
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
        public ActionResult SaleReportWctnView(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptSaleWctnInv.rpt"));

            rd.SetDataSource(list);
            rd.SetParameterValue("compname", CompanyName);
            rd.SetParameterValue("email", Email);
            rd.SetParameterValue("phone", Phone);
            rd.SetParameterValue("address", Address);
            rd.SetParameterValue("date", date);
            rd.SetParameterValue("invid", ID);
            rd.SetParameterValue("customername", customer);
            rd.SetParameterValue("cargo", cargo);
            rd.SetParameterValue("dis", dis);
            rd.SetParameterValue("grandtotal", grandtotal);
            rd.SetParameterValue("cusemail", cusemail);
            rd.SetParameterValue("cusphone", cusphone);
            rd.SetParameterValue("cusaddress", cusaddress);
            rd.SetParameterValue("Image", Image);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptSaleWctnInv.pdf");
        }
        public ActionResult Create(PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from srpm where status='TPRINV'").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();
            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name, int[] id, decimal[] sp, decimal[] tax, string[] qty, string[] n_total, PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from srpm where status='TPRINV'").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srpdetail (tax,Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES ("+ tax[i] + ",'TPRINV'," + i + "," + id[i] + ",'" + item_name[i] + "',"+sp[i]+",0,"+qty[i]+","+n_total[i]+","+poMaster.invid+",0,0)");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid="+poMaster.supid+"").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srpm (tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status ) VALUES (" + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty+"','"+poMaster.note+"'," + poMaster.supid + "," + poMaster.invid+",'"+poMaster.date+"',"+poMaster.total+",'"+ poMaster.supname + "','TPRINV','" + DateTime.Now+"',0,0,0,0,0,'Request')");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            decimal taxsss = poMaster.tax + poMaster.total;
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + poMaster.tax_amount + "','0'," + poMaster.invid + ",'TPRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Stock',10000002,0,'" + poMaster.total + "'," + poMaster.invid + ",'TPRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales tax payable and refundable',5500002,0,'" + poMaster.tax + "'," + poMaster.invid + ",'TPRINV')");


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
            var poMaster = _context.Database.SqlQuery<PoMaster>("select * from srpm where invid =" + ID + " and status='TPRINV'").SingleOrDefault();
            var poDetail = _context.Database.SqlQuery<PoDetail>("select * from srpdetail where invid =" + ID + " and Stauts='TPRINV'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poDetail = poDetail,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] sp, decimal[] tax, string[] qty, string[] n_total, PoMaster poMaster)
        {

            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + poMaster.invid + " and Vtype = 'TPRINV'");
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + poMaster.invid + " and status='TPRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + poMaster.invid + "  and Stauts='TPRINV' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srpdetail (tax,Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES (" + tax[i] + ",'TPRINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srpm (tax,tax_amount,builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status ) VALUES (" + poMaster.tax + "," + poMaster.tax_amount + ",'" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','TPRINV','" + DateTime.Now + "',0,0,0,0,0,'Request')");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            decimal taxsss = poMaster.tax + poMaster.total;
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + poMaster.tax_amount + "','0'," + poMaster.invid + ",'TPRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Stock',10000002,0,'" + poMaster.total + "'," + poMaster.invid + ",'TPRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Sales tax payable and refundable',5500002,0,'" + poMaster.tax + "'," + poMaster.invid + ",'TPRINV')");

            return RedirectToAction("Index");
        }
        public ActionResult InvoiceReport(int ID)
        {
            var list = _context.Database.SqlQuery<PoDetail>("select * from srpdetail where invid =" + ID + " and Stauts='TPRINV'").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            //var grandtotal = _context.Database.SqlQuery<decimal>("SELECT total FROM purchasem where InvID =" + ID + " and status='PINV'").FirstOrDefault();

            //var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            //var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            //var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            //var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            //var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();

            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();


            ViewData["date"] = date.ToString("dd-MMM-yyyy");

            ViewData["OrderId"] = ID;
            ViewData["customername"] = customer;
            ViewData["cusemail"] = cusemail;
            ViewData["cusphone"] = cusphone;
            ViewData["cusaddress"] = cusaddress;
            ViewData["cargo"] = _context.Database.SqlQuery<string>("SELECT isnull((Select name From cargo where id=orderm.cargoid),'') as CargoName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();

            ViewData["compname"] = "MyPaint";
            ViewData["email"] = _context.Database.SqlQuery<String>("SELECT email FROM Settings ").FirstOrDefault();
            ViewData["phone"] = _context.Database.SqlQuery<String>("SELECT telephone FROM Settings ").FirstOrDefault();
            ViewData["address"] = _context.Database.SqlQuery<String>("SELECT address FROM Settings ").FirstOrDefault();

            return View("InvoiceReport", list);

        }

        [HttpPost]
        public ActionResult Action(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select TOP(1) isnull(cp,0) as cp from srpdetail inner join srpm ON srpdetail.invid = srpm.invid where supid=" + code1 + " and pid=" + code + " and srpm.status='TPRINV' order by srpm.invid desc").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult Action(int code)
        //{

        //    var getgrosspackage = _context.Database.SqlQuery<Customer>("Select * From Customers where AccountNo =" + code + "").ToList();
        //    return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + ID + " and status='TPRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + ID + "  and Stauts='TPRINV' ");
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
            _context.Database.ExecuteSqlCommand("UPDATE  srpm SET  req_status ='Pending'  where status='TPRINV' AND invid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','TaxPuchaseReturns','Purchase')");
            return RedirectToAction("Index");
        }


        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  srpm SET req_status = 'Complete' where status='TPRINV' AND   invid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Purchase' AND batchno='TaxPuchaseReturns' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  srpm SET req_status = 'Request' where status='TPRINV' AND   invid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Purchase' AND batchno='TaxPuchaseReturns' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus","StockInward");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}