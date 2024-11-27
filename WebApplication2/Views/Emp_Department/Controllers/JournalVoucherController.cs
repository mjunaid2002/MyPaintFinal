using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication2.ViewModels;
using WebApplication1.ViewModels;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
    using System.Net;
using System.Text.RegularExpressions;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class JournalVoucherController : Controller
    {
        private ApplicationDbContext _context;
        public JournalVoucherController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: JournalVoucher
        public ActionResult Index()
        {
            string strquery = " AND TransDate ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " AND TransDate between '" + StartDate + "' and '" + Enddate + "'  ";

            _context.Database.ExecuteSqlCommand("update VoucherMasters set Account=10000001 WHERE     (Account IS NULL)");

            var vou_list = _context.Database.SqlQuery<VoucherMasterQuery>("SELECT TransId, TransDate, ISNULL(SUM(Dr), 0) AS Dr FROM TransactionDetails WHERE (Vtype = 'JV') AND (Dr > 0)  "+ strquery + " GROUP BY TransId, TransDate  ORDER BY TransId DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'JV' ").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {   
                vou_lists = vou_list,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult Create(TransactionDetail TransactionDetail, Voucher Voucher)
        {
            // TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails where VType = 'BPV'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //  TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails  where VType = 'BPV'").FirstOrDefault();
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'JV' ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
           // var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var ac_list = _context.AccountTitle.ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV'   ORDER BY TID DESC").ToList();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                ac_list = ac_list,
                //cus_list = cus_list,
            };
            return View(VoucherVM);
        }
        public ActionResult Create1(TransactionDetail TransactionDetail, Voucher Voucher)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'JV' ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
            var ac_list = _context.AccountTitle.ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV'   ORDER BY TID DESC").ToList();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                ac_list = ac_list,
            };
            return View(VoucherVM);
        }
        public ActionResult Creat(TransactionDetail TransactionDetail, Voucher Voucher)
        {
            // TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails where VType = 'BPV'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //  TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails  where VType = 'BPV'").FirstOrDefault();
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            Voucher.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(invid),0)+1 from VoucherMasters where Month(date)=Month(GETDATE()) and Year(date)= Year(GETDATE()) and VType = 'JV' ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
           // var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var ac_list = _context.AccountTitle.ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV'   ORDER BY TID DESC").ToList();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                ac_list = ac_list,
                //cus_list = cus_list,
            };
            return View(VoucherVM);
        }
        [HttpPost, ActionName("Create1")]
        public ActionResult Save1(HttpPostedFileBase[] file,   string[] to_acc,   string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr,decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + Voucher.Id + ",'JV','0')");
            }
            for (int i = 0; i < to_acc.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + Voucher.Id + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + "," + cr[i] + ",'0',N'" + to_acc[i] + "',N'0'," + TransactionDetail.TransId + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "','" + to_no[i] + "'," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "','0')");
                if (dr[i] != 0)
                {

                    _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + to_no[i] + ",'0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + dr[i] + "," + cr[i] + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
                }
            }
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[0] + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
            return RedirectToAction("Create1");
        }
    
        
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file,   string[] to_acc,   string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr,decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + Voucher.Id + ",'JV','0')");
            }
            for (int i = 0; i < to_acc.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + Voucher.Id + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + "," + cr[i] + ",'0',N'" + to_acc[i] + "',N'0'," + TransactionDetail.TransId + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "','" + to_no[i] + "'," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "','0')");

                if (dr[i] != 0)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + to_no[i] + ",'0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + dr[i] + "," + cr[i] + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
                }
                }
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[0] + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
            return RedirectToAction("Create");
        }
        public ActionResult Edit1(int? id, TransactionDetail TransactionDetail, Voucher Voucher)
        {

            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select isnull(sum(Dr),0) from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select isnull(sum(Cr),0) from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
            TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
            var Acc_List = _context.AccountTitle.ToList();
           // var cus_list = _context.Customer.ToList();
            var Acc_List_cash = _context.AccountTitle.ToList();
            var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV' and TID = " + id + "").ToList();
            var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'JV' and TID = " + id + "").ToList();
            var VoucherVM = new VoucherVM
            {
                from_acc = _context.Database.SqlQuery<Int32>("SELECT Account FROM VoucherMasters where VType = 'JV' and TID = " + id + "").FirstOrDefault() ,
                TransactionDetail = TransactionDetail,
                Acc_List_cash = Acc_List_cash,
                Acc_List = Acc_List,
                vou_list = vou_list,
                Voucher = Voucher,
                vou_det = vou_det,
             //   cus_list = cus_list,
            };
            return View(VoucherVM);
        }
          [HttpPost, ActionName("Edit1")]
        public ActionResult Edit(int id,string[] to_acc, string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr, decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);

            _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + id + " and Vtype='" + Vtype + "'   ");
            _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where TID =" + id + " and Vtype='" + Vtype + "'  ");
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where TransId =" + id + " and Vtype='" + Vtype + "'  ");
            //for (int i = 0; i < to_acc.Count(); i++)
            //{
            //    _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (" + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + TransactionDetail.TransId + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
            //    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype,Rinvid) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + to_no[i] + "'," + dr[i] + ",0,'" + Voucher.Id + "','" + Vtype + "','0')");
            //}
            //_context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + Request["from_acc"] + ",'0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
            //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
            for (int i = 0; i < to_acc.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + "," + cr[i] + ",'0',N'" + to_acc[i] + "',N'0'," + TransactionDetail.TransId + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
                _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype,Rinvid) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "','" + to_no[i] + "'," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "','0')");
                if (dr[i] != 0)
                {
                     _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + to_no[i] + ",'0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + dr[i] + "," + cr[i] + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
                }
            }

            return RedirectToAction("Index");
        }
         public ActionResult JournalReport(int? id, TransactionDetail TransactionDetail)
        {
            TransactionDetail = _context.Database.SqlQuery<TransactionDetail>("select top(1) * from TransactionDetails  where VType = 'JV'  and TransId = " + id + "  ").FirstOrDefault();

            var Acc_List = _context.AccountTitle.ToList();
            var vou_det2=_context.Database.SqlQuery<TransactionDetail>("select * from TransactionDetails  where VType = 'JV'  and TransId = " + id + "  ").ToList();
            ViewBag.AmountWords = NumberToWords(Decimal.ToInt32(_context.Database.SqlQuery<decimal>("select SUM(Dr) from TransactionDetails  where VType = 'JV'  and TransId = " + id + "  ").FirstOrDefault()));
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            var Settings = _context.BusinessUnits.SingleOrDefault();
            var VoucherVM = new VoucherVM
            {
                TransactionDetail = TransactionDetail,
                Acc_List = Acc_List,
                vou_det2 = vou_det2,
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

//public ActionResult Save(HttpPostedFileBase[] file,   string[] to_acc,   string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr,decimal[] cr, string[] narr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
//  {
//      Random r = new Random();
//      int num = r.Next();
//      string ImageName = "";
//      string physicalPath;
//      string img; 
//      for (int i = 0; i < file.Count(); i++)
//      {

//          if (file[i] == null)
//          {
//              img = "demo.jpg";
//          }
//          else
//          {
//              ImageName = System.IO.Path.GetFileName(file[i].FileName);
//              img = num + ImageName;
//              physicalPath = Server.MapPath("~/Uploads/" + img);
//              file[i].SaveAs(physicalPath);
//          }
//          _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype,b_unit) VALUES ('" + img + "'," + Voucher.Id + ",'JV','0')");
//      }
//      for (int i = 0; i < to_acc.Count(); i++)
//      {
//          //_context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + Voucher.Id + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
//          _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Narr,Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (N'" + narr[i] + "'," + dr[i] + "," + cr[i] + ",'0',N'" + to_acc[i] + "',N'0'," + Voucher.Id + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
//          _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[i] + "','" + to_no[i] + "'," + dr[i] + "," + cr[i] + ",'" + Voucher.Id + "','" + Vtype + "')");

//          if (dr[i] != 0)
//          {
//              _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + to_no[i] + ",'0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "'," + dr[i] + "," + cr[i] + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
//          }
//          }
//      //_context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + Voucher.Date + "',N'" + narr[0] + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
//      return RedirectToAction("Create");
//  }
//public ActionResult Edit(int id,string[] to_acc, string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
//{
//    var Bunit = Convert.ToString(Session["BusinessUnit"]);

//    _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + id + " and Vtype='" + Vtype + "'   ");
//    _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where invid =" + id + " and Vtype='" + Vtype + "'  ");
//    _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where invid =" + id + " and Vtype='" + Vtype + "'  ");
//    for (int i = 0; i < to_acc.Count(); i++)
//    {
//        _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (" + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + Voucher.Id + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
//        _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + to_no[i] + "'," + dr[i] + ",0,'" + Voucher.Id + "','" + Vtype + "')");
//    }
//    _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + Request["from_acc"] + ",'0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
//    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
//    return RedirectToAction("Index");
//}

//public ActionResult Edit(int? id, TransactionDetail TransactionDetail, Voucher Voucher)
//{

//    var Bunit = Convert.ToString(Session["BusinessUnit"]);

//    TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'JV' and invid = " + id + "  ").FirstOrDefault();
//    TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select isnull(sum(Dr),0) from TransactionDetails  where VType = 'JV' and invid = " + id + "  ").FirstOrDefault();
//    TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select isnull(sum(Cr),0) from TransactionDetails  where VType = 'JV' and invid = " + id + "  ").FirstOrDefault();
//    TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'JV' and invid = " + id + "  ").FirstOrDefault();
//    TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'JV' and invid = " + id + "  ").FirstOrDefault();
//    var Acc_List = _context.AccountTitle.ToList();
//   // var cus_list = _context.Customer.ToList();
//    var Acc_List_cash = _context.AccountTitle.ToList();
//    var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV' and invid = " + id + "").ToList();
//    var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'JV' and TID = " + id + "").ToList();
//    var VoucherVM = new VoucherVM
//    {
//        from_acc = _context.Database.SqlQuery<Int32>("SELECT Account FROM VoucherMasters where VType = 'JV' and invid = " + id + "").FirstOrDefault() ,
//        TransactionDetail = TransactionDetail,
//        Acc_List_cash = Acc_List_cash,
//        Acc_List = Acc_List,
//        vou_list = vou_list,
//        Voucher = Voucher,
//        vou_det = vou_det,
//     //   cus_list = cus_list,
//    };
//    return View(VoucherVM);
//}


//public ActionResult Edit(int? id, TransactionDetail TransactionDetail, Voucher Voucher)
//{

//    var Bunit = Convert.ToString(Session["BusinessUnit"]);

//    TransactionDetail.TransId = _context.Database.SqlQuery<int>("select TransId from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
//    TransactionDetail.Dr = _context.Database.SqlQuery<decimal>("select isnull(sum(Dr),0) from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
//    TransactionDetail.Cr = _context.Database.SqlQuery<decimal>("select isnull(sum(Cr),0) from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
//    TransactionDetail.TransDes = _context.Database.SqlQuery<string>("select TransDes from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
//    TransactionDetail.TransDate = _context.Database.SqlQuery<string>("select TransDate from TransactionDetails  where VType = 'JV' and TransId = " + id + "  ").FirstOrDefault();
//    var Acc_List = _context.AccountTitle.ToList();
//    // var cus_list = _context.Customer.ToList();
//    var Acc_List_cash = _context.AccountTitle.ToList();
//    var vou_list = _context.Database.SqlQuery<VoucherMaster>("SELECT * FROM VoucherMasters where VType = 'JV' and TID = " + id + "").ToList();
//    var vou_det = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'JV' and TID = " + id + "").ToList();
//    var VoucherVM = new VoucherVM
//    {
//        from_acc = _context.Database.SqlQuery<Int32>("SELECT Account FROM VoucherMasters where VType = 'JV' and TID = " + id + "").FirstOrDefault(),
//        TransactionDetail = TransactionDetail,
//        Acc_List_cash = Acc_List_cash,
//        Acc_List = Acc_List,
//        vou_list = vou_list,
//        Voucher = Voucher,
//        vou_det = vou_det,
//        //   cus_list = cus_list,
//    };
//    return View(VoucherVM);
//}
//[HttpPost]
//public ActionResult Edit(int id, string[] to_acc, string[] cl_date, int[] to_no, string[] c_no, string[] status, decimal[] dr, TransactionDetail TransactionDetail, Voucher Voucher, string Vtype)
//{
//    var Bunit = Convert.ToString(Session["BusinessUnit"]);

//    _context.Database.ExecuteSqlCommand("Delete From Vouchers where TID =" + id + " and Vtype='" + Vtype + "'   ");
//    _context.Database.ExecuteSqlCommand("Delete From VoucherMasters where TID =" + id + " and Vtype='" + Vtype + "'  ");
//    _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where TID =" + id + " and Vtype='" + Vtype + "'  ");
//    for (int i = 0; i < to_acc.Count(); i++)
//    {
//        _context.Database.ExecuteSqlCommand("INSERT INTO Vouchers (Dr,Cr,b_unit,AccountName,BankName,TID,Account_Id,Bank_Account,Cl_date,ChequeNo,Cheque_status,Vtype) VALUES (" + dr[i] + ",0,'0',N'" + to_acc[i] + "',N'0'," + TransactionDetail.TransId + "," + to_no[i] + ",'" + Request["from_acc"] + "','" + cl_date[i] + "','" + c_no[i] + "','" + status[i] + "','" + Vtype + "')");
//        _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype,Rinvid) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + to_no[i] + "'," + dr[i] + ",0,'" + Voucher.Id + "','" + Vtype + "','0')");
//    }
//    _context.Database.ExecuteSqlCommand("INSERT INTO VoucherMasters (Account,b_unit,TID,Date,TDr,TCr,Remarks,VType,invid) VALUES (" + Request["from_acc"] + ",'0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "'," + TransactionDetail.Dr + "," + TransactionDetail.Cr + ",N'" + TransactionDetail.TransDes + "','" + Vtype + "'," + Voucher.Id + ")");
//    _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES ('0'," + TransactionDetail.TransId + ",'" + TransactionDetail.TransDate + "',N'" + TransactionDetail.TransDes + "','" + Request["from_acc"] + "',0," + TransactionDetail.Dr + "," + Voucher.Id + ",'" + Vtype + "')");
//    return RedirectToAction("Index");
//}