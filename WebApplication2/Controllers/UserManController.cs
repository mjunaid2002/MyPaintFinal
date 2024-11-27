using CRM.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
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
            var role_list = _context.tbl_UserRole.ToList();
            var UserMan = new UserMan
            {
                role_list = role_list,
            };
            return View(UserMan);
        }
        public ActionResult IndexLogin()
        {
            //var role_list = _context.Database.SqlQuery<UserRoleView>("SELECT ul.id , ul.UserName, ul.EmpId, ul.Password, ur.name AS role, Employees.name AS EmployeeName FROM UserLogins AS ul LEFT OUTER JOIN Employees ON ul.EmpId = Employees.ID LEFT OUTER JOIN UserRoles AS ur ON ul.roleid = ur.Id").ToList();
            var role_list = _context.Database.SqlQuery<UserRoleView>("SELECT ul.Id, ul.UserName, ul.EmpId, ul.Password, ur.name AS role, Employees.name AS EmployeeName, ISNULL(Emp_Department.Name, '') AS dep_id, ISNULL ((SELECT name AS Expr1 FROM Employees AS E2 WHERE (ID = ul.teamlead_id)), '') AS teamLead_id FROM UserLogins AS ul LEFT OUTER JOIN Emp_Department ON ul.dep_id = Emp_Department.ID LEFT OUTER JOIN Employees ON ul.EmpId = Employees.ID LEFT OUTER JOIN UserRoles AS ur ON ul.roleid = ur.Id").ToList();
            var UserMan = new UserMan
            {
                emp_list = _context.Employee.ToList(),
                Userlogin_list = role_list,
            };
            return View(UserMan);
        }

        public ActionResult Create(UserLogin UserLogin)
        {
            //var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var emp_list = _context.Employee.ToList();
            var role_list = _context.tbl_UserRole.ToList();
            var UserMan = new UserMan
            {
                role_list = role_list,
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
                for (int i = 0; i < aListItems.Count(); i++)
                {
                    if (Convert.ToInt32(aListItems[i]) <= Convert.ToInt32("1000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (" + value + ",0,0," + empid + ")");
                    }
                    else if (Convert.ToInt32(aListItems[i]) > Convert.ToInt32("1000") & Convert.ToInt32(aListItems[i]) < Convert.ToInt32("1000000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (0," + value + ",0," + empid + ")");
                    }
                    else if (Convert.ToInt32(aListItems[i]) > Convert.ToInt32("1000000"))
                    {
                        var value = aListItems[i];
                        _context.Database.ExecuteSqlCommand("INSERT INTO UserAccesses values (0,0," + value + "," + empid + ")");
                    }
                }
            }
            return RedirectToAction("Index");
            //return Json(insertedRecords);
        }
        [HttpPost]
        public ActionResult EditGetJavaScriptString(string ChildNodes, string empid)
        {
            string abc = ChildNodes.Replace("%2C", ",");
            string[] aListItems = abc.Split(',');
            var count = aListItems.Count();

            if (aListItems != null)
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM UserAccesses where EmpId = " + empid + "");
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
        }
        public ActionResult CreateRole(UserRole UserRole)
        {
            return View(UserRole);
        }
        public ActionResult RoleIndex()
        {
            var list = _context.tbl_UserRole.ToList();
            return View(list);
        }


        [HttpPost]
        public ActionResult SaveRole(UserRole cat)
        {
            if(cat.Id==0)
                _context.Database.ExecuteSqlCommand("insert into userroles (Name,b_unit) values(N'" + cat.name + "','" + Session["BusinessUnit"] + "')");
            else
                _context.Database.ExecuteSqlCommand("update userroles set Name=N'" + cat.name + "' where Id=" + cat.Id);

            return RedirectToAction("CreateRole");
        }
        public ActionResult AssignRole(UserLogin UserLogin)
        {
            var emp_list = _context.Employee.ToList();
            var role_list = _context.tbl_UserRole.ToList();
            var department_list = _context.Emp_Department.ToList();
            var teamlead_list = _context.Employee.ToList();
           
            var UserMan = new UserMan
            {   
                teamlead_list= teamlead_list,
                dep_list= department_list,
                role_list = role_list,
                emp_list = emp_list,
                UserLogin = UserLogin
            };
            return View(UserMan);
        }
        public ActionResult EditAssignRole(int id)
        {
           var UserLogin=_context.UserLogin.Where(x => x.Id == id).SingleOrDefault();
           var dep_id = _context.UserLogin.Where(x => x.Id == id).Select(x=>x.dep_id).SingleOrDefault();
           var teamlead_id = _context.UserLogin.Where(x => x.Id == id).Select(x=>x.teamlead_id).SingleOrDefault();
            var emp_list = _context.Employee.Where(x => x.emp_id == teamlead_id).ToList();
            var role_list = _context.tbl_UserRole.ToList();
            var department_list = _context.Emp_Department.ToList();
            var teamlead_list = _context.Employee.Where(x=>x.dep_id== dep_id).ToList();
            var UserMan = new UserMan
            {
                teamlead_list = teamlead_list,
                dep_list = department_list,
                role_list = role_list,
                emp_list = emp_list,
                UserLogin = UserLogin,
            };
            return View(UserMan);
        }
        [HttpPost]
       // public ActionResult SaveEditAssignRole(UserLogin userLogin)
        public ActionResult SaveEditAssignRole(UserMan UserMan)
        {
            _context.Database.ExecuteSqlCommand("UPDATE  UserLogins SET  UserName ='" + UserMan.UserLogin.UserName + "', EmpId =" + UserMan.UserLogin.EmpId + ", Password ='" + UserMan.UserLogin.Password + "', roleid =" + UserMan.UserLogin.roleid + ", dep_id =" + UserMan.UserLogin.dep_id + ", teamlead_id =" + UserMan.UserLogin.teamlead_id + "  where id= " + UserMan.UserLogin.Id);
           // _context.Database.ExecuteSqlCommand("INSERT INTO UserLogins values ('" + userLogin.UserName + "'," + userLogin.EmpId + ",'" + userLogin.Password + "'," + userLogin.StoreID + ",'" + Session["BusinessUnit"] + "','0'," + userLogin.roleid + ",1)");

            return RedirectToAction("IndexLogin");
        }
        public ActionResult Edit(int id)
        {
            var list = _context.tbl_UserRole.Where(x=>x.Id==id).SingleOrDefault();
            return RedirectToAction("CreateRole", list);
        }
        public ActionResult Edits(int? ID, UserLogin UserLogin)
        {
            var role_list = _context.tbl_UserRole.ToList();
            var Roles = _context.tbl_UserRole.SingleOrDefault(c => c.Id == ID);
            var UserAccess = _context.UserAccess.Where(z => z.EmpId == ID).ToList();
            var UserMan = new UserMan
            {
                UserAccess = UserAccess,
                role_list = role_list,
                Roles = Roles
            };
            return View(UserMan);
        }

        public ActionResult UserList()
        {
            var list = _context.UserLogin.ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult SaveUser(UserLogin userLogin)
        {
            int user_login = _context.Database.SqlQuery<int>("SELECT count(*) from  UserLogins where UserName='" + userLogin.UserName + "' ").FirstOrDefault();
            if (user_login == 0)
            {
                _context.Database.ExecuteSqlCommand("DELETE FROM UserLogins where EmpId = " + userLogin.EmpId + "");
                _context.Database.ExecuteSqlCommand("INSERT INTO UserLogins values ('" + userLogin.UserName + "'," + userLogin.EmpId + ",'" + userLogin.Password + "'," + userLogin.StoreID + ",'" + Session["BusinessUnit"] + "','0'," + userLogin.roleid + ",1,"+userLogin.dep_id+","+userLogin.teamlead_id+")");
            }
            else
            {
                return RedirectToAction("Create", "RegisterUsers", new { ac = "fail" });
            }
                return RedirectToAction("AssignRole");
        }

        public ActionResult UserLogHistory()
        {
            return View(_context.Employee.ToList());
        }

        public ActionResult LogSearch(string empid) 
        {
            
            var StartDate = Convert.ToDateTime(Request["log_s_date"]);
            var Enddate = Convert.ToDateTime(Request["log_e_date"]);

            if (empid == "" || empid == null)
                empid = "";
            else
            {
                decimal emppid = Convert.ToDecimal(empid);
                empid = " and CurrentUser='" + _context.UserLogin.Where(x => x.EmpId == emppid).SingleOrDefault().UserName + "' ";

            }

            var userLogHistories = _context.Database.SqlQuery<UserLogHistory>("select * from tblLogHistory where date between '" + StartDate + "' and '" + Enddate + "'" + empid).ToList();

            var UserManVM = new UserMan
            {
                emp_list = _context.Employee.ToList(),
                userLogHistories = userLogHistories
            };

            return View(UserManVM);
        }

        [HttpGet]
        public ActionResult GetTeamLeads(int dep_id)
        {
            var teamLeads = _context.Employee.Where(x=>x.dep_id==dep_id && x.ID==x.emp_id).Select(x => new { x.ID, x.Name }).ToList();
            return Json(teamLeads, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetEmployees(int teamlead_id)
        {
            var employees = _context.Employee.Where(x => x.emp_id == teamlead_id).Select(x => new { x.ID, x.Name }).ToList();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
    }
}