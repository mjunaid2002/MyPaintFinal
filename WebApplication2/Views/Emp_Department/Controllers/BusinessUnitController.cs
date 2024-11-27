using Antlr.Runtime.Tree;
using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class BusinessUnitController : Controller
    {
        private ApplicationDbContext _context;
        public BusinessUnitController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: BusinessUnit
        public ActionResult Index()
        {
            var list = _context.BusinessUnits.ToList();
            return View(list);
        }
        public ActionResult Create(BusinessUnit bus)
        {
            bus.Id = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from BusinessUnits").FirstOrDefault();
            return View(bus);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase file, BusinessUnit cat)
        {
            int b_unit = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(Id) +1, 0) as Id FROM BusinessUnits").FirstOrDefault();
            string ImageName = "";
            string physicalPath;

            if (file != null)
            {
                ImageName = System.IO.Path.GetFileName(file.FileName);
                physicalPath = Server.MapPath("~/Uploads/" + ImageName);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("insert into BusinessUnits (expiraydate,lang,Id,Name,Image,Email,Phone,Telephone,Address,Gst,Ntn) values('" + cat.expiraydate + "'," + cat.Lang + "," + cat.Id + ",N'" + cat.Name + "','" + ImageName + "',N'" + cat.Email + "',N'" + cat.Phone + "',N'" + cat.Telephone + "',N'" + cat.Address + "',N'" + cat.Gst + "',N'" + cat.Ntn + "')");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("insert into BusinessUnits (expiraydate,lang,Id,Name,Email,Phone,Telephone,Address,Gst,Ntn) values('" + cat.expiraydate + "'," + cat.Lang + "," + cat.Id + ",N'" + cat.Name + "',N'" + cat.Email + "',N'" + cat.Phone + "',N'" + cat.Telephone + "',N'" + cat.Address + "',N'" + cat.Gst + "',N'" + cat.Ntn + "')");
            }

            //First Level 

            int first_acc1 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '1' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (first_acc1 == 0)
            {
                first_acc1 = Convert.ToInt32("1001");
            }
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc1 + ",1,'Current Asset'," + first_acc1 + ",'" + b_unit + "' )");
            first_acc1 ++; 
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc1 + ",1,'Fixed Asset'," + first_acc1 + ",'" + b_unit + "')");

          
            int first_acc2 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '2' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (first_acc2 == 0)
            {
                first_acc2 = Convert.ToInt32("2001");
            }
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc2 + ",2,'Creditors'," + first_acc2 + ",'" + b_unit + "')");

            int first_acc3 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '3' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (first_acc3 == 0)
            {
                first_acc3 = Convert.ToInt32("3001");
            }
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc3 + ",3,'Capital'," + first_acc3 + ",'" + b_unit + "')");
            
            int first_acc4 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '4' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (first_acc4 == 0)
            {
                first_acc4 = Convert.ToInt32("4001");
            }
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc4 + ",4,'Revenue'," + first_acc4 + ",'" + b_unit + "')");

            int first_acc5 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(a_main_id), 0) as account_no FROM Ac_main where a_head = '5' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (first_acc5 == 0)
            {
                first_acc5 = Convert.ToInt32("5001");
            }
            _context.Database.ExecuteSqlCommand("insert into Ac_main values(" + first_acc5 + ",5,'Direct Expense'," + first_acc5 + ",'" + b_unit + "')");

            //Second Level

            int second_acc1 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '1' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (second_acc1 == 0)
            {
                second_acc1 = Convert.ToInt32("1000001");
            }
            _context.Database.ExecuteSqlCommand("insert into ac_second values(1, 1001 ,"+ second_acc1 + ",'Customers','" + b_unit + "')");
            second_acc1++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(1, 1001 ," + second_acc1 + ",'Bank','" + b_unit + "')");
            second_acc1++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(1, 1001 ," + second_acc1 + ",'Stock','" + b_unit + "')");
            second_acc1++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(1, 1002 ," + second_acc1 + ",'Building/Construnction/Furniture','" + b_unit + "')");
            second_acc1++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(1, 1001 ," + second_acc1 + ",'Cash','" + b_unit + "')");


            int second_acc2 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '2' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (second_acc2 == 0)
            {
                second_acc2 = Convert.ToInt32("2000001");
            }
            _context.Database.ExecuteSqlCommand("insert into ac_second values(2, 2001 ," + second_acc2 + ",'Supplier','" + b_unit + "')");
            second_acc2++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(2, 2001 ," + second_acc2 + ",'Employee','" + b_unit + "')");
            second_acc2++;
            _context.Database.ExecuteSqlCommand("insert into ac_second values(2, 2001 ," + second_acc2 + ",'Tax','" + b_unit + "')");

            int second_acc3 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '3' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (second_acc3 == 0)
            {
                second_acc3 = Convert.ToInt32("3000001");
            }
            _context.Database.ExecuteSqlCommand("insert into ac_second values(3, 3001 ," + second_acc3 + ",'Investor','" + b_unit + "')");

            int second_acc4 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '4' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (second_acc4 == 0)
            {
                second_acc4 = Convert.ToInt32("4000001");
            }
            _context.Database.ExecuteSqlCommand("insert into ac_second values(4, 4001 ," + second_acc4 + ",'Sales','" + b_unit + "')");

            int second_acc5 = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(account_no), 0) as account_no FROM ac_second where head_id = '5' and b_unit='" + b_unit + "'").FirstOrDefault();
            if (second_acc5 == 0)
            {
                second_acc5 = Convert.ToInt32("5000001");
            }
            _context.Database.ExecuteSqlCommand("insert into ac_second values(5, 5001 ," + second_acc5 + ",'Cost of Goods Sold','" + b_unit + "')");

            //ThirdLevel

            _context.Database.ExecuteSqlCommand("insert into accounttitles values('10000001', 1001,1,1000005,'Cash','Cash',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('10000003', 1001,1,1000001,'Cash Customer','Cash Customer',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('10000002', 1001,1,1000003,'Stock','Stock',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('2100001', 2001,2,2000001,'Cash Supplier','Cash Supplier',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('3100001', 3001,3,3000001,'Branch Name','Branch Name',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('4400001', 4001,4,4000001,'Sales','Sales',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('5500001', 5001,5,5000001,'CGS','Sales',0,0,'" + b_unit + "')");
             _context.Database.ExecuteSqlCommand("insert into accounttitles values('2100003', 2001,2001,2000003,'Tax Payable','Tax Payable',0,0,'" + b_unit + "')");

            decimal max_id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ID),0)+1 from Customers ").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into Customers values(" + max_id + ",N'Cash Customer','0','0',N'0','0','0',N'0',N'0',10000003,'0','0','0','0','0'," + b_unit + ",0,0,0,'0','0',0,0,'0','0',0,0,0)");

            decimal max_ids = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Suppliers").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into Suppliers values("+ max_ids + ",N'Cash Supplier','0','0',N'0','0','0',N'0',N'0',2100001,'0','0','0','0','0'," + b_unit + ",'0',0,0,'0','0',0,'0')");
            
            decimal max_idss = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Settings").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Settings]([ID] ,[CompanyName] ,[Email] ,[Phone] ,[Telephone] ,[Address] ,[MImage] ,[Gst] ,[Ntn] ,[SaleNote] ,[CNameStatus] ,[GstStatus] ,[NtnStatus] ,[SaleNoteStatus] ,[LogoStatus] ,[EmailStatus] ,[PhoneStatus] ,[TelephoneStatus] ,[Lang] ,[Bunit_status] ,[Str].[Bunit]) VALUES ("+ max_idss + ",'" + cat.Name + "','0','0','0','0','','0','0','0','0','0','0','0','0','0','0',0,0,'0','" + b_unit + "')");


            return RedirectToAction("Create");
        }
        public ActionResult Edit(BusinessUnit cat, int? ID)
        {
            var data = _context.BusinessUnits.SingleOrDefault(c => c.Id == ID);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int ID, BusinessUnit cat, HttpPostedFileBase file)
        {
            string ImageName = "";
            string physicalPath;

            if (file != null)
            {
                ImageName = System.IO.Path.GetFileName(file.FileName);
                physicalPath = Server.MapPath("/Uploads/" + ImageName);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("Update BusinessUnits set expiraydate='" + cat.expiraydate + "',lang=" + cat.Lang + ",Id =" + cat.Id + ",Name  = N'" + cat.Name + "',Image = '" + ImageName + "',email=N'" + cat.Email + "',phone=N'" + cat.Phone + "',Telephone=N'" + cat.Telephone + "',Address=N'" + cat.Address + "',Gst=N'" + cat.Gst + "',ntn=N'" + cat.Ntn + "' where Id = " + ID + "");
                return RedirectToAction("Index");
            }
            else
            {
                _context.Database.ExecuteSqlCommand("Update BusinessUnits set expiraydate='" + cat.expiraydate + "',lang="+cat.Lang+",Id =" + cat.Id + ",Name  = N'" + cat.Name + "',email=N'" + cat.Email + "',phone=N'" + cat.Phone + "',Telephone=N'" + cat.Telephone + "',Address=N'" + cat.Address + "',Gst=N'" + cat.Gst + "',ntn=N'" + cat.Ntn + "' where Id = " + ID + "");
                return RedirectToAction("Index");
            }
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From BusinessUnits where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}