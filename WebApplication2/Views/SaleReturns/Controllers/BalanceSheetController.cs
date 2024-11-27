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
    public class BalanceSheetController : Controller
    {
        private ApplicationDbContext _context;
        private List<LedgerQuery> trans_list;
        public BalanceSheetController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: BalanceSheet
        public ActionResult Index(AccountTitle accountTitle)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='" + Bunit + "'").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpPost]
        public ActionResult BalanceSheetSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='" + Bunit + "'").ToList();
            var ass = _context.Database.SqlQuery<TrailQuery>("SELECT AccountNo, AccountTitleName, cdr, ccr FROM(SELECT AccountNo, AccountTitleName, cr, dr, (SELECT ISNULL(SUM(Dr), 0) AS Expr1 FROM dbo.TransactionDetails WHERE transdate <= '" + s_date + "'  and TransactionDetails.b_unit= '" + Bunit + "' and (AccountId = dbo.AccountTitles.AccountNo)) AS cdr, (SELECT ISNULL(SUM(Cr), 0) AS Expr1 FROM dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate <= '" + s_date + "'  and (AccountId = dbo.AccountTitles.AccountNo) and AccountTitles.b_unit='" + Bunit + "') AS ccr FROM dbo.AccountTitles where AccountHeadId=1 and b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var lib = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cdr, ccr FROM     (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM      dbo.TransactionDetails WHERE   transdate <= '" + s_date + "'  and TransactionDetails.b_unit='" + Bunit + "' and     (AccountId = dbo.AccountTitles.AccountNo)) AS cdr, (SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate <= '" + s_date + "'   and    (AccountId = dbo.AccountTitles.AccountNo)  and AccountTitles.b_unit='" + Bunit + "') AS ccr  FROM      dbo.AccountTitles where AccountHeadId=2 and b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var cap = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cdr, ccr FROM     (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM      dbo.TransactionDetails WHERE   transdate <= '" + s_date + "'  and TransactionDetails.b_unit = '" + Bunit + "'  and     (AccountId = dbo.AccountTitles.AccountNo)) AS cdr, (SELECT        ISNULL(SUM(Cr), 0) AS Expr1 FROM            dbo.TransactionDetails AS TransactionDetails_1 WHERE transdate <= '" + s_date + "'  and    (AccountId = dbo.AccountTitles.AccountNo)  and AccountTitles.b_unit='" + Bunit + "') AS ccr  FROM      dbo.AccountTitles where AccountHeadId=3 and b_unit='" + Bunit + "') AS derivedtbl_2").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
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