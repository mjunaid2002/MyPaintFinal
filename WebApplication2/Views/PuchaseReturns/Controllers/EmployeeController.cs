using CRM.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

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
            Employee.ID = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Employees ").FirstOrDefault();
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
                _context.Database.ExecuteSqlCommand("insert into Employees values(" + Employee.ID + ",N'" + Employee.Name + "','" + Employee.Email + "',N'" + Employee.Address + "','" + img + "','" + Employee.Phone + "',"+ account_no1 + ","+ Session["BusinessUnit"] + "," + Employee.mon_sal + "," + Employee.dal_sal + ",'" + Employee.joiningdate + "'," + Employee.dai_insentive + "," + Employee.cnic + "," + Employee.mon_insentive + ")");
                _context.Database.ExecuteSqlCommand("insert into AccountTitles (AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel,b_unit) values(" + account_no1 + ",2001,2,N'" + Employee.Name + "','Employees',0,0,'2000002'," + Session["BusinessUnit"] + ")");
                return RedirectToAction("Create");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("insert into Employees values(" + Employee.ID + ",N'" + Employee.Name + "','" + Employee.Email + "',N'" + Employee.Address + "','0','" + Employee.Phone + "'," + account_no1 + "," + Session["BusinessUnit"] + "," + Employee.mon_sal + "," + Employee.dal_sal + ",'" + Employee.joiningdate + "'," + Employee.dai_insentive + "," + Employee.cnic + "," + Employee.mon_insentive + ")");
                _context.Database.ExecuteSqlCommand("insert into AccountTitles (AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel,b_unit) values(" + account_no1 + ",2001,2,N'" + Employee.Name + "','Employees',0,0,'2000002'," + Session["BusinessUnit"] + ")");
                return RedirectToAction("Create");
            }

        }
        public ActionResult Edit(int? ID)
        {
            var data = _context.Employee.SingleOrDefault(c => c.ID == ID);
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
                _context.Database.ExecuteSqlCommand("Update Employees set Name=N'" + Employee.Name + "',Image='" + img + "',Email='" + Employee.Email + "',Phone='" + Employee.Phone + "',Address=N'" + Employee.Address + "',mon_sal=" + Employee.mon_sal + ",dal_sal=" + Employee.dal_sal + ",joiningdate='" + Employee.joiningdate + "',dai_insentive=" + Employee.dai_insentive + ",cnic=" + Employee.cnic + ",mon_insentive=" + Employee.mon_insentive + " where ID =" + ID+"");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("Update Employees set Name=N'" + Employee.Name + "',Email='" + Employee.Email + "',Phone='" + Employee.Phone + "',Address=N'" + Employee.Address + "',mon_sal=" + Employee.mon_sal + ",dal_sal=" + Employee.dal_sal + ",joiningdate='" + Employee.joiningdate + "',dai_insentive=" + Employee.dai_insentive + ",cnic=" + Employee.cnic + ",mon_insentive=" + Employee.mon_insentive + " where ID =" + ID + "");
             }
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set AccountTitleName = N'" + Employee.Name + "' where AccountNo = " + Employee .AccountNo+ "");
            return RedirectToAction("Index");


        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Employees where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}