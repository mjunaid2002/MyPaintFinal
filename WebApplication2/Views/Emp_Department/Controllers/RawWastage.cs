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
    public class RawWastageController : Controller
    {
        private ApplicationDbContext _context;

        public RawWastageController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "'  ";

            var list = _context.Database.SqlQuery<samplem>("SELECT * from samplem" + strquery).ToList();
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
        public ActionResult InvoiceReport(int ID)
        {
            var list = _context.Database.SqlQuery<sampledetail>("select * from sampledetail where invid =" + ID + "").ToList();
            var date = _context.Database.SqlQuery<string>("SELECT Date FROM samplem where InvID =" + ID + " ").FirstOrDefault();
            var status = _context.Database.SqlQuery<string>("SELECT status FROM samplem where InvID =" + ID + " ").FirstOrDefault();
            //var grandtotal = _context.Database.SqlQuery<decimal>("SELECT total FROM purchasem where InvID =" + ID + " and status='PINV'").FirstOrDefault();

            //var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            //var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            //var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            //var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            //var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptrawwastage.rpt"));

            rd.SetDataSource(list);
            rd.SetParameterValue("date", date);
            rd.SetParameterValue("status", status);
           

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "WastageReport.pdf");
        }
        public ActionResult Create(samplem samplem)
        {
            samplem.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from samplem").FirstOrDefault();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                samplem = samplem,
                pro_listsss = pro_listsss,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, samplem samplem)
        {
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO sampledetail (sr,pid,pname,cp,qty,total,invid,date ) VALUES (" + i + 1 + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + "," + samplem.invid + ",'" + samplem.date + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO samplem (invid,date,total,note,status) VALUES (" + samplem.invid + ",'" + samplem.date + "'," + samplem.total + ",'" + samplem.note + "','" + Request["status"] + "')");
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
            var samplem = _context.Database.SqlQuery<samplem>("select * from samplem where invid =" + ID + "").SingleOrDefault();
            var sampledetail = _context.Database.SqlQuery<sampledetail>("select * from sampledetail where invid =" + ID + "").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                samplem = samplem,
                sampledetail = sampledetail,
                pro_listsss = pro_listsss,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, samplem samplem)
        {
            _context.Database.ExecuteSqlCommand("Delete From sampledetail where InvId =" + samplem.invid + " ");
            _context.Database.ExecuteSqlCommand("Delete From samplem where InvId =" + samplem.invid + "");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO sampledetail (sr,pid,pname,cp,qty,total,invid,date ) VALUES (" + i + 1 + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + "," + samplem.invid + ",'" + samplem.date + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO samplem (invid,date,total,note,status) VALUES (" + samplem.invid + ",'" + samplem.date + "'," + samplem.total + ",'" + samplem.note + "','" + Request["status"] + "')");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select TOP(1) isnull(cp,0) as cp from qtndetail inner join qtnm ON qtndetail.invid = qtnm.invid where supid=" + code1 + " and pid=" + code + " and  qtnm.status='PO' order by qtnm.invid desc").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetstockValue(int value)
        {
            var allow_list = _context.Database.SqlQuery<Products>("Select SUM(qty),cp from purchasedetail where pid=" + value + " group by cp").ToList();
            var allow_list1 = _context.Database.SqlQuery<Products>("Select SUM(vattax),UnitPrice as cp from Product where ProductID=" + value + " group by UnitPrice").ToList();

            if (allow_list.Count == 0)
            {
                return Json(allow_list1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (allow_list1.Count > 0)
                {
                    allow_list.Clear();
                    foreach (var itsd in allow_list1)
                    {
                        allow_list.Add(itsd);
                    }

                    allow_list1 = _context.Database.SqlQuery<Products>("Select SUM(qty),cp from purchasedetail where pid=" + value + " group by cp").ToList();
                    foreach (var itsd in allow_list1)
                    {
                        allow_list.Add(itsd);
                    }

                    allow_list1.Clear();
                    allow_list1 = _context.Database.SqlQuery<Products>("SELECT SUM(Stand_total_weight) AS qty, kgcoast as cp FROM labm WHERE (pid = " + value + ") GROUP BY kgcoast").ToList();
                    foreach (var itsd in allow_list1)
                    {
                        allow_list.Add(itsd);
                    }

                }

                return Json(allow_list, JsonRequestBehavior.AllowGet);
            }
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

    }
}