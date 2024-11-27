using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using CRM.Models;
using System.Collections.Generic;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ReportsssssController : Controller
    {
        private ApplicationDbContext _context;
        private List<stockquery> list;

        public ReportsssssController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Reports
        public ActionResult GatePass()
        {
            var gate_list = _context.Database.SqlQuery<GatepassQuery>("SELECT *,(Select name From Suppliers where AccountNo=GatepassMaster.supid) as SupplierName  FROM [GatepassMaster] where  b_unit = '" + Session["BusinessUnit"] + "' ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                gate_list = gate_list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Purchase()
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
        public ActionResult Production()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT   NetAmount,net_final as BTotal,inv_id as InvID,s_date as Date,e_date as Phone,batch_no as Rtotal, labour_cost as CargoCharges,other_cost as DiscountAmount,total_cost as Total,gorss_total as GrandTotal from [ProductionMaster_new] where b_unit ='" + Bunit + "'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Sale()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult StockIndex()
        {
            string b_unit = Convert.ToString(Session["BusinessUnit"]);
            var cat_list = _context.Category.Where(c => c.b_unit == b_unit).ToList();
            return View(cat_list);
        }
        [HttpPost]
        public ActionResult StockReport(string type)
        {
            string s_date = Request["s_date"];
            //string storeid = Request["storeid"];
            string b_unit = Convert.ToString(Session["BusinessUnit"]);

            if (type == "")
            {
                 list = _context.Database.SqlQuery<stockquery>("SELECT OpeningStock,Products.Name AS item_name,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID = Products.id) and (date < '" + s_date + "') and b_unit = '" + b_unit + "' ) AS openingnopur,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and (date < '" + s_date + "') and b_unit = '" + b_unit + "') AS openingno,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and (date = '" + s_date + "') and b_unit = '" + b_unit + "') AS QTYOUT,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID =Products.id) and (date = '" + s_date + "') and b_unit = '" + b_unit + "') AS QTYIN from Products where BusinessUnit = '" + b_unit + "'").ToList();
            }
            else
            {
                 list = _context.Database.SqlQuery<stockquery>("SELECT OpeningStock,Products.Name AS item_name,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID = Products.id) and (date < '" + s_date + "') and b_unit = '" + b_unit + "' ) AS openingnopur,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and (date < '" + s_date + "') and b_unit = '" + b_unit + "') AS openingno,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and (date = '" + s_date + "') and b_unit = '" + b_unit + "') AS QTYOUT,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID =Products.id) and (date = '" + s_date + "') and b_unit = '" + b_unit + "') AS QTYIN from Products where BrandID='" + type + "' and BusinessUnit = '" + b_unit + "'").ToList();
            }
            ViewBag.StartDate = s_date;
            ViewBag.type = type;
            return View(list);

        }
        public ActionResult DailyRawStock()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RawStockReport()
        {
            string s_date = Request["s_date"];
            var list = _context.Database.SqlQuery<stockquery>("SELECT ItemID, ItemName, Name, AccountNo, OpeningBalNoGatePass + OpeningBalNoPur AS openingno, OpeningBalHnksGatePass + OpeningBalHnkspur AS openinghanks, BalNoGatePass AS stockinno, BalHnksGatePass AS stockinhanks, BalNopur AS stockoutno, BalHnkspur AS stockouthanks, OpeningBalNoGatePass + BalNoGatePass - OpeningBalNoPur - BalNopur AS balanceno, OpeningBalHnksGatePass + BalHnksGatePass - OpeningBalHnkspur - BalHnkspur AS balancehanks FROM(SELECT MUnit, ItemID, ItemName, Name, AccountNo, (SELECT ISNULL(SUM(GatepassDetail.Qty), 0) AS Expr1 FROM GatepassDetail INNER JOIN dbo.Products ON GatepassDetail.ItemID = dbo.Products.ID INNER JOIN GatepassMaster ON GatepassDetail.InvId = GatepassMaster.invid WHERE (dbo.Products.type = 1) AND (dbo.Products.MUnit = 'No') AND (GatepassDetail.ItemID = dbf.ItemID) AND (GatepassMaster.supid = dbf.AccountNo) AND (GatepassDetail.date < '" + s_date + "')) AS OpeningBalNoGatePass, (SELECT ISNULL(SUM(GatepassDetail_2.Qty), 0) AS Expr1 FROM GatepassDetail AS GatepassDetail_2 INNER JOIN dbo.Products AS Products_2 ON GatepassDetail_2.ItemID = Products_2.ID INNER JOIN GatepassMaster AS GatepassMaster_2 ON GatepassDetail_2.InvId = GatepassMaster_2.invid WHERE (Products_2.type = 1) AND (Products_2.MUnit <> 'No') AND (GatepassDetail_2.ItemID = dbf.ItemID) AND (GatepassMaster_2.supid = dbf.AccountNo) AND (GatepassDetail_2.date < '" + s_date + "')) AS OpeningBalHnksGatePass, (SELECT ISNULL(SUM(GatepassDetail_3.Qty), 0) AS Expr1 FROM GatepassDetail AS GatepassDetail_3 INNER JOIN dbo.Products AS Products_3 ON GatepassDetail_3.ItemID = Products_3.ID INNER JOIN GatepassMaster AS GatepassMaster_3 ON GatepassDetail_3.InvId = GatepassMaster_3.invid WHERE (Products_3.type = 1) AND (Products_3.MUnit = 'No') AND (GatepassDetail_3.ItemID = dbf.ItemID) AND (GatepassMaster_3.supid = dbf.AccountNo) AND (GatepassDetail_3.date = '" + s_date + "')) AS BalNoGatePass, (SELECT ISNULL(SUM(GatepassDetail_2.Qty), 0) AS Expr1 FROM GatepassDetail AS GatepassDetail_2 INNER JOIN dbo.Products AS Products_2 ON GatepassDetail_2.ItemID = Products_2.ID INNER JOIN GatepassMaster AS GatepassMaster_2 ON GatepassDetail_2.InvId = GatepassMaster_2.invid WHERE (Products_2.type = 1) AND (Products_2.MUnit <> 'No') AND (GatepassDetail_2.ItemID = dbf.ItemID) AND (GatepassMaster_2.supid = dbf.AccountNo) AND (GatepassDetail_2.date = '" + s_date + "')) AS BalHnksGatePass, (SELECT ISNULL(SUM(ProductionDetail_new.qty), 0) AS Expr1 FROM ProductionDetail_new INNER JOIN dbo.Products AS Products_4 ON ProductionDetail_new.pid = Products_4.ID INNER JOIN ProductionMaster_new ON ProductionDetail_new.inv_id = ProductionMaster_new.inv_id WHERE (Products_4.type = 1) AND (Products_4.MUnit = 'No') AND (ProductionDetail_new.pid = dbf.ItemID) AND (ProductionDetail_new.party = dbf.AccountNo) AND (ProductionDetail_new.date < '" + s_date + "')) AS OpeningBalNoPur, (SELECT ISNULL(SUM(ProductionDetail_new_1.qty), 0) AS Expr1 FROM ProductionDetail_new AS ProductionDetail_new_1 INNER JOIN dbo.Products AS Products_5 ON ProductionDetail_new_1.pid = Products_5.ID INNER JOIN ProductionMaster_new AS ProductionMaster_new_1 ON ProductionDetail_new_1.inv_id = ProductionMaster_new_1.inv_id WHERE (Products_5.type = 1) AND (Products_5.MUnit <> 'No') AND (ProductionDetail_new_1.pid = dbf.ItemID) AND (ProductionDetail_new_1.party= dbf.AccountNo) AND (ProductionDetail_new_1.date < '" + s_date + "')) AS OpeningBalHnkspur, (SELECT ISNULL(SUM(ProductionDetail_new_3.qty), 0) AS Expr1 FROM ProductionDetail_new AS ProductionDetail_new_3 INNER JOIN dbo.Products AS Products_7 ON ProductionDetail_new_3.pid = Products_7.ID INNER JOIN ProductionMaster_new AS ProductionMaster_new_3 ON ProductionDetail_new_3.inv_id = ProductionMaster_new_3.inv_id WHERE (Products_7.type = 1) AND (Products_7.MUnit = 'No') AND (ProductionDetail_new_3.pid = dbf.ItemID) AND (ProductionDetail_new_3.party= dbf.AccountNo) AND (ProductionDetail_new_3.date = '" + s_date + "')) AS BalNopur, (SELECT ISNULL(SUM(ProductionDetail_new_2.qty), 0) AS Expr1 FROM ProductionDetail_new AS ProductionDetail_new_2 INNER JOIN dbo.Products AS Products_6 ON ProductionDetail_new_2.pid = Products_6.ID INNER JOIN ProductionMaster_new AS ProductionMaster_new_2 ON ProductionDetail_new_2.inv_id = ProductionMaster_new_2.inv_id WHERE (Products_6.type = 1) AND (Products_6.MUnit <> 'No') AND (ProductionDetail_new_2.pid = dbf.ItemID) AND (ProductionDetail_new_2.party = dbf.AccountNo) AND (ProductionDetail_new_2.date = '" + s_date + "')) AS BalHnkspur FROM (SELECT Products_1.MUnit, GatepassDetail_1.ItemID, GatepassDetail_1.ItemName, Suppliers_1.Name, Suppliers_1.AccountNo FROM GatepassDetail AS GatepassDetail_1 INNER JOIN GatepassMaster AS GatepassMaster_1 ON GatepassDetail_1.InvId = GatepassMaster_1.invid INNER JOIN dbo.Suppliers AS Suppliers_1 ON GatepassMaster_1.supid = Suppliers_1.AccountNo INNER JOIN dbo.Products AS Products_1 ON GatepassDetail_1.ItemID = Products_1.ID WHERE (Products_1.type = 1)) AS dbf) AS derivedtbl_1").ToList();
            ViewBag.StartDate = s_date;

            return View(list);
        }
        public ActionResult FinshedStock()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FinshedStockReport()
        {
            string s_date = Request["s_date"];
            var list = _context.Database.SqlQuery<stockquery>("Select Name, Isnull((select SUM(qty) from Production_finished_new where p_id=products.id and date<'" + s_date + "')-(select SUM(qty) from SaleDetails where itemid=products.id and date<'" + s_date + "'),0) as ope, Isnull((select SUM(qty) from Production_finished_new where p_id=products.id and date='" + s_date + "'),0) as pro, Isnull((select SUM(qty) from SaleDetails where itemid=products.id and date='" + s_date + "'),0) as sale,Isnull((select SUM(qty) from [SaleDetailReturns] where itemid=products.id and date='" + s_date + "'),0) as saleRtn,Brandid from products").ToList();
            ViewBag.StartDate = s_date;
            var cat_list = _context.Category.ToList();
            var ProductVM = new ProductVM
            {
                list = list,
                cat_list = cat_list,
            };
            return View(ProductVM);
        }
        //Cheque Reporrtss
        public ActionResult ChequesReport()
        {
            var vou_list = _context.Database.SqlQuery<Voucher>("SELECT * FROM Vouchers where VType = 'JV' OR VType = 'BPV' OR VType = 'BRV' ORDER BY TID DESC").ToList();
            return View(vou_list);
        }
        [HttpPost]
        public ActionResult ChequeUpdate(string Cheque_status, string id)
        {
            _context.Database.ExecuteSqlCommand("UPDATE Vouchers set Cheque_status='" + Cheque_status + "' where ID = " + id + "");
            return RedirectToAction("ChequesReport");
        }
        public ActionResult SaleReportIndex()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' " + query + " ORDER BY InvID DESC").ToList();
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select Top(1) name From Customers where AccountNo=SaleMasters.CustomerId and BusinessUnit='"+Bunit+"') as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvType = 'SINVWOCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                list = list
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult SaleSearch()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            //var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' " + query + " ORDER BY InvID DESC").ToList();
            var list = _context.Database.SqlQuery<Salequery>("SELECT *,(Select Top(1) name From Customers where AccountNo=SaleMasters.CustomerId and BusinessUnit='" + Bunit + "') as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "' and date between '"+Request["s_date"] +"' and '"+Request["e_date"] +"' ORDER BY InvID DESC").ToList();
            var s_detail = _context.Database.SqlQuery<SaleDetailqueryss>("SELECT *,(Select Munit From Products where id=SaleDetails.ItemID) as Munit  from SaleDetails where InvType = 'SINVWOCTN'").ToList();
            var SaleInvVM = new SaleInvVM
            {
                s_detail = s_detail,
                list = list
            };
            return View("SaleReportIndex", SaleInvVM);
        }
        public ActionResult PurchaseIndex()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select Top(1) name From Suppliers where AccountNo=PurMasters.SupplierId  and BusinessUnit='" + Bunit + "') as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' and b_unit ='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT *,(Select Munit From Products where id=PurDetails.ItemID) as Munit from PurDetails where  InvType = 'PINVWOCTN' ").ToList();

            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                p_detail = p_detail,
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult PurchaseSearch()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select Top(1) name From Suppliers where AccountNo=PurMasters.SupplierId  and BusinessUnit='" + Bunit + "') as SupplierName ,(Select TOP(1) name From Employees where AccountNo=PurMasters.DsrId and BusinessUnit='" + Bunit + "') as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' and b_unit ='" + Bunit + "' and date between '" + Request["s_date"] + "' and '" + Request["e_date"] + "' ORDER BY InvID DESC").ToList();
            var p_detail = _context.Database.SqlQuery<PurDetailqueryss>("SELECT *,(Select Munit From Products where id=PurDetails.ItemID) as Munit from PurDetails where  InvType = 'PINVWOCTN' ").ToList();

            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                p_detail = p_detail,
            };
            return View("PurchaseIndex", PurInvoiceVM);
        }
    }
}