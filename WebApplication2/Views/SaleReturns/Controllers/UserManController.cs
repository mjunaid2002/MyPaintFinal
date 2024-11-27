using CRM.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class UserManController : Controller
    {
        private ApplicationDbContext _context;
        public UserManController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: UserMan
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult Create(UserLogin UserLogin)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var emp_list = _context.Employee.ToList();
            var store_list = _context.Store.ToList();
            var role_list = _context.tbl_UserRole.Where(z => z.b_unit == Bunit).ToList();
            var UserMan = new UserMan
            {
                role_list = role_list,
                store_list = store_list,
                emp_list = emp_list,
                UserLogin = UserLogin

            };
            return View(UserMan);
        }
        [HttpPost]
        public ActionResult GetJavaScriptString(string ChildNodes, string empid)
        {
            string[] aListItems = ChildNodes.Split(',');
            var count = aListItems.Count();

            if (aListItems != null)
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM UserAccesses where EmpId = " + empid + "");
               // _context.Database.ExecuteSqlCommand("DELETE FROM UserLogins where EmpId = " + empid + "");
               // _context.Database.ExecuteSqlCommand("INSERT INTO UserLogins values ('" + username + "'," + empid + ",'" + pass + "'," + store_id + ",'" + Session["BusinessUnit"] + "','0')");
                for (int i = 0; i < aListItems.Count(); i++)
                {
                    if (Convert.ToInt32(aListItems[i]) <= Convert.ToInt32("1000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (" + value + ",0,0," + empid + ")");

                    }
                    else if (Convert.ToInt32(aListItems[i]) > Convert.ToInt32("1000") & Convert.ToInt32(aListItems[i]) < Convert.ToInt32("100000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (0," + value + ",0," + empid + ")");
                    }
                    else if (Convert.ToInt32(aListItems[i]) > Convert.ToInt32("100000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (0,0," + value + "," + empid + ")");
                    }
                }

            }
            return RedirectToAction("Index");
            //return Json(insertedRecords);
        }
        public ActionResult CreateRole(UserRole UserRole)
        {
            return View(UserRole);
        }
        public ActionResult RoleIndex()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.tbl_UserRole.Where(z => z.b_unit == Bunit).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult SaveRole(UserRole cat)
        {
            _context.Database.ExecuteSqlCommand("insert into userroles (Name,b_unit) values(N'" + cat.name + "','" + Session["BusinessUnit"] + "')");
            return RedirectToAction("CreateRole");
        }
        public ActionResult AssignRole(UserLogin UserLogin)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var emp_list = _context.Employee.ToList();
            var store_list = _context.Store.ToList();
            var role_list = _context.tbl_UserRole.Where(z => z.b_unit == Bunit).ToList();
            var UserMan = new UserMan
            {
                role_list = role_list,
                store_list = store_list,
                emp_list = emp_list,
                UserLogin = UserLogin
            };
            return View(UserMan);
        }
        [HttpPost]
        public ActionResult SaveUser(UserLogin userLogin)
        {
            int user_login = _context.Database.SqlQuery<int>("SELECT count(*) from  UserLogins where UserName='" + userLogin.UserName + "' ").FirstOrDefault();
            // var user_login = _context.UserLogin.SingleOrDefault(c => c.UserName == userMan.UserLogin.UserName);
            if (user_login == 0)
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM UserLogins where EmpId = " + userLogin.EmpId + "");
                _context.Database.ExecuteSqlCommand("INSERT INTO UserLogins values ('" + userLogin.UserName + "'," + userLogin.roleid + ",'" + userLogin.Password + "'," + userLogin.StoreID + ",'" + Session["BusinessUnit"] + "','0'," + userLogin.roleid + ")");
            }
            else
            {
                return RedirectToAction("Create", "RegisterUsers", new { ac = "fail" });
            }
                return RedirectToAction("AssignRole");
        }

    }
}