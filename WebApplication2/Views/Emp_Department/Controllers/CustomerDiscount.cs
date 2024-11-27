using System.Linq;
using System.Web.Mvc;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System.Net;
namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class CustomerDiscountController : Controller
    {
        private ApplicationDbContext _context;

        public CustomerDiscountController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: SaleWctn
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<CustomerDiscount>("SELECT *  from tbl_Customer_Item_Discount").ToList();
            return View(list);
        }
        public ActionResult Create(CustomerDiscount customerDiscount)
        {
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
            var cat_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var SaleInvVM = new SaleInvVM
            {
                customerDiscount = customerDiscount,
                cat_list = cat_list,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] cus_name, string[] cat_name, decimal[] disc, string[] cus_value, string[] cat_value)
        {
            for (int i = 0; i < cus_value.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO tbl_Customer_Item_Discount (customerid,CategoryID,customername,CategoryName,disc) values(" + cus_value[i] + "," + cat_value[i] + ",'" + cus_name[i] + "','" + cat_name[i] + "'," + disc[i] + ")");
            }
            return RedirectToAction("Index");
        }
           public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             _context.Database.ExecuteSqlCommand("Delete From tbl_Customer_Item_Discount where Id =" + ID + "");
            return RedirectToAction("Index");
        }
    
    }
}