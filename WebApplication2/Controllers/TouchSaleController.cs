using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using WebApplication2.ViewModels;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class TouchSaleController : Controller
    {
        private ApplicationDbContext _context;
        public TouchSaleController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: TouchSale
        public ActionResult Index()
        {
            var cat_list = _context.Category.ToList();
            var pro_list = _context.Product.ToList();
            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT *,(Select name From Customers where AccountNo=SaleMasters.CustomerId) as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWCTN' ORDER BY InvID DESC").ToList();
            var TouchSaleInvVM = new TouchSaleInvVM
            {

                cat_list = cat_list,
                sale_list_woc = sale_list_woc,
                pro_list = pro_list
            };
            return View(TouchSaleInvVM);
        }
        [HttpGet]
        public PartialViewResult GetAllowanceList(int? id)
        {
           
            var partialViewmodel = new partialViewmodel
            {
                pro_list = _context.Database.SqlQuery<Product>("SELECT * FROM Products Where CatID = " + id+"").ToList(),
            };


            return PartialView("PartialView", partialViewmodel);
        }
        public ActionResult Create()
        {
            return View();
        }
    }
}