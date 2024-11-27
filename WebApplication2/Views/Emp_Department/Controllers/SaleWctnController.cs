using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text.RegularExpressions;
using CRM.Models;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class SaleWctnController : Controller
    {
        private ApplicationDbContext _context;
        public SaleWctnController()
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
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWCTN' and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
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
        public ActionResult Create(SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasters where InvType = 'SINVWCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit='" + Bunit + "'").ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWCTN' and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
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
        public ActionResult Save(string[] ctn,decimal[] p_box, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails (InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" + SaleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',"+ctn[i]+ "," + p_box[i] + ",'SINVWCTN')");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + SaleMaster.Austrialan + "," + SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Customer'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Sale',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','CGS',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Stock',10000002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWCTN')");

            return RedirectToAction("Create");
        }
        public ActionResult SINVWCTNReport(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWCTN");
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }
        public ActionResult Edit(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit='" + Bunit + "'").ToList();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWCTN");
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT * from SaleDetails where InvID=" + ID + " and InvType = 'SINVWCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                pro_list = pro_list
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(TransactionDetail TransactionDetail, SaleInvVM saleInvVM, int[] item_id, decimal[] p_box, string[] item_name, string[] sp, string[] qty, string[] n_total, string[] ctn)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + saleInvVM.SaleMaster.InvID + " and Vtype='SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + saleInvVM.SaleMaster.InvID + " and InvType='SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetails where InvId =" + saleInvVM.SaleMaster.InvID + " and InvType= 'SINVWCTN'");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails (InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" + saleInvVM.SaleMaster.InvID + "," + item_id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + saleInvVM.SaleMaster.Date + "'," + ctn[i] + "," + p_box[i] + ",'SINVWCTN')");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + saleInvVM.SaleMaster.Store + "," + saleInvVM.SaleMaster.US + "," + saleInvVM.SaleMaster.Euro + "," + saleInvVM.SaleMaster.Austrialan + "," + saleInvVM.SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + saleInvVM.SaleMaster.InvID + "," + saleInvVM.SaleMaster.CustomerId + ",1,N'" + saleInvVM.SaleMaster.Address + "','" + saleInvVM.SaleMaster.Phone + "','" + saleInvVM.SaleMaster.Date + "',N'" + saleInvVM.SaleMaster.CargoName + "'," + saleInvVM.SaleMaster.CargoCharges + "," + saleInvVM.SaleMaster.NetAmount + "," + saleInvVM.SaleMaster.DiscountAmount + "," + saleInvVM.SaleMaster.GrandTotal + "," + saleInvVM.SaleMaster.Total + "," + saleInvVM.SaleMaster.Rtotal + "," + saleInvVM.SaleMaster.BTotal + ",'SINVWCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleInvVM.SaleMaster.Date + "','Customer'," + saleInvVM.SaleMaster.CustomerId + ",'" + saleInvVM.SaleMaster.GrandTotal + "',0," + saleInvVM.SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleInvVM.SaleMaster.Date + "','Sale',4400001,0,'" + saleInvVM.SaleMaster.GrandTotal + "'," + saleInvVM.SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleInvVM.SaleMaster.Date + "','CGS',5500001,'" + saleInvVM.SaleMaster.GrandTotal + "',0," + saleInvVM.SaleMaster.InvID + ",'SINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleInvVM.SaleMaster.Date + "','Stock',10000002,0,'" + saleInvVM.SaleMaster.GrandTotal + "'," + saleInvVM.SaleMaster.InvID + ",'SINVWCTN')");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Customers>("Select * From Customers where customerid =" + code + "").ToList();
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
        public ActionResult DeliveryNote(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWCTN");
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }
        public ActionResult SaleGstReport(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWCTN");
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            ViewBag.gst = _context.Database.SqlQuery<decimal>("Select gst from Settings").FirstOrDefault();
            decimal tax = SaleMaster.GrandTotal * Convert.ToDecimal(ViewBag.gst) / 100;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount + tax));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }


    }
}