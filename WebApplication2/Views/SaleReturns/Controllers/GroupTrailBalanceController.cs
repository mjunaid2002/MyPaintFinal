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
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class GroupTrailBalanceController : Controller
    {
        private ApplicationDbContext _context;

        public GroupTrailBalanceController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: GroupTrailBalance
        public ActionResult Index()
        {
            var Ac_head_list = _context.Ac_head.ToList();
            var TouchSaleInvVM = new TouchSaleInvVM
            {
                Ac_head_list = Ac_head_list,
              };
            return View(TouchSaleInvVM);
        }
        [HttpPost]
        public ActionResult GroupTrailBalanceSearch(AccountTitle accountTitle, int acc_head, string e_date, TransactionDetail transactionDetail)
        {
            var Ac_head_list = _context.Ac_head.ToList();
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles ").ToList();
            var trans = _context.Database.SqlQuery<TrailQuery>("SELECT  AccountNo, AccountTitleName, cr, dr, cdr, ccr FROM   (SELECT   AccountNo, AccountTitleName, cr, dr, cdr, ccr FROM   (SELECT        AccountNo, AccountTitleName, cr, dr, (SELECT        ISNULL(SUM(Dr), 0) AS Expr1 FROM   dbo.TransactionDetails WHERE   transdate<='" + e_date + "' and     (AccountId = dbo.AccountTitles.AccountNo)) AS cdr, (SELECT   ISNULL(SUM(Cr), 0) AS Expr1 FROM   dbo.TransactionDetails AS TransactionDetails_1 WHERE    transdate<='" + e_date + "'  and    (AccountId = dbo.AccountTitles.AccountNo)) AS ccr FROM     dbo.AccountTitles  where (dbo.AccountTitles.AccountHeadId = "+ acc_head + ")) AS derivedtbl_2) AS derivedtbl_1").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                Ac_head_list = Ac_head_list,
                third_level = third_level,
                accountTitle = accountTitle,
                trans = trans,
            };
            return View(Chart_of_account_Vm);
        }
    }
}