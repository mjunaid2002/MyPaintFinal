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
    public class SupplierController : Controller
    {
        private ApplicationDbContext _context;
        public SupplierController()
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
            var list = _context.Database.SqlQuery<Customers>("select * from  customers where discount =1 ").ToList();
            return View(list);
        }
        public ActionResult Create(Customers customers)
        {
            customers.customerid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(customerid),0)+1 from customers").FirstOrDefault();
            return View(customers);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(Customers customers)
        {
            int account_no1;
            Session["message"] = "";
           
             customers.accno = _context.Database.SqlQuery<int>("SELECT ISNULL(MAX(AccountNo), 0) as account_no FROM AccountTitles where AccountHeadId = '2'").FirstOrDefault();
                if (customers.accno == 0)
                {
                    account_no1 = Convert.ToInt32("2100001");
                }
                else
                {
                    account_no1 = Convert.ToInt32(customers.accno + 1);
                }
               
                _context.Database.ExecuteSqlCommand("insert into customers (customerid,Name,address,Cityid,sectorid,Phone,mobile,email,discount,accno,partywht) values(" + customers.customerid + ",N'" + customers.Name + "',N'" + customers.Address + "',1,1,N'" + customers.Phone + "',N'" + customers.mobile + "',N'" + customers.email + "',1," + account_no1 + ",1)");
                _context.Database.ExecuteSqlCommand("insert into AccountTitles (b_unit,AccountNo,AccMain,AccountHeadId,AccountTitleName,AccountType,cr,dr,Secondlevel) values('0'," + account_no1 + ",2001,2,N'" + customers.Name + "','Supplier',0,0,'2000001')");

                    return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var cargo = _context.Database.SqlQuery<Customers>("select * from customers where customerid =" + ID + "").SingleOrDefault();
            return View(cargo);
        }
        [HttpPost]
        public ActionResult Edit(Customers customers)
        {
            _context.Database.ExecuteSqlCommand("UPDATE  customers set Name='" + customers.Name + "',address='" + customers.Address + "',Phone='" + customers.Phone + "',email='" + customers.email + "',mobile='" + customers.mobile + "'  where customerid = " + customers.customerid + "");
            _context.Database.ExecuteSqlCommand("UPDATE AccountTitles set AccountTitleName = N'" + customers.Name + "' where AccountNo = " + customers.accno + "");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From customers where customerid =" + ID + "");
            return RedirectToAction("Index");
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