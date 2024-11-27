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
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
 
    public class ThirdLevelAccountController : Controller
    {
        // GET: SecondLevelAccount
        private ApplicationDbContext _context;
        public ThirdLevelAccountController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public int count;
        public int account_no;
        public int final_acc;
        // GET: Second
        public ActionResult Index()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<AcMain>("SELECT dbo.AccountTitles.b_unit, dbo.AccountTitles.Id, dbo.AccountTitles.AccountNo, dbo.AccountTitles.AccountTitleName,(Select Top(1) a_title From Ac_main where account_no=AccountTitles.AccMain) as a_title, (Select Top(1) a_title From Ac_second where account_no=AccountTitles.SecondLevel) as secondlevel, (Select Top(1) ac_h_name From Ac_head where ac_h_id=AccountTitles.AccountHeadId) as ac_h_name FROM dbo.AccountTitles").ToList();
            return View(list);
        }
        public ActionResult Create(Ac_main Ac_main,AccountTitle accountTitle )
        {
            var Ac_head_list = _context.Ac_head.ToList();
            var Account_headVm = new Account_headVm
            {
                accountTitle = accountTitle,
                Ac_head_list = Ac_head_list,
                Ac_main = Ac_main
            };
            return View(Account_headVm);
        }
        [HttpGet]
        public ActionResult ShowData(int message)
        {
            var lst = _context.Database.SqlQuery<AcMain>("Select * From Ac_main where a_head=" + message + " ").ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetThirdData(int message)
        {
            var lst = _context.Database.SqlQuery<AcMain>("Select * From ac_second where ac_main=" + message + " ").ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(Ac_main Ac_main, int a_main, Ac_second ac_Second, AccountTitle accountTitle, int second_id)
        {
            var ac_main = _context.Database.SqlQuery<int>("Select  Count(*) as count from accounttitles where AccountTitleName = '" + accountTitle.AccountTitleName + "'").FirstOrDefault();
            var count = ac_main;
            if (count == 0)
            {
                if (accountTitle.AccountHeadId == 1)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM accounttitles where AccountHeadId = '1'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("10000003");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (accountTitle.AccountHeadId == 2)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM accounttitles where AccountHeadId = '2'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("2200001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (accountTitle.AccountHeadId == 3)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM accounttitles where AccountHeadId = '3'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("3300001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (accountTitle.AccountHeadId == 4)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM accounttitles where AccountHeadId = '4'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("4400001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (accountTitle.AccountHeadId == 5)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM accounttitles where AccountHeadId = '5'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("5500001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                var Bunit = Convert.ToString(Session["BusinessUnit"]);
                _context.Database.ExecuteSqlCommand("insert into accounttitles values(" + final_acc + "," + a_main + "," + accountTitle.AccountHeadId + ","+second_id+ ",N'" + accountTitle.AccountTitleName + "',N'" + accountTitle.AccountTitleName + "',0,0,'0')");

                return RedirectToAction("Index");
            }
            else
            {

                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.AccountTitle.SingleOrDefault(c => c.Id == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, AccountTitle accountTitle)
        {
            _context.Database.ExecuteSqlCommand("Update AccountTitles set AccountTitleName  = N'" + accountTitle.AccountTitleName + "' where Id = " + ID + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            var ac_main = _context.Database.SqlQuery<int>("Select  Count(*) as count from TransactionDetails where AccountId = " + ID + "").FirstOrDefault();
            var count = ac_main;
            if(count>0)
            {

            }
            else
            {
                _context.Database.ExecuteSqlCommand("Delete from AccountTitles where AccountNo  = " + ID + "");
            }
            return RedirectToAction("Index");
        }
    }
}