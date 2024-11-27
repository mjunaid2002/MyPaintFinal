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
    public class SaleController : Controller
    {
        private ApplicationDbContext _context;
        public SaleController()
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
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=DeliveryNoteMaster.CustomerId) as CustomerName ,(Select name From Employees where id=DeliveryNoteMaster.DsrId) as EmployeeName FROM DeliveryNoteMaster where status =1 and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult SaleReportSS(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWOCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptSinvss.rpt"));

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
            //rd.SetParameterValue("cusemail", cusemail);
            //rd.SetParameterValue("cusphone", cusphone);
            //rd.SetParameterValue("cusaddress", cusaddress);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptSinvss.pdf");
        }
        public ActionResult SaleReportView(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWOCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();
            
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptSaleInv.rpt"));

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
            rd.SetParameterValue("image", Image);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptSaleInv.pdf");
        }
        [HttpPost]
        public ActionResult SaleSearch(int[] ID, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
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
            SaleMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasters where InvType = 'SINVWOCTN'").FirstOrDefault();
            SaleMaster.CustomerId = _context.Database.SqlQuery<int>("select CustomerId from DeliveryNoteMaster where invid = " + ID[0] + "").FirstOrDefault();
            SaleMaster.DsrId = _context.Database.SqlQuery<int>("select DsrId from DeliveryNoteMaster where invid = " + ID[0] + "").FirstOrDefault();
            SaleMaster.Address = _context.Database.SqlQuery<string>("select address from DeliveryNoteMaster where invid = " + ID[0] + "").FirstOrDefault();
            SaleMaster.Phone = _context.Database.SqlQuery<string>("select Phone from DeliveryNoteMaster where invid = " + ID[0] + "").FirstOrDefault();
            SaleMaster.CargoName = _context.Database.SqlQuery<string>("select CargoName from DeliveryNoteMaster where invid = " + ID[0] + "").FirstOrDefault();

            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where type=2 and BusinessUnit='" + Bunit + "'").ToList();
            var delivery_list = _context.Database.SqlQuery<SaleDetailQuery>("Select  (Select PiecesBox From Products where id=DeliveryNoteDetail.ItemId) as PiecesBox,*,(Select SalePrice From Products where id=DeliveryNoteDetail.ItemId) as SP1 from DeliveryNoteDetail where InvId IN (" + inv_ids + ")").ToList();
            var pro_list_s = _context.Database.SqlQuery<stockquery>("SELECT ID,((OpeningStock + QTYIN) - QTYOUT) AS BAL, Name, SalePrice , PiecesBox FROM (SELECT Products.ID, Products.Name, Products.SalePrice,Products.Munit, Products.Barcode, Products.PiecesBox, Products.OpeningStock, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM Production_finished WHERE(p_id = Products.ID)) AS QTYIN, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM SaleDetails WHERE(itemid = Products.ID )) AS QTYOUT FROM Products WHERE type=2 and BusinessUnit='" + Bunit + "') AS derivedtbl_1").ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='"+ Bunit + "' ORDER BY InvID DESC").ToList();
            var SaleInvVM = new SaleInvVM
            {

                Store = Store,
                delivery_list = delivery_list,
                SaleDetail = SaleDetail,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                sale_list_woc = sale_list_woc,
                pro_list_s = pro_list_s
            };
            return View("Create",SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(int[] inv_id, string[] d_no, string[] t_drum, string[] e_drum, string[] idnit, string[] packing, decimal[] p_box, string[] item_name, int[] id, decimal[] sp, int[] qty, decimal[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails (d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" +
                    "'"+ d_no[i]+"','"+ t_drum[i]+"','"+ e_drum[i]+"','"+ idnit[i]+"','"+ packing[i]+"','" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
                _context.Database.ExecuteSqlCommand("Update DeliveryNoteMaster set status =2 where InvId = " + inv_id[i] + "");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.Store + "," + SaleMaster.US+","+ SaleMaster.Euro + ","+ SaleMaster.Austrialan + ","+ SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Customer'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Sale',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','CGS',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Stock',10000002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");

            return RedirectToAction("Index");
        }
        public ActionResult SINVWOCTNReport(int? ID,int? value, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            SaleMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("Select DiscountAmount from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN");

            if (value == 1)
            {
                SaleMaster.NetAmount = SaleMaster.NetAmount;
            }
            else if(value == 2)
            {
                SaleMaster.NetAmount = SaleMaster.NetAmount / SaleMaster.US;
            }
            else if (value == 3)
            {
                SaleMaster.NetAmount = SaleMaster.NetAmount / SaleMaster.Euro;
            }
            else if (value == 4)
            {
                SaleMaster.NetAmount = SaleMaster.NetAmount / SaleMaster.Austrialan;
            }
            else if (value == 5)
            {
                SaleMaster.NetAmount = SaleMaster.NetAmount / SaleMaster.Canadian;
            }

            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == SaleMaster.CustomerId );
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
            ViewBag.Value = value;
            return View(SaleInvVM);
        }
        public ActionResult Edit(int? ID,SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from SaleMasters where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var cus_list = _context.Customer.Where(z => z.BusinessUnit == Bunit).ToList();
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where type=2 and BusinessUnit='" + Bunit + "'").ToList();

            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "SINVWOCTN");
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT * from SaleDetails where InvID=" + ID + " and InvType = 'SINVWOCTN'").ToList();
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
        public ActionResult Update(string[] d_no, string[] t_drum, string[] e_drum, string[] idnit, string[] packing, decimal[] p_box, string[] item_name, int[] id, decimal[] sp, int[] qty, decimal[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + SaleMaster.InvID + " and Vtype='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + SaleMaster.InvID + " and InvType='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetails where InvId =" + SaleMaster.InvID + " and InvType= 'SINVWOCTN'");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetails ,(d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" +
                    "'" + d_no[i] + "','" + t_drum[i] + "','" + e_drum[i] + "','" + idnit[i] + "','" + packing[i] + "','" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasters (Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + SaleMaster.Austrialan + "," + SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Customer'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Sale',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','CGS',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SINVWOCTN')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Stock',10000002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SINVWOCTN')");
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
    }
}