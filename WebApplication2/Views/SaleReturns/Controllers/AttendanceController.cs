using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication2.ViewModels;
using WebApplication1.QueryViewModel;

using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class AttendanceController : Controller
    {
        private ApplicationDbContext _context;
        
        public AttendanceController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Attendance
        public ActionResult Create(Attendance attendance)
        {
            var emp_list = _context.Employee.ToList();
            var VoucherVM = new VoucherVM
            {
                emp_list = emp_list,
                attendance = attendance,
            };
            return View(VoucherVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(Attendance attendance, int status,decimal[] emp_id,DateTime[] check_in, DateTime[] check_out)
        {
            for (Int32 i = 0; i < emp_id.Count(); i++)
            {
                if (status == 1)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO Attendance (EmpId,checkin_datetime,Status) VALUES (" + emp_id[i] + ",'" + check_in[i] + "'," + status + ")");
                }
                else if (status == 2)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO Attendance (EmpId,checkout_datetime,Status) VALUES (" + emp_id[i] + ",'" + check_out[i] + "'," + status + ")");
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO Attendance (EmpId,checkin_datetime,checkout_datetime,Status) VALUES (" + emp_id[i] + ",'" + check_in[i] + "','" + check_out[i] + "'," + status + ")");
                }
            }
            return RedirectToAction("Report");
        }
        public ActionResult Report(Attendance attendance)
        {
            var atte_list = _context.Database.SqlQuery<AttendanceQuery>("SELECT (Select name From Employees where Id=Attendance.EmpId) as EmpName,ISNULL((checkin_datetime),0) as checkin_datetime ,ISNULL((checkout_datetime),0) as checkout_datetime,Status FROM Attendance").ToList();
            var emp_list = _context.Employee.ToList();
            var VoucherVM = new VoucherVM
            {
                emp_list = emp_list,
                atte_list = atte_list,
            };
            return View(VoucherVM);

        }
       

        [HttpPost]
        public ActionResult AttenReportSearch(Attendance attendance)
        {

            if (attendance.EmpId == 0)
            {
                var atte_list = _context.Database.SqlQuery<AttendanceQuery>("select (Select name From Employees where Id= dtatt.empid) as EmpName,Attdate,( select top(1) CONVERT(time, checkin_datetime) from hclpvtlt_hclnew.attendance where checkin_datetime is not null and CONVERT(date, checkin_datetime) = dtatt.Attdate ) as STIN,(select top(1) CONVERT(time, checkout_datetime) from hclpvtlt_hclnew.attendance where  checkout_datetime is not null and CONVERT(date, checkout_datetime)= dtatt.Attdate) as STOUT from( select  empid,CONVERT(date, checkin_datetime) as Attdate from hclpvtlt_hclnew.attendance where  (CONVERT(date, checkin_datetime) Between '" + attendance.checkin_datetime + "' and  '" + attendance.checkout_datetime + "' ) OR(CONVERT(date, checkout_datetime) Between '" + attendance.checkin_datetime + "' and  '" + attendance.checkout_datetime + "') and checkin_datetime is not null) as dtatt group by Attdate,empid").ToList();
                var emp_list = _context.Employee.ToList();
                var VoucherVM = new VoucherVM
                {
                    emp_list = emp_list,
                    atte_list = atte_list,
                };
                return View(VoucherVM);
            }
            else if (attendance.EmpId != 0)
            {
                var atte_list = _context.Database.SqlQuery<AttendanceQuery>("select (Select name From Employees where Id= dtatt.empid) as EmpName,Attdate,( select top(1) CONVERT(time, checkin_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId + "  and checkin_datetime is not null and CONVERT(date, checkin_datetime) = dtatt.Attdate ) as STIN,(select top(1) CONVERT(time, checkout_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId + " and checkout_datetime is not null and CONVERT(date, checkout_datetime)= dtatt.Attdate) as STOUT from( select empid,CONVERT(date, checkin_datetime) as Attdate from hclpvtlt_hclnew.attendance where empid= " + attendance.EmpId + " and ((CONVERT(date, checkin_datetime) Between '" + attendance.checkin_datetime + "' and  '" + attendance.checkout_datetime + "' ) OR(CONVERT(date, checkout_datetime) Between '" + attendance.checkin_datetime + "' and  '" + attendance.checkout_datetime + "')) and checkin_datetime is not null) as dtatt group by Attdate,empid ").ToList();
                var emp_list = _context.Employee.ToList();
                var VoucherVM = new VoucherVM
                {
                    emp_list = emp_list,
                    atte_list = atte_list,
                };
                return View(VoucherVM);
            }
            else
            {
                return View();
            }
        }
        public ActionResult Daily(Attendance attendance)
        {
            var atte_list = _context.Database.SqlQuery<AttendanceQuery>("SELECT (Select name From Employees where Id=Attendance.EmpId) as EmpName,ISNULL((checkin_datetime),0) as checkin_datetime ,ISNULL((checkout_datetime),0) as checkout_datetime,Status FROM Attendance").ToList();
            var emp_list = _context.Employee.ToList();
            var VoucherVM = new VoucherVM
            {
                emp_list = emp_list,
                atte_list = atte_list,
            };
            return View(VoucherVM);

        }
        [HttpPost]
        public ActionResult DailyAttensearch(Attendance attendance)
        {

            if (attendance.EmpId == 0)
            {
                var atte_list = _context.Database.SqlQuery<AttendanceQuery>("select * from (select (Select name From Employees where Id= dtatt.empid) as EmpName,Attdate,( select top(1) CONVERT(time, checkin_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId + "  and checkin_datetime is not null and CONVERT(date, checkin_datetime)  = dtatt.Attdate ) as STIN,(select top(1) CONVERT(time, checkout_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId + " and checkout_datetime is not null and CONVERT(date, checkout_datetime)= dtatt.Attdate) as STOUT from( select empid,CONVERT(date, checkin_datetime) as Attdate from hclpvtlt_hclnew.attendance where empid= "+attendance.EmpId+"  and checkin_datetime is not null) as dtatt group by Attdate,empid) as dt1").ToList();
                var emp_list = _context.Employee.ToList();
                var VoucherVM = new VoucherVM
                {
                    emp_list = emp_list,
                    atte_list = atte_list,
                };
                return View(VoucherVM);

            }
            else if (attendance.EmpId != 0)
            {
                var atte_list = _context.Database.SqlQuery<AttendanceQuery>("select * from (select (Select name From Employees where Id= dtatt.empid) as EmpName,Attdate,( select top(1) CONVERT(time, checkin_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId + "  and checkin_datetime is not null and CONVERT(date, checkin_datetime)  = dtatt.Attdate ) as STIN,(select top(1) CONVERT(time, checkout_datetime) from hclpvtlt_hclnew.attendance where empid = " + attendance.EmpId+ " and checkout_datetime is not null and CONVERT(date, checkout_datetime)= dtatt.Attdate) as STOUT from( select empid,CONVERT(date, checkin_datetime) as Attdate from hclpvtlt_hclnew.attendance where empid= " + attendance.EmpId + "  and checkin_datetime is not null) as dtatt group by Attdate,empid) as dt1 where attdate='" + attendance.checkin_datetime+"'").ToList();
                var emp_list = _context.Employee.ToList();
                var VoucherVM = new VoucherVM
                {
                    emp_list = emp_list,
                    atte_list = atte_list,
                };
                return View(VoucherVM);
            }
            else
            {
                return View();
            }
        }

    }
}