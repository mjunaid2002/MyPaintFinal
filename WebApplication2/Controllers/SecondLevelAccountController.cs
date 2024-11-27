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
    public class SecondLevelAccountController : Controller
    {
        // GET: SecondLevelAccount
        private ApplicationDbContext _context;

        public SecondLevelAccountController()
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
            var list = _context.Database.SqlQuery<AcMain>("SELECT Ac_second.id, Ac_second.account_no, Ac_second.a_title,(Select Top(1) a_title From Ac_main where account_no=Ac_second.ac_main) as FirstLevelAccount, (Select Top(1) ac_h_name From Ac_head where ac_h_id=Ac_second.head_id) as secondlevel FROM Ac_second").ToList();
            return View(list);
        }
        public ActionResult Create(Ac_main Ac_main,Ac_second ac_Second)
        {
            var Ac_head_list = _context.Ac_head.ToList();
            var Account_headVm = new Account_headVm
            {
                ac_Second = ac_Second,
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
        [HttpPost]
        public ActionResult Save(Ac_main Ac_main,int a_main,Ac_second ac_Second)
        {
            var ac_main = _context.Database.SqlQuery<int>("Select  Count(*) as count from ac_second where a_title = '" + ac_Second.a_title + "'").FirstOrDefault();
            var count = ac_main;
            if (count == 0)
            {
                if (ac_Second.head_id == 1)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '1'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("1000001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (ac_Second.head_id == 2)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '2'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("2000001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (ac_Second.head_id == 3)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '3'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("3000001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (ac_Second.head_id == 4)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '4'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("4000001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (ac_Second.head_id == 5)
                {
                    ac_Second.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '5'").FirstOrDefault();
                    account_no = ac_Second.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("5000001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                _context.Database.ExecuteSqlCommand("insert into ac_second values(" + ac_Second.head_id + "," + a_main + "," + final_acc + ",N'" + ac_Second.a_title + "','0')");
               
                return RedirectToAction("Index");
            }
            else
            {
               
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int? ID)
        {
            var data = _context.Ac_second.SingleOrDefault(c => c.ID == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, Ac_second ac_Second )
        {
            _context.Database.ExecuteSqlCommand("Update ac_Second set a_title  = N'" + ac_Second.a_title + "' where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}