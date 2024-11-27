using CRM.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ChartofAccountsController : Controller
    {
        private ApplicationDbContext _context;

        public ChartofAccountsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: ChartofAccounts
        public ActionResult Index()
        {
            //Assests
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var ac_head = _context.Database.SqlQuery<Ac_head>("SELECT * FROM Ac_head").ToList();
            var pro_list = _context.Database.SqlQuery<Ac_main>("SELECT * FROM Ac_main where b_unit ='" + Bunit + "' ").ToList();
            var sec_list = _context.Database.SqlQuery<Ac_second>("SELECT * FROM Ac_second where b_unit ='" + Bunit + "' ").ToList();
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles where b_unit ='"+ Bunit + "'").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                ac_head = ac_head,
                pro_list = pro_list,
                sec_list = sec_list,
                third_level = third_level,
            };
            return View(Chart_of_account_Vm);
        }
    }
}