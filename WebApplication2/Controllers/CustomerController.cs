using CRM.Models;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;


namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class CustomerController : Controller
    {
        private ApplicationDbContext _context;
        public CustomerController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Customer
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<Customers>("select * from  customers where discount =0 ").ToList();
            return View(list);
        }
        public ActionResult Create(Customers customers)
        {
            customers.customerid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(customerid),0)+1 from customers").FirstOrDefault();
            //var list = _context.Database.SqlQuery<Employee>("select * from  Employees").ToList();
            var list = _context.Database.SqlQuery<Acc_Empolyee>("select AccountNo as ID,AccountTitleName as Name  from  AccountTitles").ToList();
            var BeltArea = _context.Database.SqlQuery<BeltArea>("select  * from  BeltArea").ToList();
            ViewBag.employee = list;
            ViewBag.BeltArea = BeltArea;
            return View(customers);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(Customers customers)
        {
            //int account_no1;
            //Session["message"] = "";
            //customers.accno = _context.Database.SqlQuery<decimal>("SELECT ISNULL(MAX(account_title_id), 0) as account_no FROM tbl_account_title where account_head_id = '1'").FirstOrDefault();
            //if (customers.accno == 0)
            //{
            //    account_no1 = Convert.ToInt32("10000001");
            //}
            //else
            //{
            //    account_no1 = Convert.ToInt32(customers.accno + 1);
            //}
            //_context.Database.ExecuteSqlCommand("insert into tbl_account_title (account_title_id,account_head_id,account_title,account_type,opening_bal,dr,cr) values(" + account_no1 + ",1,'" + customers.Name + "','Customer',0,0,0)");


            int account_no1;
            customers.accno = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM AccountTitles where AccountHeadId = '1'").FirstOrDefault();
            if (customers.accno == 0)
            {
                account_no1 = Convert.ToInt32("10000003");
            }
            else
            {
                account_no1 = Convert.ToInt32(customers.accno + 1);
            }
            _context.Database.ExecuteSqlCommand("insert into AccountTitles (b_unit,AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel) values('0'," + account_no1 + ",1001,1,N'" + customers.Name + "','Customer',0,0,'1000001')");
            _context.Database.ExecuteSqlCommand("insert into customers (NTN,partywht,customerid,Name,address,Cityid,sectorid,Phone,mobile,email,discount,accno, BeltArea, employer,creditlimitamount,creditlimitdays) values('" + customers.NTN + "'," + customers.partywht + "," + customers.customerid + ",N'" + customers.Name + "',N'" + customers.Address + "',1,1,N'" + customers.Phone + "',N'" + customers.mobile + "',N'" + customers.email + "',0," + account_no1 + "," + customers.BeltArea + "," + customers.employer + "," + customers.creditlimitamount + "," + customers.creditlimitdays + ")");

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var cargo = _context.Database.SqlQuery<Customers>("select * from customers where customerid =" + ID + "").SingleOrDefault();
            //var list = _context.Database.SqlQuery<Employee>("select * from  Employees").ToList();
            var list = _context.Database.SqlQuery<Acc_Empolyee>("select AccountNo as ID,AccountTitleName as Name  from  AccountTitles").ToList();

            var BeltArea = _context.Database.SqlQuery<BeltArea>("select * from  BeltArea").ToList();
            ViewBag.employee = list;
            ViewBag.BeltArea = BeltArea;
            return View(cargo);
        }
        [HttpPost]
        public ActionResult Edit(Customers customers)
        {
            _context.Database.ExecuteSqlCommand("UPDATE  customers set NTN='" + customers.NTN + "',partywht=" + customers.partywht + ", Name='" + customers.Name + "',address='" + customers.Address + "',Phone='" + customers.Phone + "',email='" + customers.email + "',mobile='" + customers.mobile + "',BeltArea=" + customers.BeltArea + ",employer=" + customers.employer + "  ,creditlimitamount=" + customers.creditlimitamount + "  ,creditlimitdays=" + customers.creditlimitdays + "  where customerid = " + customers.customerid + "");
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set AccountTitleName = N'" + customers.Name + "' where AccountNo = " + customers.accno + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From customers where customerid =" + ID + "");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult WHTAction(string Customerid)
        {
            var getgrosspackage = _context.Database.SqlQuery<Customers>("SELECT TOP(1) partywht FROM   Customers where customerid="+Customerid).SingleOrDefault();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        //Delete From Categories
        //Delete From Brands
        //Delete From Customers
        //Delete From Suppliers
        //Delete From PurDetailDis
        //Delete From PurMasterDis
        //Delete From SaleDetailDis
        //Delete From SaleMasterDis
        //Delete From PurDetails
        //Delete From PurMasters
        //Delete From SaleDetails
        //Delete From SaleMasters
        //Delete From Ac_second
        //Delete From Item_legder
        //Delete From VoucherImages
        //Delete From Vouchers
        //Delete From VoucherImages
        //Delete From VoucherMasters
        //Delete From Employees
        //Delete From Products
        //Delete From Provinces
        //Delete From Towns
        //Delete From Cities
        //Delete From SaleMasters
        //Delete From SaleDetails
        //Delete From PurMasters
        //Delete From PurDetails
        //Delete From TransactionDetails
        //Delete From AccountTitles


    }
}