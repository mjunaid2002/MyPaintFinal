using CRM.Models;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class OpeningBalanceController : Controller
    {
        private ApplicationDbContext _context;
        public OpeningBalanceController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult List(OpeningBalance openingBalance)
        {
            var third_level = _context.Database.SqlQuery<OpeningBalanceQuery>("SELECT        (SELECT AccountTitleName  FROM   dbo.AccountTitles WHERE  (OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo )) AS AccountTitleName,OpeningBalance.AccountNo,OpeningBalance.date As date ,OpeningBalance.narration as narration ,SUM (OpeningBalance.Cr) as Cr,SUM (OpeningBalance.Dr) as Dr FROM            OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo Group by OpeningBalance.AccountNo,OpeningBalance.date,OpeningBalance.narration  ").ToList();
            return View(third_level);
        }
        public ActionResult Index(OpeningBalance openingBalance)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles ").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                openingBalance = openingBalance,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpGet]
        public ActionResult ShowData(int message, int message1)
        {
            var lst = _context.Database.SqlQuery<OpeningBalance>("Select * From OpeningBalance where Session_id =" + message + " and AccountNo = " + message1 + "").ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OpeningBalanceSubmit(OpeningBalance openingBalance)
        {
            _context.Database.ExecuteSqlCommand("Delete From OpeningBalance where AccountNo = " + openingBalance.AccountNo + "");
            _context.Database.ExecuteSqlCommand("insert into OpeningBalance (AccountNo,Cr,Dr,date,narration) values(" + openingBalance.AccountNo + "," + openingBalance.Cr + "," + openingBalance.Dr + ",'" + openingBalance.date + "','" + openingBalance.narration + "')");
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set dr=" + openingBalance.Dr + " , cr = " + openingBalance.Cr + " where AccountNo = " + openingBalance.AccountNo + "");
            return RedirectToAction("List");
        }


        //public ActionResult Edit(int ID)
        //{
        //    var Bunit = Convert.ToString(Session["BusinessUnit"]);
        //    var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles").ToList();
        //    var openingBalance = _context.Database.SqlQuery<OpeningBalance>("SELECT * FROM OpeningBalance where id=" + ID).FirstOrDefault();
        //    var Chart_of_account_Vm = new ChartofaccountVm
        //    {
        //        third_level = third_level,
        //        openingBalance = openingBalance,
        //    };
        //    return View(Chart_of_account_Vm);
        //}
        //[HttpPost]
        //public ActionResult OpeningBalanceUpdate(OpeningBalance openingBalance)
        //{
        //    //_context.Database.ExecuteSqlCommand("Delete From OpeningBalance where AccountNo = " + openingBalance.AccountNo + "");
        //    _context.Database.ExecuteSqlCommand("Update  OpeningBalance set AccountNo='" + openingBalance.AccountNo + "',Cr='" + openingBalance.Cr + "',Dr='" + openingBalance.Dr + "',date='" + openingBalance.date + "',narration='" + openingBalance.narration + "'  where AccountNo=" + openingBalance.AccountNo);
        //    _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set dr=" + openingBalance.Dr + " , cr = " + openingBalance.Cr + " where AccountNo = " + openingBalance.AccountNo + "");
        //    return RedirectToAction("List");
        //}
    }
}