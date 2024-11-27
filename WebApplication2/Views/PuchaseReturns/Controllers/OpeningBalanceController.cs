using CRM.Models;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;
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
            var third_level = _context.Database.SqlQuery<OpeningBalanceQuery>("SELECT        OpeningBalance.b_unit,(SELECT Name  FROM   Sessions WHERE  (OpeningBalance.Session_id = Sessions.Id )) AS Name,(SELECT AccountTitleName  FROM   dbo.AccountTitles WHERE  (OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo )) AS AccountTitleName,OpeningBalance.AccountNo,OpeningBalance.Session_id ,SUM (OpeningBalance.Cr) as Cr,SUM (OpeningBalance.Dr) as Dr FROM            OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id where OpeningBalance.b_unit = " + Session["BusinessUnit"] + " Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit  ").ToList();
            return View(third_level);
        }
        public ActionResult Index(OpeningBalance openingBalance)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit='"+Session["BusinessUnit"] +"' ").ToList();
            var sess = _context.Database.SqlQuery<Session>("SELECT * FROM Sessions where b_unit ='" + Bunit + "'").ToList();
            ViewBag.SessionId= _context.Database.SqlQuery<int>("Select id from Sessions where year(enddate)= "+DateTime.Now.Year+"").FirstOrDefault();
            ViewBag.SessionName= _context.Database.SqlQuery<string>("Select Name from Sessions where year(enddate)= "+DateTime.Now.Year+"").FirstOrDefault();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                sess = sess,
                third_level = third_level,
                openingBalance = openingBalance,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpGet]
        public ActionResult ShowData(int message,int message1)
        {
            var lst = _context.Database.SqlQuery<OpeningBalance>("Select * From OpeningBalance where Session_id =" + message + " and AccountNo = " + message1 + "").ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult OpeningBalanceSubmit(OpeningBalance openingBalance)
        {
            _context.Database.ExecuteSqlCommand("Delete From OpeningBalance where Session_id ="+ openingBalance.Session_id + " and AccountNo = " + openingBalance.AccountNo + "");
            _context.Database.ExecuteSqlCommand("insert into OpeningBalance (AccountNo,Session_id,Cr,Dr,b_unit) values(" + openingBalance .AccountNo+ ","+ openingBalance.Session_id + "," + openingBalance.Cr+ ","+ openingBalance.Dr+ "," + Session["BusinessUnit"] + ")");
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set dr="+ openingBalance.Dr + " , cr = "+ openingBalance.Cr + " where AccountNo = "+ openingBalance.AccountNo + "");
            return RedirectToAction("Index");
        }
        }
}