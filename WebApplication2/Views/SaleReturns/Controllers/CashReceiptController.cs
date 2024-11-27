using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication2.ViewModels;
using WebApplication1.QueryViewModel;

using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text.RegularExpressions;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class CashReceiptController : Controller
    {
        private ApplicationDbContext _context;

        public CashReceiptController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: CashReceipt
        public ActionResult Index()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and b_unit='" + Bunit + "' ORDER BY TID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'CRV' and  b_unit='" + Bunit + "'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                vou_list = vou_list,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult Create(TransactionDetail TransactionDetail, Voucher Voucher)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //  TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails  where VType = 'CRV'").FirstOrDefault();
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'CRV' and b_unit='" + Bunit + "'").FirstOrDefault();
            var Acc_List = _context.AccountTitle.Where(z => z.b_unit == Bunit).ToList();
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005 && z.b_unit == Bunit).ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and b_unit='" + Bunit + "' ORDER BY TID DESC").ToList();
            var VoucherVM = new VoucherVM
            {
                Acc_List_cash = Acc_List_cash,
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                cus_list = cus_list,
            };
            return View(VoucherVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, string[] account_name, int[] account_no, decimal[] dr, decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + Voucher.Id + ",'CRV','" + Session["BusinessUnit"] + "')");
            }
            for (int i = 0; i < account_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (b_unit,AccountName,TID,Account_Id,Dr,Cr,Narr,Vtype,Bank_Account) VALUES ('" + Session["BusinessUnit"] + "',N'" + account_name[i]+"'," + Voucher.Id + "," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + narr[i] + "','" + Vtype + "',0)");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "'," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'"+Voucher.Id+"','" + Vtype + "')");

            }
            _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + TransactionDetail.TransDes + "','" + Request["cash_account"] + "'," + TransactionDetail.Cr + "," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");


            return RedirectToAction("Create");
        }
        public ActionResult Edit(int? id, TransactionDetail TransactionDetail, Voucher Voucher)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'CRV' and invid = " + id + "  and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select Dr from TransactionDetails  where VType = 'CRV' and invid = " + id + "  and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select Cr from TransactionDetails  where VType = 'CRV' and invid = " + id + "  and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'CRV' and invid = " + id + "  and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'CRV' and invid = " + id + "  and b_unit='" + Bunit + "'").FirstOrDefault();
            var Acc_List = _context.AccountTitle.Where(z => z.b_unit == Bunit).ToList();
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005 && z.b_unit == Bunit).ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and invid = " + id + "").ToList();
            var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'CRV' and TID = " + id + "").ToList();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List_cash = Acc_List_cash,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                vou_det = vou_det,
                cus_list = cus_list,
            };
            return View(VoucherVM);
        }
        [HttpPost]
        public ActionResult Edit(int id, string[] account_name, int[] account_no, decimal[] dr, decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);

            _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + id + " and Vtype='" + Vtype + "'  and b_unit='" + Bunit + "' ");
            _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where invid =" + id + " and Vtype='" + Vtype + "'  and b_unit='" + Bunit + "'");
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where invid =" + id + " and Vtype='" + Vtype + "'  and b_unit='" + Bunit + "'");

            for (int i = 0; i < account_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (b_unit,AccountName,TID,Account_Id,Dr,Cr,Narr,Vtype,Bank_Account) VALUES ('" + Session["BusinessUnit"] + "',N'" + account_name[i] + "'," + Voucher.Id + "," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + narr[i] + "','" + Vtype + "',0)");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + narr[i] + "'," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'"+Voucher.Id+"','" + Vtype + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + Request["cash_account"] + "'," + TransactionDetail.Cr + "," + TransactionDetail.Dr + ", " + Voucher.Id + ",'" + Vtype + "')");

            return RedirectToAction("Index");
        }
        public ActionResult CashReceiptReport(int? id, TransactionDetail TransactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select SUM(Dr) from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select Cr from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            TransactionDetail.InvId = _context.Database.SqlQuery<int>("select InvId from TransactionDetails  where VType = 'CRV' and invid = " + id + " and b_unit='" + Bunit + "'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            var Acc_List = _context.AccountTitle.Where(z => z.b_unit == Bunit).ToList();
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005 && z.b_unit == Bunit).ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and invid = " + id + "").ToList();
            var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'CRV' and TID = " + id + "").ToList();
            var Vouchers = _context.VoucherMaster.SingleOrDefault(c => c.invid == id & c.VType == "CRV" & c.b_unit == Bunit);
            ViewBag.AmountWords = NumberToWords(Decimal.ToInt32(TransactionDetail.Dr));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Vouchers = Vouchers,
                // Voucher = Voucher,
                vou_det = vou_det,
                cus_list = cus_list,
                Settings = Settings,
            };
            return View(VoucherVM);
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
        public ActionResult Delete(int? ID, string Vtype)
        {
            _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + ID + " and Vtype='" + Vtype + "'");
            _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where TID =" + ID + " and Vtype='" + Vtype + "'");
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where TransId =" + ID + " and Vtype='" + Vtype + "'");
            return RedirectToAction("Index");
        }
    }
}