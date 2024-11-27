using CRM.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class RawProductsController : Controller
    {
        private ApplicationDbContext _context;

        public RawProductsController()
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
            var list = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product").ToList();
            return View(list);
        }
        public ActionResult Create(Products products)
        {
            products.ProductID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ProductID),0)+1 from Product  ").FirstOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where rawproductcheck=1").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                products = products,
            };
            return View(ProductVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Products products)
        {
              _context.Database.ExecuteSqlCommand("insert into Product (ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active) values('" + products.ProductName + "'," + products.ProductID + "," + products.UnitPrice + "," + products.ReorderLevel + "," + products.vattax + "," + products.CategoryID + ",N'" + products.desc + "','" + Request["active"] + "')");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var products = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where ProductID = "+ID+"").SingleOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where rawproductcheck=1").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                products = products,
            };
            return View(ProductVM);
        }
        [HttpPost]
        public ActionResult Edit(Products products)
        {
                _context.Database.ExecuteSqlCommand("Update Product set ProductName  = '" + products.ProductName + "', [desc]  = '" + products.desc + "', Active  = '" + Request["active"] + "', CategoryID=" + products.CategoryID + ",vattax=" + products.vattax + ", ReorderLevel=" + products.ReorderLevel + ", UnitPrice=" + products.UnitPrice + " where ProductID = " + products.ProductID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Product where ProductID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}