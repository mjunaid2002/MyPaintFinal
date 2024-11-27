using System;
using System.Linq;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class FirstLevelController : Controller
    {
        // GET: FirstLevel
        private ApplicationDbContext _context;
        public FirstLevelController()
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
        // GET: Purchase
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<AcMain>("SELECT *,(Select ac_h_name From ac_head where ac_h_id=Ac_main.a_head) as HeadName FROM ac_main").ToList();
            return View(list);
        }
        public ActionResult Create(Ac_main Ac_main)
        {
            var Ac_head_list = _context.Ac_head.ToList();
             var Account_headVm = new Account_headVm
            {
                Ac_head_list = Ac_head_list,
                Ac_main = Ac_main
            };
            return View(Account_headVm);
        }
        [HttpPost]
        public ActionResult Save(Ac_main Ac_main)
        {
            var ac_main = _context.Database.SqlQuery<int>("Select  Count(*) as count from Ac_main where a_title = '" + Ac_main.a_title + "'").FirstOrDefault();
            var count = ac_main;
            if (count == 0)
            {
                if (Ac_main.a_head == 1)
                {
                    Ac_main.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '1'").FirstOrDefault();
                    account_no = Ac_main.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("1001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (Ac_main.a_head == 2)
                {
                    Ac_main.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '2'").FirstOrDefault();
                    account_no = Ac_main.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("2001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (Ac_main.a_head == 3)
                {
                    Ac_main.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '3'").FirstOrDefault();
                    account_no = Ac_main.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("3001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (Ac_main.a_head == 4)
                {
                    Ac_main.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '4'").FirstOrDefault();
                    account_no = Ac_main.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("4001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                if (Ac_main.a_head == 5)
                {
                    Ac_main.account_no = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '5'").FirstOrDefault();
                    account_no = Ac_main.account_no;
                    if (account_no == 0)
                    {
                        final_acc = Convert.ToInt32("5001");
                    }
                    else
                    {
                        final_acc = account_no + 1;
                    }
                }
                _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + final_acc + "," + Ac_main.a_head + ",N'" + Ac_main.a_title + "'," + final_acc + ",'0')");
                //var Ac_head_list = _context.Ac_head.ToList();
                //var Account_headVm = new Account_headVm
                //{
                //    Ac_head_list = Ac_head_list,
                //    Ac_main = Ac_main
                //};
                return RedirectToAction("Index");
            }
            else
            {
                //ViewData["Login"] = "This Account Title Already Exists !";
                //var Ac_head_list = _context.Ac_head.ToList();
                //var Account_headVm = new Account_headVm
                //{
                //    Ac_head_list = Ac_head_list,
                //    Ac_main = Ac_main
                //};
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Ac_main.SingleOrDefault(c => c.ID == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, Ac_main ac_Main)
        {
            _context.Database.ExecuteSqlCommand("Update Ac_main set a_title  = N'" + ac_Main.a_title + "' where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}