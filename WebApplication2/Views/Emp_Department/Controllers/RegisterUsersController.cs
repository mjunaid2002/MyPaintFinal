using CRM.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using WebApplication2.ViewModels;
namespace WebApplication2.Controllers
{
    public class RegisterUsersController : Controller
    {
        private ApplicationDbContext _context;
        public RegisterUsersController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: RegisterUsers
        public ActionResult Index()
        {
            var list = _context.UserLogin.ToList();
            return View(list);
        }
        public ActionResult Create(UserLogin UserLogin)
        {
            var emp_list = _context.AccountTitle.ToList();
            var b_unit = _context.BusinessUnits.ToList();
            var UserMan = new UserMan
            {
                b_unit = b_unit,
              //  emp_list = emp_list,
                UserLogin = UserLogin
            };
            return View(UserMan);
        }
        [HttpPost]
        public ActionResult Create(UserMan userMan, UserLogin UserLogin)
        {
            int user_login = _context.Database.SqlQuery<int>("SELECT count(*) from  UserLogins where UserName='"+ userMan.UserLogin.UserName + "' ").FirstOrDefault();
           // var user_login = _context.UserLogin.SingleOrDefault(c => c.UserName == userMan.UserLogin.UserName);
            if (user_login == 0)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO UserLogins values ('" + userMan.UserLogin.UserName + "',1,'" + userMan.UserLogin.Password + "'," + userMan.UserLogin.StoreID + ",'" + userMan.UserLogin.b_unit + "','0',1)");
            }
            else
            {
                return RedirectToAction("Create", "RegisterUsers", new { ac = "fail" });
            }
            return RedirectToAction("Index");
            //return Json(insertedRecords);
        }
        public ActionResult Edit(UserLogin cat, int? ID)
        {
            var data = _context.UserLogin.SingleOrDefault(c => c.Id == ID);
            var emp_list = _context.AccountTitle.ToList();
            var b_unit = _context.BusinessUnits.ToList();
            var UserMan = new UserMan
            {
                b_unit = b_unit,
             //   emp_list = emp_list,
                UserLogin = data

            };
            return View(UserMan);
        }
        [HttpPost]
        public ActionResult Update(UserMan userMan)
        {
            _context.Database.ExecuteSqlCommand("Update UserLogins set  UserName='" + userMan.UserLogin.UserName + "',Password='" + userMan.UserLogin.Password + "',b_unit='" + userMan.UserLogin.b_unit + "' where id="+ userMan.UserLogin.Id + "");
            return RedirectToAction("Index");
        }
        
    }
}