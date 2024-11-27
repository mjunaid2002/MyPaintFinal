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
    public class SaleInvoiceController : Controller
    {
        private ApplicationDbContext _context;
        public SaleInvoiceController()
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

            var list = _context.Database.SqlQuery<OrderMasterQuery>("SELECT OrderID,date,custname,total,req_status  from Order_Master" + strquery).ToList();
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
            var list = _context.Database.SqlQuery<SaleReturnDetailQuery>("select * from Order_detail  where OrderID =" + ID + "").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM Order_Master where OrderID =" + ID + " ").FirstOrDefault();

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

             var customer = _context.Database.SqlQuery<string>("SELECT isnull((Select name From Customers where customerid=Order_Master.custid  ),0) as CustomerName  FROM Order_Master where OrderID =" + ID + "").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT isnull((Select email From Customers where customerid=Order_Master.custid  ),0) as CustomerName  FROM Order_Master where OrderID =" + ID + "").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT isnull((Select Phone From Customers where customerid=Order_Master.custid  ),0) as CustomerName  FROM Order_Master where OrderID =" + ID + "").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT isnull((Select Address From Customers where customerid=Order_Master.custid  ),0) as CustomerName  FROM Order_Master where OrderID =" + ID + "").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptsaleinv_final.rpt"));

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
                return File(stream, "application/pdf", "SaleInvoice.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("dd-MMM-yyyy");

                ViewData["OrderId"] = ID;
                ViewData["type"] = type;
                ViewData["customername"] = customer;
                ViewData["cusemail"] = cusemail;
                ViewData["cusphone"] = cusphone;
                ViewData["cusaddress"] = cusaddress;

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
            orderMasterQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from Order_Master").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where  RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                orderMasterQuery = orderMasterQuery,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
                Cargo_list = _context.Database.SqlQuery<cargo>("SELECT * from cargo").ToList(),
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(decimal[] disc_val, decimal[] disc_amount, decimal[] total_after,string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, OrderMasterQuery orderMasterQuery)
        {
            orderMasterQuery.OrderID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderID),0)+1 from Order_Master").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Order_detail (disc_amount,packing,disc_val,total_after,sr,prid,prname,sp,ctn,qty,total,OrderID,barcode,pr ) VALUES ("+ disc_amount [i]+ ",'0'," + disc_val[i] + "," + total_after[i] + "," + i + "," + id[i] + ",'" + item_name[i] + "',"+sp[i]+",0,"+qty[i]+","+n_total[i]+","+ orderMasterQuery.OrderID+",0,0)");
            }
            orderMasterQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid="+ orderMasterQuery.custid+"").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO Order_Master (note,custid,cargoid,OrderID,date,total,custname,discount,ntotal,req_status) VALUES ('" + orderMasterQuery.note+"'," + orderMasterQuery.custid + "," + orderMasterQuery.cargoid + "," + orderMasterQuery.OrderID+",'"+ orderMasterQuery.date+"',"+ orderMasterQuery.total+",'"+ orderMasterQuery.custname + "'," + orderMasterQuery.discount + "," + orderMasterQuery.ntotal + ",'Request')");

            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + orderMasterQuery.ntotal + "',0," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + orderMasterQuery.discount + "',0," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + orderMasterQuery.ntotal + "'," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",0,'" + orderMasterQuery.discount + "'," + orderMasterQuery.OrderID + ",'SINVRAW')");

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
            var orderMasterQuery = _context.Database.SqlQuery<OrderMasterQuery>("select discount,ntotal,OrderID,date,custname,total,note,custid,req_status from Order_Master where OrderID =" + ID + " ").SingleOrDefault();
            var OrderDetailQuery = _context.Database.SqlQuery<OrderDetailQuery>("select * from Order_detail where OrderID =" + ID + "").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();

            //var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                OrderDetailQuery = OrderDetailQuery,
                orderMasterQuery = orderMasterQuery,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(decimal[] disc_amount, decimal[] disc_val, decimal[] total_after, string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, OrderMasterQuery orderMasterQuery)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + orderMasterQuery.OrderID + " and vtype = 'SINVRAW'");
            _context.Database.ExecuteSqlCommand("Delete From Order_Master where OrderID =" + orderMasterQuery.OrderID + "");
            _context.Database.ExecuteSqlCommand("Delete From Order_detail where OrderID =" + orderMasterQuery.OrderID + "");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Order_detail (disc_amount,packing,disc_val,total_after,sr,prid,prname,sp,ctn,qty,total,OrderID,barcode,pr ) VALUES (" + disc_amount[i] + ",'0'," + disc_val[i] + "," + total_after[i] + "," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + orderMasterQuery.OrderID + ",0,0)");
            }
            orderMasterQuery.custname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO Order_Master (note,custid,OrderID,date,total,custname,discount,ntotal,req_status) VALUES ('" + orderMasterQuery.note + "'," + orderMasterQuery.custid + "," + orderMasterQuery.OrderID + ",'" + orderMasterQuery.date + "'," + orderMasterQuery.total + ",'" + orderMasterQuery.custname + "'," + orderMasterQuery.discount + "," + orderMasterQuery.ntotal + ",'Request')");

            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + orderMasterQuery.custid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",'" + orderMasterQuery.ntotal + "',0," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales discount',5500003,'" + orderMasterQuery.discount + "',0," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Sales',4400001,0,'" + orderMasterQuery.ntotal + "'," + orderMasterQuery.OrderID + ",'SINVRAW')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + orderMasterQuery.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",0,'" + orderMasterQuery.discount + "'," + orderMasterQuery.OrderID + ",'SINVRAW')");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select TOP(1) isnull(sp,0) as cp from Order_detail inner join Order_Master ON Order_detail.OrderID = Order_Master.OrderID where custid=" + code1 + " and prid=" + code + "  order by Order_Master.OrderID desc").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Action2(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select disc from tbl_Customer_Item_Discount where customerid=" + code1 + " and CategoryID in (select CategoryID from Product where ProductID=" + code + ")").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Action21(int code, int code1)
        {
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("select partywht from Customers where customerid=" + code1 + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From Order_Master where OrderID =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From Order_detail where OrderID =" + ID + "");
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
            _context.Database.ExecuteSqlCommand("UPDATE  Order_Master SET  req_status ='Pending'  where orderid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','SaleInvoice','Sale')");
            return RedirectToAction("Index");
        }


        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  Order_Master SET req_status = 'Complete' where orderid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Sale' AND batchno='SaleInvoice' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  Order_Master SET req_status = 'Request' where   orderid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Sale' AND batchno='SaleInvoice' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus", "SaleInvoiceNew");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}