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
using static WebApplication1.Controllers.HomeController;

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
            string strquery = " AND date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " AND date between '" + StartDate + "' and '" + Enddate + "'  ";

            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' " + strquery + "  ORDER BY TID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'CRV'").ToList();
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
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'CRV' ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
            //var cus_list = _context.Customer.ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005).ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV'  ORDER BY TID DESC").ToList();
            var VoucherVM = new VoucherVM
            {
                Acc_List_cash = Acc_List_cash,
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                //cus_list = cus_list,
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
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'CRV' ").FirstOrDefault();

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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + Voucher.Id + ",'CRV','0')");
            }
            for (int i = 0; i < account_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (b_unit,AccountName,TID,Account_Id,Dr,Cr,Narr,Vtype,Bank_Account) VALUES ('0',N'" + account_name[i] + "'," + TransactionDetail.TransId + "," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + narr[i] + "','" + Vtype + "',0)");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "'," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "','" + Request["cash_account"] + "'," + cr[i] + "," + dr[i] + "," + Voucher.Id + ",'" + Vtype + "')");
            }
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'','" + Request["cash_account"] + "'," + TransactionDetail.Cr + "," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
            _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + Request["cash_account"] + ",'0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + narr[0] + "','" + Vtype + "'," + Voucher.Id + ")");


            return RedirectToAction("Create");
        }
        public ActionResult Edit(int? id, TransactionDetail TransactionDetail, Voucher Voucher)
        {
            if (id != null && TransactionSettings.RestrictedTransIds.Contains(id.Value))
            {
                return RedirectToAction("Index"); // Redirect to the index page if the id exists in the restricted list
            }
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'CRV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select SUM(Dr) from TransactionDetails  where VType = 'CRV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select SUM(Cr) from TransactionDetails  where VType = 'CRV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'CRV' and invid = " + id + "  ").FirstOrDefault();
            TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'CRV' and TransId = " + id + "  ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
            // var cus_list = _context.Customer.ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005).ToList();
            var VoucherMaster = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and tid = " + id + "").SingleOrDefault();
            var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'CRV' and TID = " + id + "").ToList();

            ViewBag.acc_name = _context.Database.SqlQuery<string>("select AccountTitleName from AccountTitles  where AccountNo = " + VoucherMaster.Account + " ").FirstOrDefault();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List_cash = Acc_List_cash,
                Acc_List = Acc_List,
                Voucher = Voucher,
                vou_det = vou_det,
                VoucherMaster = VoucherMaster,
                //cus_list = cus_list,
            };
            return View(VoucherVM);
        }
        [HttpPost]
        public ActionResult Edit(int? id, string[] account_name, int[] account_no, decimal[] dr, decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
        {
            if (id != null && TransactionSettings.RestrictedTransIds.Contains(id.Value))
            {
                return RedirectToAction("Index"); // Redirect to the index page if the id exists in the restricted list
            }
            var Bunit = Convert.ToString(Session["BusinessUnit"]);

            _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + id + " and Vtype='" + Vtype + "'   ");
            _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where TID =" + id + " and Vtype='" + Vtype + "'  ");
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where TransId =" + id + " and Vtype='" + Vtype + "'  ");

            for (int i = 0; i < account_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (b_unit,AccountName,TID,Account_Id,Dr,Cr,Narr,Vtype,Bank_Account) VALUES ('0',N'" + account_name[i] + "'," + TransactionDetail.TransId + "," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + narr[i] + "','" + Vtype + "',0)");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + narr[i] + "'," + account_no[i] + "," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + narr[i] + "','" + Request["cash_account"] + "'," + cr[i] + "," + dr[i] + "," + Voucher.Id + ",'" + Vtype + "')");

            }
            _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + Request["cash_account"] + ",'0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + narr[0] + "','" + Vtype + "'," + Voucher.Id + ")");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + narr[0] + "','" + Request["cash_account"] + "'," + TransactionDetail.Cr + "," + TransactionDetail.Dr + ", " + Voucher.Id + ",'" + Vtype + "')");

            return RedirectToAction("Index");
        }
        public ActionResult CashReceiptReport(int? id, TransactionDetail TransactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select SUM(Dr) from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select SUM(Cr) from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            TransactionDetail.InvId = _context.Database.SqlQuery<int>("select InvId from TransactionDetails  where VType = 'CRV' and TransId = " + id + " ").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
            // var cus_list = _context.Customer.ToList();
            var Acc_List_cash = _context.AccountTitle.Where(z => z.SecondLevel == 1000005).ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'CRV' and tid = " + id + "").ToList();
            var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'CRV' and TID = " + id + "").ToList();
            var Vouchers = _context.VoucherMaster.SingleOrDefault(c => c.TID == id & c.VType == "CRV");
            ViewBag.AmountWords = NumberToWords(Decimal.ToInt32(TransactionDetail.Dr));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Vouchers = Vouchers,
                // Voucher = Voucher,
                vou_det = vou_det,
                // cus_list = cus_list,
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
            if (ID != null && TransactionSettings.RestrictedTransIds.Contains(ID.Value))
            {
                return RedirectToAction("Index"); // Redirect to the index page if the id exists in the restricted list
            }
            _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + ID + " and Vtype='" + Vtype + "'");
            _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where TID =" + ID + " and Vtype='" + Vtype + "'");
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where TransId =" + ID + " and Vtype='" + Vtype + "'");
            return RedirectToAction("Index");
        }
    }
}