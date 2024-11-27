using System;
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
    public class PurchaseReturnController : Controller
    {
        private ApplicationDbContext _context;
        public PurchaseReturnController()
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
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' and b_unit ='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'PINVWCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult PurchaseReturnList()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasterReturns.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasterReturns.DsrId) as EmployeeName FROM PurMasterReturns where InvType = 'PINVWOCTN' and b_unit ='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'PINVWCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult PurReportView(int ID)
        {
            var list = _context.Database.SqlQuery<PurDetail>("Select * from [PurDetailReturns] where InvId = " + ID + " and InvType='PINVWOCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName  FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName  FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName  FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName  FROM PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "repPurInv.rpt"));

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

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "repPurInv.pdf");
        }
        public ActionResult PINVWOCTNReturnReport(int? ID,PurDetail PurDetail, TransactionDetail TransactionDetail, PurMasterReturn purMasterreturn)
        {
            purMasterreturn.InvID = _context.Database.SqlQuery<int>("Select InvID from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            purMasterreturn.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            purMasterreturn.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            purMasterreturn.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            purMasterreturn.GrandTotal = _context.Database.SqlQuery<decimal>("Select GrandTotal from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            purMasterreturn = _context.Database.SqlQuery<PurMasterReturn>("Select * from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").SingleOrDefault();
            purMasterreturn.CargoName = NumberToWords(Decimal.ToInt32(purMasterreturn.GrandTotal));
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var supplier = _context.Supplier.SingleOrDefault(c => c.AccountNo == purMasterreturn.SupplierId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT *,(Select Munit From Products where id=PurDetailReturns.ItemID) as Munit from PurDetailReturns where InvID=" + ID + " and InvType = 'PINVWOCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                purMasterreturn = purMasterreturn,
                p_detail = p_detail,
                PurDetail = PurDetail,
                TransactionDetail = TransactionDetail,
                Settings = Settings,
                supplier = supplier
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult Create(int inv_id,PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == inv_id && c.InvType == "PINVWOCTN");
            PurMaster.pinv_id = inv_id;
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from [PurMasterReturns] where InvType = 'PINVWOCTN'").FirstOrDefault();
            //  PurMaster.Rtotal = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var product = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit=" + Bunit + "").ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=[PurMasterReturns].SupplierId) as SupplierName ,(Select name From Employees where AccountNo=[PurMasterReturns].DsrId) as EmployeeName FROM [PurMasterReturns] where InvType = 'PINVWOCTN' and b_unit ='" + Bunit + "'  ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                Store = Store,
                batch_no = batch_no,
                pro_list1 = product,
                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                pur_list_woc = pur_list_woc,
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult PurchaseReturnSave(decimal[] length, int[] inv_id, int[] b_no, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {

            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where Rinvid =" + PurMaster.pinv_id + " and Vtype = 'PINV'");
            _context.Database.ExecuteSqlCommand("Delete From [PurDetailReturns] where pinv_id =" + PurMaster.pinv_id + "  and InvType = 'PINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From [PurMasterReturns] where pinv_id =" + PurMaster.pinv_id + "  and InvType = 'PINVWOCTN' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("Update [GatepassDetail] set status = 1 where id = "+new_id[i]+"");
                _context.Database.ExecuteSqlCommand("INSERT INTO [PurDetailReturns] (pinv_id,party,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType,g_passid) VALUES (" + PurMaster.pinv_id + "," + PurMaster.SupplierId + ",'" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMaster.Date + "',0," + length[i] + ",'PINVWOCTN'," + inv_id[i] + ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO [PurMasterReturns] (pinv_id,store,b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + PurMaster.pinv_id + "," + PurMaster.Store + ",'" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + PurMaster.DsrId + ",N'" + PurMaster.Address + "','" + PurMaster.Phone + "','" + PurMaster.Date + "',N'" + PurMaster.CargoName + "'," + PurMaster.CargoCharges + "," + PurMaster.NetAmount + "," + PurMaster.DiscountAmount + "," + PurMaster.GrandTotal + "," + PurMaster.Total + "," + PurMaster.Rtotal + "," + PurMaster.BTotal + ",'PINVWOCTN')");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data


            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + PurMaster.pinv_id + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Supplier'," + PurMaster.SupplierId + ",0,'" + PurMaster.GrandTotal + "'," + PurMaster.InvID + ",'PRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + PurMaster.pinv_id + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Stock',10000002,'" + PurMaster.GrandTotal + "',0," + PurMaster.InvID + ",'PRINV')");

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail,PurMasterReturn purMasterreturn)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from [PurMasterReturns] where InvID =" + ID + " and  InvType = 'PINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            //var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit=" + Bunit + "").ToList();
            purMasterreturn = _context.Database.SqlQuery<PurMasterReturn>("Select * from PurMasterReturns where InvID =" + ID + " and InvType='PINVWOCTN'").SingleOrDefault();
            var sup_list = _context.Supplier.Where(z => z.AccountNo == purMasterreturn.SupplierId).ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT * from [PurDetailReturns] where InvID=" + ID + " and InvType = 'PINVWOCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                purMasterreturn = purMasterreturn,
                Store = Store,
                batch_no = batch_no,
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
        public ActionResult Update(TransactionDetail TransactionDetail, PurInvoiceVM PurInvoiceVM, decimal[] length, int[] b_no, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + PurInvoiceVM.purMasterreturn.InvID + " and Vtype = 'PRINV'");
            _context.Database.ExecuteSqlCommand("Delete From [PurDetailReturns] where InvId =" + PurInvoiceVM.purMasterreturn.InvID + "  and InvType = 'PINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From [PurMasterReturns] where InvId =" + PurInvoiceVM.purMasterreturn.InvID + "  and InvType = 'PINVWOCTN' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("Update [GatepassDetail] set status = 1 where id = "+new_id[i]+"");
                _context.Database.ExecuteSqlCommand("INSERT INTO [PurDetailReturns] (party,pinv_id,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType,g_passid) VALUES (" + PurInvoiceVM.purMasterreturn.SupplierId + "," + PurInvoiceVM.purMasterreturn.pinv_id + ",'" + Session["BusinessUnit"] + "'," + PurInvoiceVM.purMasterreturn.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurInvoiceVM.purMasterreturn.Date + "',0," + length[i] + ",'PINVWOCTN',0)");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO [PurMasterReturns] (pinv_id,Store,b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + PurInvoiceVM.purMasterreturn.pinv_id + "," + PurInvoiceVM.purMasterreturn.Store + ",'" + Session["BusinessUnit"] + "'," + PurInvoiceVM.purMasterreturn.InvID + "," + PurInvoiceVM.purMasterreturn.SupplierId + "," + PurInvoiceVM.purMasterreturn.DsrId + ",N'" + PurInvoiceVM.purMasterreturn.Address + "','" + PurInvoiceVM.purMasterreturn.Phone + "','" + PurInvoiceVM.purMasterreturn.Date + "',N'" + PurInvoiceVM.purMasterreturn.CargoName + "'," + PurInvoiceVM.purMasterreturn.CargoCharges + "," + PurInvoiceVM.purMasterreturn.NetAmount + "," + PurInvoiceVM.purMasterreturn.DiscountAmount + "," + PurInvoiceVM.purMasterreturn.GrandTotal + "," + PurInvoiceVM.purMasterreturn.Total + "," + PurInvoiceVM.purMasterreturn.Rtotal + "," + PurInvoiceVM.purMasterreturn.BTotal + ",'PINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + PurInvoiceVM.purMasterreturn.pinv_id + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.purMasterreturn.Date + "','Supplier'," + PurInvoiceVM.purMasterreturn.SupplierId + ",0,'" + PurInvoiceVM.purMasterreturn.GrandTotal + "'," + PurInvoiceVM.purMasterreturn.InvID + ",'PRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + PurInvoiceVM.purMasterreturn.pinv_id + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.purMasterreturn.Date + "','Stock',10000002,'" + PurInvoiceVM.purMasterreturn.GrandTotal + "',0," + PurInvoiceVM.purMasterreturn.InvID + ",'PRINV')");
            return RedirectToAction("PurchaseReturnList");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'PINV'");
            _context.Database.ExecuteSqlCommand("Delete From [PurDetailReturns] where InvId =" + ID + " and InvType = 'PINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From [PurMasterReturns] where InvId =" + ID + " and InvType = 'PINVWOCTN' ");
            return RedirectToAction("PurchaseReturnList");
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