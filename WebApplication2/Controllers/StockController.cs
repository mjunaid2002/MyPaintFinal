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

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class StockController : Controller
    {
        private ApplicationDbContext _context;
        public StockController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult StockReport()
        {
            var list = _context.Database.SqlQuery<stockquery>("SELECT ID, Name, CostPrice, Barcode, BrandName, CatName, PiecesBox, QTYIN, QTYOUT, QTYIN - QTYOUT AS BAL, FLOOR((QTYIN - QTYOUT) / PiecesBox) AS CTN, (QTYIN - QTYOUT) % PiecesBox AS QTY, CostPrice * (QTYIN - QTYOUT) AS TotalCost FROM(SELECT   Products.ID, Products.Name, Products.CostPrice, Products.Barcode, Brands.Name AS BrandName, Categories.Name AS CatName, Products.PiecesBox,(SELECT ISNULL(SUM(Ctn * PiecesBox + Qty), 0) AS Expr1 FROM  PurDetails WHERE(ItemID = Products.ID)) AS QTYIN,(SELECT  ISNULL(SUM(Ctn * PiecesBox + Qty), 0) AS Expr1 FROM  SaleDetails WHERE(ItemID = Products.ID)) AS QTYOUT FROM Products INNER JOIN Categories ON Products.CatID = Categories.Id INNER JOIN Brands ON Products.BrandID = Brands.Id) AS derivedtbl_1").ToList();
            return View(list);
        }

        public ActionResult PrintRep()
        {
            var list = _context.Database.SqlQuery<stockquery>("SELECT ID, Name, CostPrice, Barcode, BrandName, CatName, PiecesBox, QTYIN, QTYOUT, QTYIN - QTYOUT AS BAL, FLOOR((QTYIN - QTYOUT) / PiecesBox) AS CTN, (QTYIN - QTYOUT) % PiecesBox AS QTY, CostPrice * (QTYIN - QTYOUT) AS TotalCost FROM(SELECT   Products.ID, Products.Name, Products.CostPrice, Products.Barcode, Brands.Name AS BrandName, Categories.Name AS CatName, Products.PiecesBox,(SELECT ISNULL(SUM(Ctn * PiecesBox + Qty), 0) AS Expr1 FROM  PurDetails WHERE(ItemID = Products.ID)) AS QTYIN,(SELECT  ISNULL(SUM(Ctn * PiecesBox + Qty), 0) AS Expr1 FROM  SaleDetails WHERE(ItemID = Products.ID)) AS QTYOUT FROM Products INNER JOIN Categories ON Products.CatID = Categories.Id INNER JOIN Brands ON Products.BrandID = Brands.Id) AS derivedtbl_1").ToList();
            return View(list);
        }
    }
}