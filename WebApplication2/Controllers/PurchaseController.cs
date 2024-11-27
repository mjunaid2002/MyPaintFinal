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
    public class PurchaseController : Controller
    {
        private ApplicationDbContext _context;
        public PurchaseController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Purchase
        public ActionResult GatepassIndex()
        {
            var gate_list = _context.Database.SqlQuery<GatepassQuery>("SELECT *,(Select name From Suppliers where AccountNo=GatepassMaster.supid) as SupplierName  FROM [GatepassMaster] where  b_unit = '" + Session["BusinessUnit"] + "' and status=0 ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                gate_list = gate_list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Index()
        {
            //bool b_status = Convert.ToBoolean(Session["Bunit_status"]);
            //var query = "";
            //if (b_status == false)
            //{
            //    query = "AND b_unit = '" + Session["BusinessUnit"] + "' ";
            //}

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
        public ActionResult PurReportView(int ID)
        {
            var list = _context.Database.SqlQuery<PurDetail>("Select * from PurDetails where InvId = " + ID + " and InvType='PINVWOCTN'").ToList();
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
        public ActionResult PINVWOCTNReport(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            PurMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            PurMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            PurMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            PurMaster.GrandTotal = _context.Database.SqlQuery<decimal>("Select GrandTotal from PurMasters where InvID =" + ID + " and InvType='PINVWOCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWOCTN");
            PurMaster.CargoName = NumberToWords(Decimal.ToInt32(PurMaster.GrandTotal));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var supplier = _context.Supplier.SingleOrDefault(c => c.AccountNo == PurMaster.SupplierId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT *,(Select Munit From Products where id=PurDetails.ItemID) as Munit from PurDetails where InvID=" + ID + " and InvType = 'PINVWOCTN' ").ToList();
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
        [HttpPost]
        public ActionResult SearchGatepass(int[] ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            string inv_ids = "";
            for (int i = 0; i < ID.Count(); i++)
            {
                inv_ids = inv_ids + "'" + ID[i] + "'";
                if (i != ID.Count() - 1)
                {
                    inv_ids = inv_ids + ",";
                }
            }
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWOCTN'").FirstOrDefault();
          //  PurMaster.Rtotal = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWOCTN'").FirstOrDefault();
            PurMaster.NetAmount = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWOCTN'").FirstOrDefault();
            PurMaster.SupplierId = Convert.ToInt32(_context.Database.SqlQuery<decimal>("select supid from [GatepassMaster] where invid = "+ID[0]+"").FirstOrDefault());
            PurMaster.Date = _context.Database.SqlQuery<string>("select date from [GatepassMaster] where invid = "+ID[0] + "").FirstOrDefault();
            PurMaster.Address = _context.Database.SqlQuery<string>("select address from [GatepassMaster] where invid = "+ID[0] + "").FirstOrDefault();
            PurMaster.Phone = _context.Database.SqlQuery<string>("select phoneno from [GatepassMaster] where invid = "+ID[0] + "").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var product = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit='" + Bunit + "'").ToList();
            var pro_lists = _context.Database.SqlQuery<GatepassdetailQUery>("Select *,(Select SalePrice From Products where id=hclpvtlt_pos.GatepassDetail.ItemId) as SP,(Select PiecesBox From Products where id=hclpvtlt_pos.GatepassDetail.ItemId) as p_box,(Select Munit From Products where id=hclpvtlt_pos.GatepassDetail.ItemId) as Munit  from hclpvtlt_pos.GatepassDetail where InvId IN (" + inv_ids + ")").ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' ORDER BY InvID DESC").ToList();
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
                pro_lists = pro_lists
            };
            return View("Create",PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult SearchGatepassSave( decimal[] length, int[] inv_id, int[] b_no,string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            
            for (int i = 0; i < item_name.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("Update [GatepassDetail] set status = 1 where id = "+new_id[i]+"");
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetails (party,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType,g_passid) VALUES (" + PurMaster.SupplierId + ",'" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMaster.Date + "',0," + length[i] + ",'PINVWOCTN'," + inv_id[i] + ")");
                _context.Database.ExecuteSqlCommand("Update [GatepassMaster] set status =1 where InvId = "+inv_id[i]+"");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasters (store,b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + PurMaster.Store + ",'" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + PurMaster.DsrId + ",N'" + PurMaster.Address + "','" + PurMaster.Phone + "','" + PurMaster.Date + "',N'" + PurMaster.CargoName + "'," + PurMaster.CargoCharges + "," + PurMaster.NetAmount + "," + PurMaster.DiscountAmount + "," + PurMaster.GrandTotal + "," + PurMaster.Total + "," + PurMaster.Rtotal + "," + PurMaster.BTotal + ",'PINVWOCTN')");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data


            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Supplier'," + PurMaster.SupplierId + ",0,'" + PurMaster.GrandTotal + "'," + PurMaster.InvID + ",'PINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Stock',10000002,'" + PurMaster.GrandTotal + "',0," + PurMaster.InvID + ",'PINVWOCTN')");

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvID =" + ID + " and  InvType = 'PINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where BusinessUnit=" + Bunit + "").ToList();
            PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWOCTN");
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT * from PurDetails where InvID=" + ID + " and InvType = 'PINVWOCTN'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
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
        public ActionResult Update(TransactionDetail TransactionDetail, PurInvoiceVM PurInvoiceVM, int[] inv_id, decimal[] length, int[] b_no, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + PurInvoiceVM.PurMaster.InvID + " and Vtype = 'PINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From PurDetails where InvId =" + PurInvoiceVM.PurMaster.InvID + "  and InvType = 'PINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From PurMasters where InvId =" + PurInvoiceVM.PurMaster.InvID + "  and InvType = 'PINVWOCTN' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                //_context.Database.ExecuteSqlCommand("Update [GatepassDetail] set status = 1 where id = "+new_id[i]+"");
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetails (b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType,g_passid) VALUES ('" + Session["BusinessUnit"] + "'," + PurInvoiceVM.PurMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurInvoiceVM.PurMaster.Date + "',0," + length[i] + ",'PINVWOCTN'," + inv_id[0] + ")");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasters (Store,b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + PurInvoiceVM.PurMaster.Store + ",'" + Session["BusinessUnit"] + "'," + PurInvoiceVM.PurMaster.InvID + "," + PurInvoiceVM.PurMaster.SupplierId + "," + PurInvoiceVM.PurMaster.DsrId + ",N'" + PurInvoiceVM.PurMaster.Address + "','" + PurInvoiceVM.PurMaster.Phone + "','" + PurInvoiceVM.PurMaster.Date + "',N'" + PurInvoiceVM.PurMaster.CargoName + "'," + PurInvoiceVM.PurMaster.CargoCharges + "," + PurInvoiceVM.PurMaster.NetAmount + "," + PurInvoiceVM.PurMaster.DiscountAmount + "," + PurInvoiceVM.PurMaster.GrandTotal + "," + PurInvoiceVM.PurMaster.Total + "," + PurInvoiceVM.PurMaster.Rtotal + "," + PurInvoiceVM.PurMaster.BTotal + ",'PINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.PurMaster.Date + "','Supplier'," + PurInvoiceVM.PurMaster.SupplierId + ",0,'" + PurInvoiceVM.PurMaster.GrandTotal + "'," + PurInvoiceVM.PurMaster.InvID + ",'PINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurInvoiceVM.PurMaster.Date + "','Stock',10000002,'" + PurInvoiceVM.PurMaster.GrandTotal + "',0," + PurInvoiceVM.PurMaster.InvID + ",'PINVWOCTN')");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'PINV'");
            _context.Database.ExecuteSqlCommand("Delete From PurDetails where InvId =" + ID + " and InvType = 'PINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From PurMasters where InvId =" + ID + " and InvType = 'PINVWOCTN' ");
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