using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class IncomestatementController : Controller
    {
        private ApplicationDbContext _context;
          public IncomestatementController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Incomestatement
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult IncomestatementSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var item_ist = _context.Database.SqlQuery<TrailQuery>("SELECT   SUM(dbo.SaleDetails.NetTotal) as saledetailtotal, dbo.SaleDetails.ItemName, dbo.SaleDetails.NetTotal, dbo.Products.ID, dbo.SaleDetails.ItemID FROM            dbo.Products INNER JOIN dbo.SaleDetails ON dbo.Products.ID = dbo.SaleDetails.ItemID where BusinessUnit='"+ Bunit + "' Group by dbo.SaleDetails.NetTotal,dbo.SaleDetails.ItemName, dbo.SaleDetails.NetTotal, dbo.Products.ID, dbo.SaleDetails.ItemID ").ToList();
            var item_ist2 = _context.Database.SqlQuery<TrailQuery>("SELECT   SUM(dbo.SaleDetailDis.retailprice) as saledetaildistotal, dbo.SaleDetailDis.prname, dbo.SaleDetailDis.retailprice, dbo.Products.ID, dbo.SaleDetailDis.prid FROM            dbo.Products INNER JOIN dbo.SaleDetailDis ON dbo.Products.ID = dbo.SaleDetailDis.prid where BusinessUnit='" + Bunit + "' Group by dbo.SaleDetailDis.retailprice,dbo.SaleDetailDis.prname, dbo.SaleDetailDis.retailprice, dbo.Products.ID, dbo.SaleDetailDis.prid ").ToList();
            var total_cgs = _context.Database.SqlQuery<TrailQuery>("SELECT  ISNULL(SUM(dr),0) as trans_balance FROM TransactionDetails WHERE accountid = '5500001' and  b_unit='" + Bunit + "'").ToList();
            var exp_list = _context.Database.SqlQuery<TrailQuery>("SELECT   AccountHeadId,AccountNo, AccountTitleName, cr, dr, cdr, ccr FROM      (SELECT        AccountHeadId,AccountNo, AccountTitleName, cr, dr, cdr, ccr FROM      (SELECT        AccountHeadId,AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM        dbo.TransactionDetails WHERE   (AccountId = dbo.AccountTitles.AccountNo) and  b_unit='"+Bunit+ "') AS cdr,(SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE  (AccountId = dbo.AccountTitles.AccountNo ) and  b_unit='" + Bunit + "') AS ccr FROM            dbo.AccountTitles where dbo.AccountTitles.AccountHeadId=5 and  b_unit='" + Bunit + "') AS derivedtbl_2) AS derivedtbl_1").ToList();
            var ass = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cdr, ccr FROM     (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM      dbo.TransactionDetails WHERE   transdate Between '" + s_date + "' AND '" + e_date + "' and     (AccountId = dbo.AccountTitles.AccountNo) and  TransactionDetails.b_unit='" + Bunit + "') AS cdr, (SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate Between '" + s_date + "' AND '" + e_date + "'  and    (AccountId = dbo.AccountTitles.AccountNo) and TransactionDetails_1.b_unit='" + Bunit + "') AS ccr  FROM      dbo.AccountTitles where AccountHeadId=1 and  b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var lib = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cdr, ccr FROM     (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM      dbo.TransactionDetails WHERE   transdate Between '" + s_date + "' AND '" + e_date + "' and     (AccountId = dbo.AccountTitles.AccountNo) and  TransactionDetails.b_unit='" + Bunit + "') AS cdr, (SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate Between '" + s_date + "' AND '" + e_date + "'  and    (AccountId = dbo.AccountTitles.AccountNo) and TransactionDetails_1.b_unit='" + Bunit + "') AS ccr  FROM      dbo.AccountTitles where AccountHeadId=2 and  b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var cap = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cdr, ccr FROM     (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM      dbo.TransactionDetails WHERE   transdate Between '" + s_date + "' AND '" + e_date + "' and     (AccountId = dbo.AccountTitles.AccountNo) and  TransactionDetails.b_unit='" + Bunit + "') AS cdr, (SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate Between '" + s_date + "' AND '" + e_date + "'  and    (AccountId = dbo.AccountTitles.AccountNo) and TransactionDetails_1.b_unit='" + Bunit + "') AS ccr  FROM      dbo.AccountTitles where AccountHeadId=3 and  b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                exp_list = exp_list,
                total_cgs = total_cgs,
                item_ist = item_ist,
                item_ist2 = item_ist2,
                ass = ass,
                lib = lib,
                cap = cap,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("dd-MM-yyyy");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("dd-MM-yyyy");
            return View(Chart_of_account_Vm);
        }
    }
}