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
    public class OpenLedgerController : Controller
    {
        private ApplicationDbContext _context;
        private List<LedgerQuery> trans_list;
        public OpenLedgerController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Ledger
        public ActionResult Index(AccountTitle accountTitle)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='"+ Bunit + "' ").ToList();
             var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpPost]
        public ActionResult LedgerSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);


            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='" + Bunit + "' ").ToList();
            int dr = _context.Database.SqlQuery<int>("SELECT  isnull(SUM (OpeningBalance.Dr),0) as Dr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo where OpeningBalance.AccountNo = " + accountTitle.AccountNo + " and AccountTitles.b_unit='" + Bunit + "'  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit").FirstOrDefault();
            //   int dr = _context.Database.SqlQuery<int>("SELECT  isnull(SUM (OpeningBalance.Dr),0) as Dr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id where  Sessions.b_unit='" + Bunit + "' and  '" + s_date + "'  between Sessions.StartDate  and   Sessions.EndDate and OpeningBalance.AccountNo = " + accountTitle.AccountNo + "  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit,Sessions.StartDate, Sessions.EndDate").FirstOrDefault();
            //  int cr = _context.Database.SqlQuery<int>("SELECT   isnull(SUM (OpeningBalance.Cr),0) as Cr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id where Sessions.b_unit='" + Bunit + "' and '" + s_date + "'  between Sessions.StartDate   and   Sessions.EndDate and OpeningBalance.AccountNo = " + accountTitle.AccountNo + "  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit,Sessions.StartDate, Sessions.EndDate").FirstOrDefault();
            int cr = _context.Database.SqlQuery<int>("SELECT  isnull(SUM (OpeningBalance.cr),0) as Dr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo where OpeningBalance.AccountNo = " + accountTitle.AccountNo + " and AccountTitles.b_unit='" + Bunit + "'  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit").FirstOrDefault();


            decimal op = 0;

            int headid = _context.Database.SqlQuery<int>("SELECT AccountHeadId from  AccountTitles where accountno=" + accountTitle.AccountNo + "").FirstOrDefault();
            int opening_bal = 0;
            if (headid == 1 || headid == 5)
            {
                op = _context.Database.SqlQuery<decimal>("SELECT   isnull(SUM (dr-cr),0) as Cr FROM   TransactionDetails  where TransDate < '" + s_date + "'  and AccountId = " + accountTitle.AccountNo + "   and b_unit='" + Bunit + "'").FirstOrDefault();
                opening_bal = dr - cr;
            }
            else if (headid == 2 || headid == 4)
            {
                op = _context.Database.SqlQuery<decimal>("SELECT   isnull(SUM (cr-dr),0) as Cr FROM   TransactionDetails  where TransDate < '" + s_date + "'  and AccountId = " + accountTitle.AccountNo + "   and b_unit='" + Bunit + "'").FirstOrDefault();
                opening_bal = cr - dr;
            }
            if (accountTitle.AccountNo == 0)
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.TransDate between '" + s_date + "' AND '" + e_date + "'  and act.b_unit='" + Bunit + "'").ToList();

            }
            else if (accountTitle.AccountNo != 0 && s_date != "" && e_date != "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + accountTitle.AccountNo + " AND td.TransDate between '" + s_date + "' AND '" + e_date + "'  and act.b_unit='" + Bunit + "'").ToList();

            }
            else if (s_date == "" && e_date == "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + accountTitle.AccountNo + "  and act.b_unit='" + Bunit + "'").ToList();

            }
            accountTitle.cr = opening_bal + Convert.ToInt32(op);
            accountTitle.AccountTitleName = _context.Database.SqlQuery<string>("SELECT AccountTitleName FROM AccountTitles WHERE AccountNo = " + accountTitle.AccountNo + "").FirstOrDefault();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
                transactionDetail = transactionDetail,
                trans_list = trans_list,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("yyyy-MM-dd");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("yyyy-MM-dd");
            return View(Chart_of_account_Vm);
        }
    }
}