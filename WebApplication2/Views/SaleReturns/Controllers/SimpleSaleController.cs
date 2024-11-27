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

namespace WebApplication2.Controllers
{
    public class SimpleSaleController : Controller
    {
        // GET: SimpleSale
        private ApplicationDbContext _context;
        public SimpleSaleController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Purchase
        public ActionResult Index()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' " + query + " ORDER BY InvID DESC").ToList();
            var lists = _context.Database.SqlQuery<Salequery>("SELECT *,(Select TOP(1) name From Customers where AccountNo=SaleMasters.CustomerId and BusinessUnit='" + Bunit+ "') as CustomerName ,(Select  TOP(1) name From Employees where AccountNo=SaleMasters.DsrId  and BusinessUnit='" + Bunit + "') as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'SINVWOCTN' and  b_unit='" + Bunit + "'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                lists = lists,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Customer>("Select * From Customers where AccountNo =" + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            SaleMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasters where InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "' ").FirstOrDefault();
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit='" + Bunit + "'").ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select TOP(1) name From Customers where AccountNo=SaleMasters.CustomerId and BusinessUnit='"+ Bunit + "') as CustomerName ,(Select TOP(1) name From Employees where AccountNo=SaleMasters.DsrId and BusinessUnit='" + Bunit + "') as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Store = Store,
                SaleDetail = SaleDetail,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                sale_list_woc = sale_list_woc,
                pro_list = pro_list
            };
            return View("Create", SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, decimal[] p_box, string[] srno, string[] item_name, int[] id, decimal[] sp, decimal[] qty, decimal[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            string trans_des =  "Sale Invoice - " + SaleMaster.InvID + " - " + Request["trans_desc"];
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            string img;
            for (int i = 0; i < file.Count(); i++)
            {

                if (file[i] == null)
                {
                    img = "demo.jpg";
                }
                else
                {
                    ImageName = System.IO.Path.GetFileName(file[i].FileName);
                    img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file[i].SaveAs(physicalPath);
                }
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + SaleMaster.InvID + ",'SINVWOCTN','" + Session["BusinessUnit"] + "')");
            }

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails (d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" +
                    "'0','0','0','0','"+ srno[i] + "','" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + SaleMaster.Austrialan + "," + SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','"+ trans_des + "'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',1100002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");

            //if(SaleMaster.Rtotal > 0)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "'," + SaleMaster.CustomerId + ",0,'" + SaleMaster.Rtotal + "'," + SaleMaster.InvID + ",'SINVWOCTN',1)");
            //    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',1100003,'" + SaleMaster.Rtotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN',1)");

            //}
            TempData["succ"] = "Record Saved Successfully";
            return RedirectToAction("Create");
        }
        public ActionResult SINVWOCTNReport(int? ID,  SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select TOP(1) InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select  TOP(1) CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN" && c.b_unit == Bunit);
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            string bUNit = Convert.ToString(Session["BusinessUnit"]);
            var sett = _context.Settings.SingleOrDefault(c => c.Bunit == bUNit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId && c.BusinessUnit == Bunit);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN'  and  b_unit='" + Bunit + "'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                sett = sett,
                s_detail = s_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }
        public ActionResult SINVWOCTNReport2(int? ID,  SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select TOP(1) InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select  TOP(1) CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN" && c.b_unit == Bunit);
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            string bUNit = Convert.ToString(Session["BusinessUnit"]);
            var sett = _context.Settings.SingleOrDefault(c => c.Bunit == bUNit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId && c.BusinessUnit == Bunit);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN'  and  b_unit='" + Bunit + "'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                sett = sett,
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
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where  BusinessUnit='" + Bunit + "'").ToList();

            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN" && c.b_unit == Bunit);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT * from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN'  and  b_unit='" + Bunit + "'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Store = Store,
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
        public ActionResult Update(HttpPostedFileBase[] file, int[] item_id, decimal[] p_box, string[] item_name, decimal[] sp, decimal[] qty, decimal[] n_total,SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            string img;
            for (int i = 0; i < file.Count(); i++)
            {

                if (file[i] == null)
                {
                    img = "demo.jpg";
                }
                else
                {
                    ImageName = System.IO.Path.GetFileName(file[i].FileName);
                    img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file[i].SaveAs(physicalPath);
                }
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + SaleMaster.InvID + ",'SINVWOCTN','" + Session["BusinessUnit"] + "')");
            }

            string trans_des = "Sale Invoice - " + SaleMaster.InvID + " - " + Request["trans_desc"];
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + SaleMaster.InvID + " and Vtype='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + SaleMaster.InvID + " and InvType='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetails where InvId =" + SaleMaster.InvID + " and InvType= 'SINVWOCTN'");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails (d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" +
                    "'0','0','0','0','0','" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + item_id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + SaleMaster.Austrialan + "," + SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','"+ trans_des + "'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',1100002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
            //if (SaleMaster.Rtotal > 0)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "'," + SaleMaster.CustomerId + ",0,'" + SaleMaster.Rtotal + "'," + SaleMaster.InvID + ",'SINVWOCTN',1)");
            //    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','" + trans_des + "',1100003,'" + SaleMaster.Rtotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN',1)");

            //}
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype='SINV'");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + ID + " and InvType='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetails where InvId =" + ID + " and InvType= 'SINVWOCTN'");
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
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN");
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN'").ToList();
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
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN" && c.b_unit== Bunit);
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select TOP(1) InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select  TOP(1) CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select  TOP(1) DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'  and  b_unit='" + Bunit + "'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            SaleMaster.NetAmount = SaleMaster.NetAmount;
            ViewBag.gst = _context.Database.SqlQuery<decimal>("Select gst from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            decimal tax_val = _context.Database.SqlQuery<decimal>("Select gst from Settings where bunit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            if (tax_val != 0)
            {
                decimal tax = SaleMaster.GrandTotal * tax_val / 100;
                decimal new_total = SaleMaster.GrandTotal + tax;
                SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(new_total));
            }
            else
            {
                SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.GrandTotal));
            }

            string bUNit = Convert.ToString(Session["BusinessUnit"]);
            var sett = _context.Settings.SingleOrDefault(c => c.Bunit == bUNit);
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId && c.BusinessUnit == Bunit);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                sett = sett,
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