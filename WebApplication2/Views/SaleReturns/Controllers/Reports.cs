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
    public class ReportsController : Controller
    {
        private ApplicationDbContext _context;
        private List<PurchaseReportQuery> list;

        public ReportsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: SaleWctn
        public ActionResult RawMaterialIndex()
        {
            return View();
        }
      
        public ActionResult RawMaterialSearch()
        {
            //var list = _context.Database.SqlQuery<RawMaterialReportQuery>("SELECT CategoryID, CategoryName, ProductID, pname, vattax, cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, Qtybal, total as ext1 FROM(SELECT TOP (100) PERCENT CategoryID, (SELECT CategoryName FROM Categories WHERE (CategoryID = derivedtbl_2.CategoryID)) AS CategoryName, ProductID, ProductName AS pname, vattax, UnitPrice AS cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast AS Qtybal, (AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast) * UnitPrice AS total FROM (SELECT CategoryID, ProductID, ProductName, vattax, UnitPrice, vattax + PurchaseOpenIn + labOpenIn + SalesReturnOpenIn - purchasereturnOpenOut - LabStkOpenout - LabStkOpenout2 - labOPout - SalesOpenOut - DmgStkOpenOut AS AdjQTY, PurchasebetweenIn AS QTYin, labBetweenIn AS QTYLabin, SalesReturnBetweenIn AS QTYrtnIn, purchasereturnBetweenOut AS qtyoutprd, LabStkBetwout + LabStkBetwout2 AS qtyout, labBTout AS qtyoutlab, SalesBetwOut AS QTYSaleOut, DmgStkBetwOut AS qtyoutwast, PurchaseOpenIn, PurchasebetweenIn, labOpenIn, labBetweenIn, SalesReturnOpenIn, SalesReturnBetweenIn, purchasereturnOpenOut, purchasereturnBetweenOut, LabStkOpenout, LabStkBetwout, LabStkOpenout2, LabStkBetwout2, labOPout, labBTout, SalesOpenOut, SalesBetwOut, DmgStkOpenOut, DmgStkBetwOut FROM (SELECT CAST(CategoryID AS decimal) AS CategoryID, CAST(ProductID AS decimal) AS ProductID, CAST(ProductName AS varchar(500)) AS ProductName, CAST(vattax AS float) AS vattax, CAST(UnitPrice AS float) AS UnitPrice, (SELECT ISNULL(SUM(purchasedetail.qty), 0) AS Expr1 FROM purchasedetail INNER JOIN purchasem ON purchasedetail.invid = purchasem.invid WHERE (purchasedetail.pid = PRD.ProductID) AND (purchasem.date < '" + Request["s_date"] + "')) AS PurchaseOpenIn, (SELECT ISNULL(SUM(purchasedetail_1.qty), 0) AS Expr1 FROM purchasedetail AS purchasedetail_1 INNER JOIN purchasem AS purchasem_1 ON purchasedetail_1.invid = purchasem_1.invid WHERE (purchasedetail_1.pid = PRD.ProductID) AND (purchasem_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS PurchasebetweenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm WHERE (pid = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOpenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm AS labm_1 WHERE (pid = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBetweenIn, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID WHERE (srsdetail.prid = PRD.ProductID) AND (srsm.date < '" + Request["s_date"] + "')) AS SalesReturnOpenIn, (SELECT ISNULL(SUM(srsdetail_1.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 INNER JOIN srsm AS srsm_1 ON srsdetail_1.OrderID = srsm_1.OrderID WHERE (srsdetail_1.prid = PRD.ProductID) AND (srsm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesReturnBetweenIn, (SELECT ISNULL(SUM(srpdetail.qty), 0) AS Expr1 FROM srpdetail INNER JOIN srpm ON srpdetail.invid = srpm.invid WHERE (srpdetail.pid = PRD.ProductID) AND (srpm.date < '" + Request["s_date"] + "')) AS purchasereturnOpenOut, (SELECT ISNULL(SUM(srpdetail_1.qty), 0) AS Expr1 FROM srpdetail AS srpdetail_1 INNER JOIN srpm AS srpm_1 ON srpdetail_1.invid = srpm_1.invid WHERE (srpdetail_1.pid = PRD.ProductID) AND (srpm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS purchasereturnBetweenOut, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail AS ProductIngrDetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout2, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 AS ProductIngrDetail1_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout2, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOPout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail AS labdetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBTout, (SELECT ISNULL(SUM(Product.prdunit * Order_detail.ctn + Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID INNER JOIN Product ON Order_detail.prid = Product.ProductID WHERE (Order_detail.prid = PRD.ProductID) AND (Order_Master.date < '" + Request["s_date"] + "')) AS SalesOpenOut, (SELECT ISNULL(SUM(Product_1.prdunit * Order_detail_1.ctn + Order_detail_1.qty), 0) AS Expr1 FROM Order_detail AS Order_detail_1 INNER JOIN Order_Master AS Order_Master_1 ON Order_detail_1.OrderID = Order_Master_1.OrderID INNER JOIN Product AS Product_1 ON Order_detail_1.prid = Product_1.ProductID WHERE (Order_detail_1.prid = PRD.ProductID) AND (Order_Master_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesBetwOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = PRD.ProductID) AND (date < '" + Request["s_date"] + "')) AS DmgStkOpenOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail AS sampledetail_1 WHERE (pid = PRD.ProductID) AND (date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS DmgStkBetwOut FROM Product AS PRD WHERE (CategoryID IN (1, 38, 43)) AND (LEN(ProductName) > 3)) AS derivedtbl_1) AS derivedtbl_2 ORDER BY CategoryID, pname) AS derivedtbl_3 ORDER BY pname").ToList();
            int opt = Convert.ToInt32(Request["opt"]);
            string sel_opt = "";
            if (opt == 0)
            {
                sel_opt = "1, 38, 43";
            }
            else if (opt == 1)
            {
                sel_opt = "1";
            }
            else if (opt == 38)
            {
                sel_opt = "38";
            }
            else if (opt == 43)
            {
                sel_opt = "43";
            }
            
           // if(opt != 0)
           // {
           //     var list = _context.Database.SqlQuery<RawMaterialReportQuery>("SELECT CategoryID, CategoryName, ProductID, pname, vattax,cost, AdjQTY, QTYin,QTYLabin, QTYrtnIn, qtyoutprd, qtyout , qtyoutlab, QTYSaleOut, qtyoutwast, total as ext1 FROM(SELECT TOP (100) PERCENT CategoryID, (SELECT CategoryName FROM Categories WHERE (CategoryID = derivedtbl_2.CategoryID)) AS CategoryName, ProductID, ProductName AS pname, vattax, UnitPrice AS cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast AS Qtybal, (AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast) * UnitPrice AS total FROM (SELECT CategoryID, ProductID, ProductName, vattax, UnitPrice, vattax + PurchaseOpenIn + labOpenIn + SalesReturnOpenIn - purchasereturnOpenOut - LabStkOpenout - LabStkOpenout2 - labOPout - SalesOpenOut - DmgStkOpenOut AS AdjQTY, PurchasebetweenIn AS QTYin, labBetweenIn AS QTYLabin, SalesReturnBetweenIn AS QTYrtnIn, purchasereturnBetweenOut AS qtyoutprd, LabStkBetwout + LabStkBetwout2 AS qtyout, labBTout AS qtyoutlab, SalesBetwOut AS QTYSaleOut, DmgStkBetwOut AS qtyoutwast, PurchaseOpenIn, PurchasebetweenIn, labOpenIn, labBetweenIn, SalesReturnOpenIn, SalesReturnBetweenIn, purchasereturnOpenOut, purchasereturnBetweenOut, LabStkOpenout, LabStkBetwout, LabStkOpenout2, LabStkBetwout2, labOPout, labBTout, SalesOpenOut, SalesBetwOut, DmgStkOpenOut, DmgStkBetwOut FROM (SELECT CAST(CategoryID AS decimal) AS CategoryID, CAST(ProductID AS decimal) AS ProductID, CAST(ProductName AS varchar(500)) AS ProductName, CAST(vattax AS float) AS vattax, CAST(UnitPrice AS float) AS UnitPrice, (SELECT ISNULL(SUM(purchasedetail.qty), 0) AS Expr1 FROM purchasedetail INNER JOIN purchasem ON purchasedetail.invid = purchasem.invid WHERE (purchasedetail.pid = PRD.ProductID) AND (purchasem.date < '" + Request["s_date"] + "')) AS PurchaseOpenIn, (SELECT ISNULL(SUM(purchasedetail_1.qty), 0) AS Expr1 FROM purchasedetail AS purchasedetail_1 INNER JOIN purchasem AS purchasem_1 ON purchasedetail_1.invid = purchasem_1.invid WHERE (purchasedetail_1.pid = PRD.ProductID) AND (purchasem_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS PurchasebetweenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm WHERE (pid = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOpenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm AS labm_1 WHERE (pid = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBetweenIn, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID WHERE (srsdetail.prid = PRD.ProductID) AND (srsm.date < '" + Request["s_date"] + "')) AS SalesReturnOpenIn, (SELECT ISNULL(SUM(srsdetail_1.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 INNER JOIN srsm AS srsm_1 ON srsdetail_1.OrderID = srsm_1.OrderID WHERE (srsdetail_1.prid = PRD.ProductID) AND (srsm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesReturnBetweenIn, (SELECT ISNULL(SUM(srpdetail.qty), 0) AS Expr1 FROM srpdetail INNER JOIN srpm ON srpdetail.invid = srpm.invid WHERE (srpdetail.pid = PRD.ProductID) AND (srpm.date < '" + Request["s_date"] + "')) AS purchasereturnOpenOut, (SELECT ISNULL(SUM(srpdetail_1.qty), 0) AS Expr1 FROM srpdetail AS srpdetail_1 INNER JOIN srpm AS srpm_1 ON srpdetail_1.invid = srpm_1.invid WHERE (srpdetail_1.pid = PRD.ProductID) AND (srpm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS purchasereturnBetweenOut, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail AS ProductIngrDetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout2, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 AS ProductIngrDetail1_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout2, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOPout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail AS labdetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBTout, (SELECT ISNULL(SUM(Product.prdunit * Order_detail.ctn + Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID INNER JOIN Product ON Order_detail.prid = Product.ProductID WHERE (Order_detail.prid = PRD.ProductID) AND (Order_Master.date < '" + Request["s_date"] + "')) AS SalesOpenOut, (SELECT ISNULL(SUM(Product_1.prdunit * Order_detail_1.ctn + Order_detail_1.qty), 0) AS Expr1 FROM Order_detail AS Order_detail_1 INNER JOIN Order_Master AS Order_Master_1 ON Order_detail_1.OrderID = Order_Master_1.OrderID INNER JOIN Product AS Product_1 ON Order_detail_1.prid = Product_1.ProductID WHERE (Order_detail_1.prid = PRD.ProductID) AND (Order_Master_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesBetwOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = PRD.ProductID) AND (date < '" + Request["s_date"] + "')) AS DmgStkOpenOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail AS sampledetail_1 WHERE (pid = PRD.ProductID) AND (date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS DmgStkBetwOut FROM Product AS PRD WHERE (CategoryID IN (" + sel_opt + ") ) AND (LEN(ProductName) > 3)) AS derivedtbl_1) AS derivedtbl_2 ORDER BY CategoryID, pname) AS derivedtbl_3 ORDER BY  CategoryName,pname asc").ToList();
           // }
           // else
           // {
                var list = _context.Database.SqlQuery<RawMaterialReportQuery>("SELECT CategoryID, CategoryName, ProductID, pname, vattax,cost, AdjQTY, QTYin,QTYLabin, QTYrtnIn, qtyoutprd, qtyout , qtyoutlab, QTYSaleOut, qtyoutwast, total as ext1 FROM(SELECT TOP (100) PERCENT CategoryID, (SELECT CategoryName FROM Categories WHERE (CategoryID = derivedtbl_2.CategoryID)) AS CategoryName, ProductID, ProductName AS pname, vattax, UnitPrice AS cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast AS Qtybal, (AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast) * UnitPrice AS total FROM (SELECT CategoryID, ProductID, ProductName, vattax, UnitPrice, vattax + PurchaseOpenIn + labOpenIn + SalesReturnOpenIn - purchasereturnOpenOut - LabStkOpenout - LabStkOpenout2 - labOPout - SalesOpenOut - DmgStkOpenOut AS AdjQTY, PurchasebetweenIn AS QTYin, labBetweenIn AS QTYLabin, SalesReturnBetweenIn AS QTYrtnIn, purchasereturnBetweenOut AS qtyoutprd, LabStkBetwout + LabStkBetwout2 AS qtyout, labBTout AS qtyoutlab, SalesBetwOut AS QTYSaleOut, DmgStkBetwOut AS qtyoutwast, PurchaseOpenIn, PurchasebetweenIn, labOpenIn, labBetweenIn, SalesReturnOpenIn, SalesReturnBetweenIn, purchasereturnOpenOut, purchasereturnBetweenOut, LabStkOpenout, LabStkBetwout, LabStkOpenout2, LabStkBetwout2, labOPout, labBTout, SalesOpenOut, SalesBetwOut, DmgStkOpenOut, DmgStkBetwOut FROM (SELECT CAST(CategoryID AS decimal) AS CategoryID, CAST(ProductID AS decimal) AS ProductID, CAST(ProductName AS varchar(500)) AS ProductName, CAST(vattax AS float) AS vattax, CAST(UnitPrice AS float) AS UnitPrice, (SELECT ISNULL(SUM(purchasedetail.qty), 0) AS Expr1 FROM purchasedetail INNER JOIN purchasem ON purchasedetail.invid = purchasem.invid WHERE (purchasedetail.pid = PRD.ProductID) AND (purchasem.date < '" + Request["s_date"] + "')) AS PurchaseOpenIn, (SELECT ISNULL(SUM(purchasedetail_1.qty), 0) AS Expr1 FROM purchasedetail AS purchasedetail_1 INNER JOIN purchasem AS purchasem_1 ON purchasedetail_1.invid = purchasem_1.invid WHERE (purchasedetail_1.pid = PRD.ProductID) AND (purchasem_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS PurchasebetweenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm WHERE (pid = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOpenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm AS labm_1 WHERE (pid = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBetweenIn, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID WHERE (srsdetail.prid = PRD.ProductID) AND (srsm.date < '" + Request["s_date"] + "')) AS SalesReturnOpenIn, (SELECT ISNULL(SUM(srsdetail_1.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 INNER JOIN srsm AS srsm_1 ON srsdetail_1.OrderID = srsm_1.OrderID WHERE (srsdetail_1.prid = PRD.ProductID) AND (srsm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesReturnBetweenIn, (SELECT ISNULL(SUM(srpdetail.qty), 0) AS Expr1 FROM srpdetail INNER JOIN srpm ON srpdetail.invid = srpm.invid WHERE (srpdetail.pid = PRD.ProductID) AND (srpm.date < '" + Request["s_date"] + "')) AS purchasereturnOpenOut, (SELECT ISNULL(SUM(srpdetail_1.qty), 0) AS Expr1 FROM srpdetail AS srpdetail_1 INNER JOIN srpm AS srpm_1 ON srpdetail_1.invid = srpm_1.invid WHERE (srpdetail_1.pid = PRD.ProductID) AND (srpm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS purchasereturnBetweenOut, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail AS ProductIngrDetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout2, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 AS ProductIngrDetail1_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout2, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOPout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail AS labdetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBTout, (SELECT ISNULL(SUM(Product.prdunit * Order_detail.ctn + Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID INNER JOIN Product ON Order_detail.prid = Product.ProductID WHERE (Order_detail.prid = PRD.ProductID) AND (Order_Master.date < '" + Request["s_date"] + "')) AS SalesOpenOut, (SELECT ISNULL(SUM(Product_1.prdunit * Order_detail_1.ctn + Order_detail_1.qty), 0) AS Expr1 FROM Order_detail AS Order_detail_1 INNER JOIN Order_Master AS Order_Master_1 ON Order_detail_1.OrderID = Order_Master_1.OrderID INNER JOIN Product AS Product_1 ON Order_detail_1.prid = Product_1.ProductID WHERE (Order_detail_1.prid = PRD.ProductID) AND (Order_Master_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesBetwOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = PRD.ProductID) AND (date < '" + Request["s_date"] + "')) AS DmgStkOpenOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail AS sampledetail_1 WHERE (pid = PRD.ProductID) AND (date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS DmgStkBetwOut FROM Product AS PRD WHERE (CategoryID IN (" + sel_opt + ") and  CategoryID IN (select CategoryID from Categories where RawProductCheck=1) and Active='Active') AND (LEN(ProductName) > 3)) AS derivedtbl_1) AS derivedtbl_2 ORDER BY CategoryID, pname) AS derivedtbl_3 ORDER BY  CategoryName,pname asc").ToList();
           // }

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptRawMaterial.rpt"));

            rd.SetDataSource(list);
            rd.SetParameterValue("compname", "Test");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptRawMaterial.pdf");
        }
        public ActionResult PurchaseeIndex()
        {
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            return View(Cus_list);
        }
        public ActionResult PurchaseSearch(int type,int optt)
        {
            //var list = _context.Database.SqlQuery<RawMaterialReportQuery>("SELECT CategoryID, CategoryName, ProductID, pname, vattax, cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, Qtybal, total as ext1 FROM(SELECT TOP (100) PERCENT CategoryID, (SELECT CategoryName FROM Categories WHERE (CategoryID = derivedtbl_2.CategoryID)) AS CategoryName, ProductID, ProductName AS pname, vattax, UnitPrice AS cost, AdjQTY, QTYin, QTYLabin, QTYrtnIn, qtyoutprd, qtyout, qtyoutlab, QTYSaleOut, qtyoutwast, AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast AS Qtybal, (AdjQTY + QTYin + QTYLabin + QTYrtnIn - qtyoutprd - qtyout - qtyoutlab - QTYSaleOut - qtyoutwast) * UnitPrice AS total FROM (SELECT CategoryID, ProductID, ProductName, vattax, UnitPrice, vattax + PurchaseOpenIn + labOpenIn + SalesReturnOpenIn - purchasereturnOpenOut - LabStkOpenout - LabStkOpenout2 - labOPout - SalesOpenOut - DmgStkOpenOut AS AdjQTY, PurchasebetweenIn AS QTYin, labBetweenIn AS QTYLabin, SalesReturnBetweenIn AS QTYrtnIn, purchasereturnBetweenOut AS qtyoutprd, LabStkBetwout + LabStkBetwout2 AS qtyout, labBTout AS qtyoutlab, SalesBetwOut AS QTYSaleOut, DmgStkBetwOut AS qtyoutwast, PurchaseOpenIn, PurchasebetweenIn, labOpenIn, labBetweenIn, SalesReturnOpenIn, SalesReturnBetweenIn, purchasereturnOpenOut, purchasereturnBetweenOut, LabStkOpenout, LabStkBetwout, LabStkOpenout2, LabStkBetwout2, labOPout, labBTout, SalesOpenOut, SalesBetwOut, DmgStkOpenOut, DmgStkBetwOut FROM (SELECT CAST(CategoryID AS decimal) AS CategoryID, CAST(ProductID AS decimal) AS ProductID, CAST(ProductName AS varchar(500)) AS ProductName, CAST(vattax AS float) AS vattax, CAST(UnitPrice AS float) AS UnitPrice, (SELECT ISNULL(SUM(purchasedetail.qty), 0) AS Expr1 FROM purchasedetail INNER JOIN purchasem ON purchasedetail.invid = purchasem.invid WHERE (purchasedetail.pid = PRD.ProductID) AND (purchasem.date < '" + Request["s_date"] + "')) AS PurchaseOpenIn, (SELECT ISNULL(SUM(purchasedetail_1.qty), 0) AS Expr1 FROM purchasedetail AS purchasedetail_1 INNER JOIN purchasem AS purchasem_1 ON purchasedetail_1.invid = purchasem_1.invid WHERE (purchasedetail_1.pid = PRD.ProductID) AND (purchasem_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS PurchasebetweenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm WHERE (pid = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOpenIn, (SELECT ISNULL(SUM(whc), 0) AS Expr1 FROM labm AS labm_1 WHERE (pid = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBetweenIn, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID WHERE (srsdetail.prid = PRD.ProductID) AND (srsm.date < '" + Request["s_date"] + "')) AS SalesReturnOpenIn, (SELECT ISNULL(SUM(srsdetail_1.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 INNER JOIN srsm AS srsm_1 ON srsdetail_1.OrderID = srsm_1.OrderID WHERE (srsdetail_1.prid = PRD.ProductID) AND (srsm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesReturnBetweenIn, (SELECT ISNULL(SUM(srpdetail.qty), 0) AS Expr1 FROM srpdetail INNER JOIN srpm ON srpdetail.invid = srpm.invid WHERE (srpdetail.pid = PRD.ProductID) AND (srpm.date < '" + Request["s_date"] + "')) AS purchasereturnOpenOut, (SELECT ISNULL(SUM(srpdetail_1.qty), 0) AS Expr1 FROM srpdetail AS srpdetail_1 INNER JOIN srpm AS srpm_1 ON srpdetail_1.invid = srpm_1.invid WHERE (srpdetail_1.pid = PRD.ProductID) AND (srpm_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS purchasereturnBetweenOut, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM ProductIngrDetail AS ProductIngrDetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS LabStkOpenout2, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductIngrDetail1 AS ProductIngrDetail1_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS LabStkBetwout2, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail WHERE (ItemId = PRD.ProductID) AND (Date < '" + Request["s_date"] + "')) AS labOPout, (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail AS labdetail_1 WHERE (ItemId = PRD.ProductID) AND (Date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS labBTout, (SELECT ISNULL(SUM(Product.prdunit * Order_detail.ctn + Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID INNER JOIN Product ON Order_detail.prid = Product.ProductID WHERE (Order_detail.prid = PRD.ProductID) AND (Order_Master.date < '" + Request["s_date"] + "')) AS SalesOpenOut, (SELECT ISNULL(SUM(Product_1.prdunit * Order_detail_1.ctn + Order_detail_1.qty), 0) AS Expr1 FROM Order_detail AS Order_detail_1 INNER JOIN Order_Master AS Order_Master_1 ON Order_detail_1.OrderID = Order_Master_1.OrderID INNER JOIN Product AS Product_1 ON Order_detail_1.prid = Product_1.ProductID WHERE (Order_detail_1.prid = PRD.ProductID) AND (Order_Master_1.date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS SalesBetwOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = PRD.ProductID) AND (date < '" + Request["s_date"] + "')) AS DmgStkOpenOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail AS sampledetail_1 WHERE (pid = PRD.ProductID) AND (date BETWEEN '" + Request["s_date"] + "' AND '" + Request["e_date"] + "')) AS DmgStkBetwOut FROM Product AS PRD WHERE (CategoryID IN (1, 38, 43)) AND (LEN(ProductName) > 3)) AS derivedtbl_1) AS derivedtbl_2 ORDER BY CategoryID, pname) AS derivedtbl_3 ORDER BY pname").ToList();
            int opt = Convert.ToInt32(Request["opt"]);
            ReportDocument rd = new ReportDocument();
            if (type ==1 || optt==1)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT qtnm.invid as ext3,qtnm.invid, qtnm.date, qtnm.supid, qtnm.cargid, qtnm.cargocharges, qtnm.othercharges, qtnm.discount, qtnm.supinv, qtnm.supname, qtnm.paid, qtnm.note, qtnm.datetime, qtnm.builty, qtnm.cntrsupplier, qtnm.status, qtndetail.pname, qtndetail.cp, qtndetail.qty, qtndetail.total, isnull(qtndetail.tax,0) as tax, isnull(qtndetail.total+ qtndetail.tax,0) as taxtotal FROM qtnm INNER JOIN qtndetail ON qtnm.invid = qtndetail.invid Inner join Product ON Product.ProductID = qtndetail.pid where qtnm.status='TPO' and  qtnm.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "' and CategoryID in (select CategoryID from Categories where RawProductCheck=1) and Active='Active'").ToList();
            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptTPO.rpt"));
            }
            else if(type == 2 || optt == 1)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT purchasem.Invno as ext3,purchasem.invid, purchasem.date, purchasem.supid, purchasem.cargid, purchasem.cargocharges, purchasem.othercharges, purchasem.discount, purchasem.supinv, purchasem.supname, purchasem.paid, purchasem.note, purchasem.datetime, purchasem.builty, purchasem.cntrsupplier, purchasem.status, purchasedetail.pname, purchasedetail.cp, purchasedetail.qty, purchasedetail.total, isnull(purchasedetail.tax,0) as tax, isnull(purchasedetail.total+ purchasedetail.tax,0) as taxtotal FROM purchasem INNER JOIN purchasedetail ON purchasem.invid = purchasedetail.invid Inner join Product ON Product.ProductID = purchasedetail.pid where purchasem.status='TPINV' and purchasem.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "' and CategoryID in (select CategoryID from Categories where RawProductCheck=1) and Active='Active'").ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptPurchasee.rpt"));
            }
            else if (type == 3 || optt == 1)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT srpm.invid as ext3,srpm.invid, srpm.date, srpm.supid, srpm.cargid, srpm.cargocharges, srpm.othercharges, srpm.discount, srpm.supinv, srpm.supname, srpm.paid, srpm.note, srpm.datetime, srpm.builty, srpm.cntrsupplier, srpm.status, srpdetail.pname, srpdetail.cp, srpdetail.qty, srpdetail.total, isnull(srpdetail.tax,0) as tax, isnull(srpdetail.total+ srpdetail.tax,0) as taxtotal FROM srpm INNER JOIN srpdetail ON srpm.invid = srpdetail.invid Inner join Product ON Product.ProductID = srpdetail.pid where srpm.status='TPRINV' and srpm.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "' and CategoryID in (select CategoryID from Categories where RawProductCheck=1) and Active='Active'").ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptTPRINV.rpt"));
            }

            if (type == 1 || optt == 2)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT qtnm.invid as ext3,qtnm.invid, qtnm.date, qtnm.supid, qtnm.cargid, qtnm.cargocharges, qtnm.othercharges, qtnm.discount, qtnm.supinv, qtnm.supname, qtnm.paid, qtnm.note, qtnm.datetime, qtnm.builty, qtnm.cntrsupplier, qtnm.status, qtndetail.pname, qtndetail.cp, qtndetail.qty, qtndetail.total, isnull(qtndetail.tax,0) as tax, isnull(qtndetail.total+ qtndetail.tax,0) as taxtotal FROM qtnm INNER JOIN qtndetail ON qtnm.invid = qtndetail.invid Inner join Product ON Product.ProductID = qtndetail.pid where qtnm.status='PO' and  qtnm.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "' and CategoryID in (select CategoryID from Categories where RawProductCheck=1) and Active='Active'").ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptPo.rpt"));
            }
            else if (type == 2 || optt == 2)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT purchasem.Invno as ext3,purchasem.invid, purchasem.date, purchasem.supid, purchasem.cargid, purchasem.cargocharges, purchasem.othercharges, purchasem.discount, purchasem.supinv, purchasem.supname, purchasem.paid, purchasem.note, purchasem.datetime, purchasem.builty, purchasem.cntrsupplier, purchasem.status, purchasedetail.pname, purchasedetail.cp, purchasedetail.qty, purchasedetail.total, isnull(purchasedetail.tax,0) as tax, isnull(purchasedetail.total+ purchasedetail.tax,0) as taxtotal  FROM purchasem INNER JOIN purchasedetail ON purchasem.invid = purchasedetail.invid Inner join Product ON Product.ProductID = purchasedetail.pid where purchasem.status='PINV' and purchasem.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "' and CategoryID in (select CategoryID from Categories where RawProductCheck=1) and Active='Active'").ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptPINV.rpt"));
            }
            else if (type == 2 || optt == 2)
            {
                list = _context.Database.SqlQuery<PurchaseReportQuery>("SELECT srpm.invid as ext3,srpm.invid, srpm.date, srpm.supid, srpm.cargid, srpm.cargocharges, srpm.othercharges, srpm.discount, srpm.supinv, srpm.supname, srpm.paid, srpm.note, srpm.datetime, srpm.builty, srpm.cntrsupplier, srpm.status, srpdetail.pname, srpdetail.cp, srpdetail.qty, srpdetail.total, isnull(srpdetail.tax,0) as tax, isnull(srpdetail.total+ srpdetail.tax,0) as taxtotal FROM srpm INNER JOIN srpdetail ON srpm.invid = srpdetail.invid Inner join Product ON Product.ProductID = srpdetail.pid where srpm.status='PRINV' and srpm.date between'" + Request["s_date"] + "' and '" + Request["e_date"] + "'").ToList();
                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptPRINV.rpt"));
            }
          


            rd.SetDataSource(list);
            rd.SetParameterValue("compname", "Test");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptPurchasee.pdf");
        }
        public ActionResult SaleReportWctnView(int ID)
        {
            var list = _context.Database.SqlQuery<SaleDetail>("Select * from SaleDetails where InvId = " + ID + " and InvType='SINVWCTN'").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT DiscountAmount FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName  FROM SaleMasters where InvID =" + ID + " and InvType='SINVWCTN'").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptSaleWctnInv.rpt"));

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
            rd.SetParameterValue("Image", Image);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptSaleWctnInv.pdf");
        }
        public ActionResult Create(PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from purchasem where status='PRINV'").FirstOrDefault();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, PoMaster poMaster)
        {
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srpdetail (Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES ('PRINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srpm (builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid ) VALUES ('" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','PRINV','" + DateTime.Now + "',0,0,0,0,0)");
            return RedirectToAction("Index");
        }
        public ActionResult SINVWCTNReport(int? ID, SaleMaster SaleMaster, TransactionDetail TransactionDetail)
        {
            SaleMaster.InvID = _context.Database.SqlQuery<int>("Select InvID from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.NetAmount = _context.Database.SqlQuery<decimal>("Select NetAmount from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.CargoCharges = _context.Database.SqlQuery<decimal>("Select NetAmount from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.CustomerId = _context.Database.SqlQuery<int>("Select CustomerId from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            SaleMaster.Date = _context.Database.SqlQuery<string>("Select Date from QuotationMaster where InvID =" + ID + "").FirstOrDefault();
            ViewBag.Image = _context.Database.SqlQuery<string>("Select MImage from Settings").FirstOrDefault();
            //SaleMaster = _context.SaleMaster.SingleOrDefault(c => c.InvID == ID );
            SaleMaster.CargoName = NumberToWords(Decimal.ToInt32(SaleMaster.NetAmount));
            int b_unit = Convert.ToInt32(Session["BusinessUnit"]);
            var Settings = _context.BusinessUnits.SingleOrDefault(c => c.Id == b_unit);
            var sett = _context.Settings.SingleOrDefault(c => c.ID == 2);
            var customer = _context.Customer.SingleOrDefault(d => d.AccountNo == SaleMaster.CustomerId);
            //var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvID=" + ID + " and InvType = 'SINVWOCTN' ORDER BY InvID DESC").ToList();
            var q_detail = _context.Database.SqlQuery<QuotationQuery>("SELECT *,(Select Munit From Products where id=QuotationDetail.ItemID) as Munit from QuotationDetail where InvID=" + ID + "").ToList();
            var SaleInvVM = new SaleInvVM
            {
                sett = sett,
                q_detail = q_detail,
                Settings = Settings,
                customer = customer,
                SaleMaster = SaleMaster,
                TransactionDetail = TransactionDetail,
                //sale_list_woc = sale_list_woc,
            };
            return View(SaleInvVM);
        }

        public ActionResult Edit(int? ID)
        {
            var poMaster = _context.Database.SqlQuery<PoMaster>("select * from srpm where invid =" + ID + " and status='PRINV'").SingleOrDefault();
            var poDetail = _context.Database.SqlQuery<PoDetail>("select * from srpdetail where invid =" + ID + " and Stauts='PRINV'").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poDetail = poDetail,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, PoMaster poMaster)
        {
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + poMaster.invid + " and status='PRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + poMaster.invid + "  and Stauts='PRINV' ");
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO srpdetail (Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES ('PRINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO srpm (builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid ) VALUES ('" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','PRINV','" + DateTime.Now + "',0,0,0,0,0)");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {

            var getgrosspackage = _context.Database.SqlQuery<Customer>("Select * From Customers where AccountNo =" + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + ID + " and status='PRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + ID + "  and Stauts='PRINV' ");
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