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
    public class SaleReturnController : Controller
    {
        private ApplicationDbContext _context;
        public SaleReturnController()
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
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            //var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=Salemasters.CustomerId) as CustomerName ,(Select name From Employees where id=DeliveryNoteMaster.DsrId) as EmployeeName FROM DeliveryNoteMaster where status =1 ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult SaleReturnList()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasterReturns.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasterReturns.DsrId) as EmployeeName FROM SaleMasterReturns where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            //var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=Salemasters.CustomerId) as CustomerName ,(Select name From Employees where id=DeliveryNoteMaster.DsrId) as EmployeeName FROM DeliveryNoteMaster where status =1 ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult SaleReportSS(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWOCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
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
        public ActionResult Create(int inv_id, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == inv_id && c.InvType == "SINVWOCTN");
            SaleMaster.sInvID = inv_id;
            SaleMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from [SaleMasterReturns] where InvType = 'SINVWOCTN'").FirstOrDefault();
            //SaleMaster.CustomerId = _context.Database.SqlQuery<int>("select CustomerId from SaleMasters where InvType = 'SINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var cus_list = _context.Customer.Where(z=>z.AccountNo==SaleMaster.CustomerId).ToList();
            var pro_list_s = _context.Database.SqlQuery<stockquery>("SELECT ID,((OpeningStock + QTYIN) - QTYOUT) AS BAL, Name, SalePrice , PiecesBox FROM (SELECT Products.ID, Products.Name, Products.SalePrice,Products.Munit, Products.Barcode, Products.PiecesBox, Products.OpeningStock, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM Production_finished WHERE(p_id = Products.ID)) AS QTYIN, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM [SaleDetailReturns] WHERE(itemid = Products.ID )) AS QTYOUT FROM Products WHERE type=2 and businessunit='"+Bunit+"') AS derivedtbl_1").ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=[SaleMasterReturns].CustomerId) as CustomerName ,(Select name From Employees where AccountNo=[SaleMasterReturns].DsrId) as EmployeeName FROM [SaleMasterReturns] where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var SaleInvVM = new SaleInvVM
            {

                Store = Store,
                SaleDetail = SaleDetail,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                sale_list_woc = sale_list_woc,
                pro_list_s = pro_list_s
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult SaleReturnSave(int[] inv_id, string[] d_no, string[] t_drum, string[] e_drum, string[] idnit, string[] packing, decimal[] p_box, string[] item_name, int[] id, decimal[] sp, int[] qty, decimal[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where Rinvid =" + SaleMaster.sInvID + " and Vtype = 'SINV'");
            _context.Database.ExecuteSqlCommand("Delete From [SaleDetailReturns] where sInvID =" + SaleMaster.sInvID + "  and InvType = 'SINVWOCTN' ");
            _context.Database.ExecuteSqlCommand("Delete From [SaleMasterReturns] where sInvID =" + SaleMaster.sInvID + "  and InvType = 'SINVWOCTN' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [SaleDetailReturns] (sInvID,d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" + SaleMaster.sInvID + "," +
                    "'" + d_no[i] + "','" + t_drum[i] + "','" + e_drum[i] + "','" + idnit[i] + "','" + packing[i] + "','" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMaster.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
                 }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO [SaleMasterReturns] (sInvID,Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + SaleMaster.sInvID + "," + SaleMaster.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + SaleMaster.Austrialan + "," + SaleMaster.Canadian + ",'" + Session["BusinessUnit"] + "'," + SaleMaster.InvID + "," + SaleMaster.CustomerId + ",1,N'" + SaleMaster.Address + "','" + SaleMaster.Phone + "','" + SaleMaster.Date + "',N'" + SaleMaster.CargoName + "'," + SaleMaster.CargoCharges + "," + SaleMaster.NetAmount + "," + SaleMaster.DiscountAmount + "," + SaleMaster.GrandTotal + "," + SaleMaster.Total + "," + SaleMaster.Rtotal + "," + SaleMaster.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + SaleMaster.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Customer'," + SaleMaster.CustomerId + ",'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + SaleMaster.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Sale',4400001,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + SaleMaster.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','CGS',5500001,'" + SaleMaster.GrandTotal + "',0," + SaleMaster.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Cr,Dr,InvId,Vtype) VALUES (" + SaleMaster.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + SaleMaster.Date + "','Stock',10000002,0,'" + SaleMaster.GrandTotal + "'," + SaleMaster.InvID + ",'SRINV')");

            return RedirectToAction("Index");
        }
        public ActionResult SINVWOCTNReturnReport(int? ID, int? value, SaleMaster SaleMaster, SaleMasterReturn saleMasterReturn, TransactionDetail TransactionDetail)
        {

            saleMasterReturn.InvID = _context.Database.SqlQuery<int>("Select InvID from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            saleMasterReturn.CargoCharges = _context.Database.SqlQuery<decimal>("Select CargoCharges from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            saleMasterReturn.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            saleMasterReturn = _context.Database.SqlQuery<SaleMasterReturn>("Select * from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").SingleOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            if (value == 1)
            {
                saleMasterReturn.NetAmount = saleMasterReturn.NetAmount;
            }
            else if (value == 2)
            {
                saleMasterReturn.NetAmount = saleMasterReturn.NetAmount / saleMasterReturn.US;
            }
            else if (value == 3)
            {
                saleMasterReturn.NetAmount = saleMasterReturn.NetAmount / saleMasterReturn.Euro;
            }
            else if (value == 4)
            {
                saleMasterReturn.NetAmount = saleMasterReturn.NetAmount / saleMasterReturn.Austrialan;
            }
            else if (value == 5)
            {
                saleMasterReturn.NetAmount = saleMasterReturn.NetAmount / saleMasterReturn.Canadian;
            }

            saleMasterReturn.CargoName = NumberToWords(Decimal.ToInt32(saleMasterReturn.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var customer = _context.Customer.SingleOrDefault(c => c.AccountNo == saleMasterReturn.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=[SaleDetailReturns].ItemID) as Munit  from [SaleDetailReturns] where InvID=" + ID + " and InvType = 'SINVWOCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                saleMasterReturn = saleMasterReturn,
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
        public ActionResult Edit(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail, SaleMasterReturn saleMasterReturn)
        {
            saleMasterReturn.InvID = _context.Database.SqlQuery<int>("Select InvID from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where type=2 and BusinessUnit='" + Bunit + "'").ToList();
            saleMasterReturn = _context.Database.SqlQuery<SaleMasterReturn>("Select * from [SaleMasterReturns] where InvID =" + ID + " and InvType='SINVWOCTN'").SingleOrDefault();
            var cus_list = _context.Customer.Where(z => z.AccountNo == saleMasterReturn.CustomerId).ToList();
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT * from [SaleDetailReturns] where InvID=" + ID + " and InvType = 'SINVWOCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                saleMasterReturn = saleMasterReturn,
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
        public ActionResult Update(SaleMasterReturn saleMasterReturn,string[] d_no, string[] t_drum, string[] e_drum, string[] idnit, string[] packing, decimal[] p_box, string[] item_name, int[] id, decimal[] sp, int[] qty, decimal[] n_total, SaleMaster SaleMaster, SaleDetail SaleDetail, TransactionDetail TransactionDetail)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + saleMasterReturn.InvID + " and Vtype='SINV'");
            _context.Database.ExecuteSqlCommand("Delete From [SaleMasterReturns] where InvId =" + saleMasterReturn.InvID + " and InvType='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From [SaleDetailReturns] where InvId =" + saleMasterReturn.InvID + " and InvType= 'SINVWOCTN'");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [SaleDetailReturns] (sInvID,d_no,t_drum,e_drum,idnit,packing,b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,InvType) VALUES (" + saleMasterReturn.sInvID + "," +
                    "'" + d_no[i] + "','" + t_drum[i] + "','" + e_drum[i] + "','" + idnit[i] + "','" + packing[i] + "','" + Session["BusinessUnit"] + "'," + saleMasterReturn.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + saleMasterReturn.Date + "',0," + p_box[i] + ",'SINVWOCTN')");
            }
            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO [SaleMasterReturns] (sInvID,Store,US,Euro,Austrialan,Canadian,b_unit,InvID,CustomerId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType) VALUES (" + saleMasterReturn.sInvID + "," + saleMasterReturn.Store + "," + SaleMaster.US + "," + SaleMaster.Euro + "," + saleMasterReturn.Austrialan + "," + saleMasterReturn.Canadian + ",'" + Session["BusinessUnit"] + "'," + saleMasterReturn.InvID + "," + saleMasterReturn.CustomerId + ",1,N'" + saleMasterReturn.Address + "','" + saleMasterReturn.Phone + "','" + saleMasterReturn.Date + "',N'" + saleMasterReturn.CargoName + "'," + saleMasterReturn.CargoCharges + "," + saleMasterReturn.NetAmount + "," + saleMasterReturn.DiscountAmount + "," + saleMasterReturn.GrandTotal + "," + saleMasterReturn.Total + "," + saleMasterReturn.Rtotal + "," + saleMasterReturn.BTotal + ",'SINVWOCTN')");
            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + saleMasterReturn.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMasterReturn.Date + "','Customer'," + saleMasterReturn.CustomerId + ",'" + saleMasterReturn.GrandTotal + "',0," + saleMasterReturn.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + saleMasterReturn.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMasterReturn.Date + "','Sale',4400001,0,'" + saleMasterReturn.GrandTotal + "'," + saleMasterReturn.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + saleMasterReturn.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMasterReturn.Date + "','CGS',5500001,'" + saleMasterReturn.GrandTotal + "',0," + saleMasterReturn.InvID + ",'SRINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (Rinvid,b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + saleMasterReturn.sInvID + ",'" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + saleMasterReturn.Date + "','Stock',10000002,0,'" + saleMasterReturn.GrandTotal + "'," + saleMasterReturn.InvID + ",'SISRINVNV')");

            return RedirectToAction("SaleReturnList");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype='SINV'");
            _context.Database.ExecuteSqlCommand("Delete From [SaleMasterReturns] where InvId =" + ID + " and InvType='SINVWOCTN'");
            _context.Database.ExecuteSqlCommand("Delete From [SaleDetailReturns] where InvId =" + ID + " and InvType= 'SINVWOCTN'");
            return RedirectToAction("SaleReturnList");
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