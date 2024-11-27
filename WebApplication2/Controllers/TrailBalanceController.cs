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
    public class TrailBalanceController : Controller
    {
        private ApplicationDbContext _context;
        private List<LedgerQuery> trans_list;
        public TrailBalanceController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: TrailBalance
        public ActionResult Index(AccountTitle accountTitle)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='" + Bunit + "' ").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpPost]
        public ActionResult TrailBalanceSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='" + Bunit + "' ").ToList();
            var Ac_head = _context.Database.SqlQuery<Ac_head>("SELECT * FROM Ac_head  ").ToList();
            var trans = _context.Database.SqlQuery<TrailQuery>("SELECT AccountHeadId,AccountNo, AccountTitleName, cdr, ccr, dr,cr,odr,ocr FROM(SELECT AccountHeadId,AccountNo, AccountTitleName, cdr, ccr, dr,cr,odr,ocr FROM(SELECT AccountHeadId,AccountNo, AccountTitleName, (SELECT ISNULL(SUM(Dr), 0) AS Expr1 FROM dbo.TransactionDetails WHERE (TransDate BETWEEN '" + s_date + "' AND '" + e_date + "' and b_unit='" + Bunit + "') AND (AccountId = AccountTitles_1.AccountNo)) AS cdr , ( SELECT ISNULL(SUM(Dr), 0) AS Expr1 FROM dbo.TransactionDetails WHERE (TransDate < '" + s_date + "' and b_unit='" + Bunit + "') AND (AccountId = AccountTitles_1.AccountNo)) AS odr , ( SELECT ISNULL(SUM(CR), 0) AS Expr1 FROM dbo.TransactionDetails WHERE (TransDate < '" + s_date + "' and b_unit='" + Bunit + "') AND (AccountId = AccountTitles_1.AccountNo)) AS ocr , (SELECT ISNULL(SUM(OpeningBalance.Dr), 0) AS Expr1 FROM OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo AND OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id WHERE ('" + s_date + "' BETWEEN Sessions.StartDate AND Sessions.EndDate) AND (OpeningBalance.AccountNo = AccountTitles_1.AccountNo) and OpeningBalance.b_unit='" + Bunit + "') AS dr, (SELECT ISNULL(SUM(OpeningBalance.Cr), 0) AS Expr1 FROM OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo AND OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id WHERE ('" + s_date + "' BETWEEN Sessions.StartDate AND Sessions.EndDate) AND (OpeningBalance.AccountNo = AccountTitles_1.AccountNo) and OpeningBalance.b_unit='" + Bunit + "') AS cr, (SELECT ISNULL(SUM(Cr), 0) AS Expr1 FROM dbo.TransactionDetails AS TransactionDetails_1 WHERE (TransDate BETWEEN '" + s_date + "' AND '" + e_date + "') AND (AccountId = AccountTitles_1.AccountNo) and b_unit='" + Bunit + "') AS ccr FROM dbo.AccountTitles AS AccountTitles_1 WHERE (b_unit = '" + Bunit + "')) AS derivedtbl_2) AS derivedtbl_1").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                Ac_head = Ac_head,
                third_level = third_level,
                accountTitle = accountTitle,
                trans = trans,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("yyyy-MM-dd");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("yyyy-MM-dd");
            return View(Chart_of_account_Vm);
        }
    }
}