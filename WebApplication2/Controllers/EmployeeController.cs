using CRM.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class EmployeeController : Controller
    {
        private ApplicationDbContext _context;
        public EmployeeController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Category
        public ActionResult Index()
        {
            var Bunit = Convert.ToDecimal(Session["BusinessUnit"]);
            var list = _context.Employee.Where(z => z.BusinessUnit == Bunit).ToList();
            return View(list);
        }
        public ActionResult Create(Employee Employee)
        {
            var Bunit = Convert.ToDecimal(Session["BusinessUnit"]);
            Employee.ID = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Employees ").FirstOrDefault();
            Employee.Branchlist = _context.Database.SqlQuery<Branch>("select id,name from Branch ").ToList();
            Employee.departmentlist = _context.Database.SqlQuery<Emp_Department>("select * from Emp_Department ").ToList();
            Employee.Employeelist = _context.Database.SqlQuery<Employee>("select * from Employees where BusinessUnit = '"+Bunit+"' "  ).ToList();
            Employee.joiningdate = DateTime.Now;
            return View(Employee);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase file, Employee Employee)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            int account_no1;
            TempData["succ"] = "Record Saved Successfully";
            Employee.AccountNo = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM AccountTitles where AccountHeadId = '2'").FirstOrDefault();
            if (Employee.AccountNo == 0)
            {
                account_no1 = Convert.ToInt32("200001");
            }
            else
            {
                account_no1 = Convert.ToInt32(Employee.AccountNo + 1);
            }

            if (file != null)
            {
                ImageName = System.IO.Path.GetFileName(file.FileName);
                string img = num + ImageName;
                physicalPath = Server.MapPath("~/Uploads/" + img);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("insert into Employees values(" + Employee.ID + ",N'" + Employee.Name + "','" + Employee.Email + "',N'" + Employee.Address + "','" + img + "','" + Employee.Phone + "',"+ account_no1 + ",0," + Employee.mon_sal + "," + Employee.dal_sal + ",'" + Employee.joiningdate + "'," + Employee.dai_insentive + "," + Employee.cnic + "," + Employee.mon_insentive + "," + Employee.dep_id + "," + Employee.emp_id + ")");
                _context.Database.ExecuteSqlCommand("insert into AccountTitles (AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel,b_unit) values(" + account_no1 + ",2001,2,N'" + Employee.Name + "','Employees',0,0,'2000002'," + Session["BusinessUnit"] + ")");
               
            }
            else
            {
                _context.Database.ExecuteSqlCommand("insert into Employees values(" + Employee.ID + ",N'" + Employee.Name + "','" + Employee.Email + "',N'" + Employee.Address + "','0','" + Employee.Phone + "'," + account_no1 + ",0," + Employee.mon_sal + "," + Employee.dal_sal + ",'" + Employee.joiningdate + "'," + Employee.dai_insentive + "," + Employee.cnic + "," + Employee.mon_insentive + ","+Employee.dep_id+","+Employee.emp_id+","+Employee.branch_id + ")");
                _context.Database.ExecuteSqlCommand("insert into AccountTitles (AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel,b_unit) values(" + account_no1 + ",2001,2,N'" + Employee.Name + "','Employees',0,0,'2000002',0)");

            }
            if (Employee.Branchid_list != null && Employee.Branchid_list.Any())
            {
                Employee.ID = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(ID),1)  FROM Employees").FirstOrDefault();

                foreach (var branchId in Employee.Branchid_list)
                {
                    _context.Database.ExecuteSqlCommand("insert into EmployeeBranch values(" + Employee.ID + ",'" + branchId + "')");

                }
            }
            return RedirectToAction("Create");

        }
        public ActionResult Edit(int? ID)
        {
            var Bunit = Convert.ToDecimal(Session["BusinessUnit"]);
            var data = _context.Employee.SingleOrDefault(c => c.ID == ID);
            data.Branchlist = _context.Database.SqlQuery<Branch>("select id,name from Branch ").ToList();

            data.departmentlist = _context.Database.SqlQuery<Emp_Department>("select * from Emp_Department ").ToList();
            data.Branchid_list = _context.Database.SqlQuery<int>("select branchid from EmployeeBranch where emp_id= "+ID).ToList();
            data.Employeelist = _context.Database.SqlQuery<Employee>("select * from Employees where BusinessUnit = '" + Bunit + "' ").ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, Employee Employee, HttpPostedFileBase file)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;

            if (file != null)
            {

                ImageName = System.IO.Path.GetFileName(file.FileName);
                string img = num + ImageName;
                physicalPath = Server.MapPath("~/Uploads/" + img);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("Update Employees set Name=N'" + Employee.Name + "',Image='" + img + "',Email='" + Employee.Email + "',Phone='" + Employee.Phone + "',Address=N'" + Employee.Address + "',mon_sal=" + Employee.mon_sal + ",dal_sal=" + Employee.dal_sal + ",joiningdate='" + Employee.joiningdate + "',dai_insentive=" + Employee.dai_insentive + ",cnic=" + Employee.cnic + ",mon_insentive=" + Employee.mon_insentive + ", dep_id ="+Employee.dep_id+", emp_id ="+Employee.emp_id+ ", branch_id =" + Employee.branch_id + " where ID =" + ID+"");
                
            }
            else
            {
                _context.Database.ExecuteSqlCommand("Update Employees set Name=N'" + Employee.Name + "',Email='" + Employee.Email + "',Phone='" + Employee.Phone + "',Address=N'" + Employee.Address + "',mon_sal=" + Employee.mon_sal + ",dal_sal=" + Employee.dal_sal + ",joiningdate='" + Employee.joiningdate + "',dai_insentive=" + Employee.dai_insentive + ",cnic=" + Employee.cnic + ",mon_insentive=" + Employee.mon_insentive + " , dep_id =" + Employee.dep_id + ", emp_id =" + Employee.emp_id + " , branch_id =" + Employee.branch_id + " where ID =" + ID + "");
             }
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set AccountTitleName = N'" + Employee.Name + "' where AccountNo = " + Employee .AccountNo+ "");
            if (Employee.Branchid_list != null && Employee.Branchid_list.Any())
            {
                _context.Database.ExecuteSqlCommand("Delete from EmployeeBranch where emp_id="+ Employee.ID);

                foreach (var branchId in Employee.Branchid_list)
                {
                    _context.Database.ExecuteSqlCommand("insert into EmployeeBranch values(" + Employee.ID + ",'" + branchId + "')");

                }
            }
            return RedirectToAction("Index");


        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Employees where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}