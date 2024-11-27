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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext _context;
        private List<LedgerQuery> trans_list;
        public TransactionsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Transactions
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
        public ActionResult TransactionSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles  ").ToList();
            int opening_bal = _context.Database.SqlQuery<int>("SELECT (dr - cr) as opening FROM AccountTitles WHERE AccountNo = " + accountTitle.AccountNo + "").FirstOrDefault();
            accountTitle.AccountTitleName = _context.Database.SqlQuery<string>("SELECT AccountTitleName FROM AccountTitles WHERE AccountNo = " + accountTitle.AccountNo + "").FirstOrDefault();

            //if (accountTitle.AccountNo == 0)
            //{
            //    trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.TransDate between '" + s_date + "' AND '" + e_date + "'").ToList();

            //}
            //else
            
            if (accountTitle.AccountNo != 0 && s_date != "" && e_date != "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + accountTitle.AccountNo + " AND td.TransDate between '" + s_date + "' AND '" + e_date + "'  and td.b_unit='" + Bunit + "'").ToList();

            }
            else if (accountTitle.AccountNo == 0 && s_date != "" && e_date != "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.TransDate between '" + s_date + "' AND '" + e_date + "'  and td.b_unit='" + Bunit + "' and act.b_unit='" + Bunit + "'").ToList();

            }
            else if (s_date == "" && e_date == "" && accountTitle.AccountNo != 0 )
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + accountTitle.AccountNo + " and td.b_unit='" + Bunit + "' and act.b_unit='" + Bunit + "'").ToList();

            }
            else if (s_date == "" && e_date == "" && accountTitle.AccountNo == 0)
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno   and td.b_unit='" + Bunit + "' and act.b_unit='" + Bunit + "'").ToList();

            }

            accountTitle.cr = opening_bal;
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
                transactionDetail = transactionDetail,
                trans_list = trans_list,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("dd-MM-yyyy");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("dd-MM-yyyy");
            return View(Chart_of_account_Vm);
        }
    }
}