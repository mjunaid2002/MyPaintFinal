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
    public class PurWctnController : Controller
    {
        private ApplicationDbContext _context;
        public PurWctnController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: PurWctn
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWCTN' ORDER BY InvID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'PINVWCTN'").ToList();

            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                img_list = img_list,
               
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Create(PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWCTN'").FirstOrDefault();
            var sup_list = _context.Supplier.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWCTN' ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {

                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                pur_list_woc = pur_list_woc,
                pro_list = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, int[] ctn, int[] b_no, decimal[] p_box, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype) VALUES ('" + img + "'," + PurMaster.InvID + ",'PINVWCTN')");
            }
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetails (InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,BatchNumber,Expriy,InvType,party,g_passid) VALUES (" + PurMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMaster.Date + "'," + ctn[i] + "," + p_box[i] + "," + b_no[i] + ",'" + exp_date[i] + "','PINVWCTN'," + PurMaster.SupplierId + ",0)");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasters (b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType,Store) VALUES ('" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + PurMaster.DsrId + ",N'" + PurMaster.Address + "','" + PurMaster.Phone + "','" + PurMaster.Date + "',N'" + PurMaster.CargoName + "'," + PurMaster.CargoCharges + "," + PurMaster.NetAmount + "," + PurMaster.DiscountAmount + "," + PurMaster.GrandTotal + "," + PurMaster.Total + "," + PurMaster.Rtotal + "," + PurMaster.BTotal + ",'PINVWCTN',0)");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data


            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Supplier'," + PurMaster.SupplierId + ",0,'" + PurMaster.GrandTotal + "'," + PurMaster.InvID + ",'PINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Stock',1100002,'" + PurMaster.GrandTotal + "',0," + PurMaster.InvID + ",'PINVWCTN')");

            return RedirectToAction("Create");
        }
        [HttpPost]
        public ActionResult Update(TransactionDetail TransactionDetail, PurInvoiceVM PurInvoiceVM, int[] b_no, string[] exp_date, int[] item_id, string[] p_box, string[] item_name, string[] sp, string[] qty, string[] n_total,int[] ctn)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + PurInvoiceVM.PurMaster.InvID + " and Vtype = 'PINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From PurDetails where InvId =" + PurInvoiceVM.PurMaster.InvID + "  and InvType = 'PINVWCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From PurMasters where InvId =" + PurInvoiceVM.PurMaster.InvID + "  and InvType = 'PINVWCTN' ");

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetails (InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,BatchNumber,Expriy,InvType,party,g_passid) VALUES (" + PurInvoiceVM.PurMaster.InvID + "," + item_id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurInvoiceVM.PurMaster.Date + "',"+ctn[i]+"," + p_box[i] + "," + b_no[i] + ",'" + exp_date[i] + "','PINVWCTN'," + PurInvoiceVM.PurMaster.SupplierId + ",0)");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasters (b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType,Store) VALUES ('" + Session["BusinessUnit"] + "'," + PurInvoiceVM.PurMaster.InvID + "," + PurInvoiceVM.PurMaster.SupplierId + "," + PurInvoiceVM.PurMaster.DsrId + ",N'" + PurInvoiceVM.PurMaster.Address + "','" + PurInvoiceVM.PurMaster.Phone + "','" + PurInvoiceVM.PurMaster.Date + "',N'" + PurInvoiceVM.PurMaster.CargoName + "'," + PurInvoiceVM.PurMaster.CargoCharges + "," + PurInvoiceVM.PurMaster.NetAmount + "," + PurInvoiceVM.PurMaster.DiscountAmount + "," + PurInvoiceVM.PurMaster.GrandTotal + "," + PurInvoiceVM.PurMaster.Total + "," + PurInvoiceVM.PurMaster.Rtotal + "," + PurInvoiceVM.PurMaster.BTotal + ",'PINVWCTN',0)");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data


            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.PurMaster.Date + "','Supplier'," + PurInvoiceVM.PurMaster.SupplierId + ",0,'" + PurInvoiceVM.PurMaster.GrandTotal + "'," + PurInvoiceVM.PurMaster.InvID + ",'PINVWCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.PurMaster.Date + "','Stock',1100002,'" + PurInvoiceVM.PurMaster.GrandTotal + "',0," + PurInvoiceVM.PurMaster.InvID + ",'PINVWCTN')");

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'PINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From PurDetails where InvId =" + ID + " and InvType = 'PINVWCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From PurMasters where InvId =" + ID + " and InvType = 'PINVWCTN' ");
            return RedirectToAction("Index");
        }
        public ActionResult PINVWCTNReport(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from PurMasters where InvID =" + ID + " and InvType='PINVWCTN'").FirstOrDefault();
            PurMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from PurMasters where InvID =" + ID + " and InvType='PINVWCTN'").FirstOrDefault();
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWCTN");
            PurMaster.CargoName = NumberToWords(Decimal.ToInt32(PurMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var supplier = _context.Supplier.SingleOrDefault(c => c.AccountNo == PurMaster.SupplierId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT * from PurDetails where InvID=" + ID + " and InvType = 'PINVWCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                p_detail = p_detail,
                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                Settings = Settings,
                supplier = supplier
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Edit(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {

            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvID =" + ID + " and  InvType = 'PINVWCTN'").FirstOrDefault();
            var sup_list = _context.Supplier.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWCTN");
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT * from PurDetails where InvID=" + ID + " and InvType = 'PINVWCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {

                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                p_detail = p_detail,
                pro_list = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Supplier>("Select * From Suppliers where AccountNo =" + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
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
        public ActionResult PurchGstReport(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from PurMasters where InvID =" + ID + " and InvType='PINVWCTN'").FirstOrDefault();
            PurMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from PurMasters where InvID =" + ID + " and InvType='PINVWCTN'").FirstOrDefault();
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWCTN");
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            ViewBag.gst = _context.Database.SqlQuery<decimal>("Select gst from Settings").FirstOrDefault();
            decimal tax = PurMaster.GrandTotal * Convert.ToDecimal(ViewBag.gst) / 100;
            PurMaster.CargoName = NumberToWords(Decimal.ToInt32(PurMaster.GrandTotal + tax));
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var supplier = _context.Supplier.SingleOrDefault(c => c.AccountNo == PurMaster.SupplierId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT * from PurDetails where InvID=" + ID + " and InvType = 'PINVWCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                p_detail = p_detail,
                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                Settings = Settings,
                supplier = supplier
            };
            return View(PurInvoiceVM);
        }


    }
}