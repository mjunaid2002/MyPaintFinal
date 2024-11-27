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
           //var list = _context.Database.SqlQuery<Products>("select ISNULL((SELECT MaxLevelRaw FROM ProductExtend WHERE (ProductID = Product.ProductID)), 0) AS MaxLevelRaw,(select isnull(CategoryName,0) from QualityCategories where id =Product.iduser ) as quality,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var list = _context.Database.SqlQuery<Products>("select ISNULL((SELECT MaxLevelRaw FROM ProductExtend WHERE (ProductID = Product.ProductID)), 0) AS MaxLevelRaw,(select isnull(CategoryName,0) from QualityCategories where id =Product.iduser ) as quality,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,Product.CategoryID, CategoryName, [desc],Active from Product INNER JOIN Categories ON Product.CategoryID = Categories.CategoryID where Product.CategoryID in (select CategoryID from Categories where RawProductCheck = 1)").ToList();
            return View(list);
        }
        public ActionResult Create(Products products)
        {
            products.ProductID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ProductID),0)+1 from Product  ").FirstOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where rawproductcheck=1").ToList();
            var QualityCategories = _context.Database.SqlQuery<QualityCategories>("SELECT * from QualityCategories").ToList();
            var product_Type = _context.Database.SqlQuery<Product_Type>("SELECT * from Product_Type order by id").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                products = products,
                QualityCategories = QualityCategories,
                Product_Type_list = product_Type
            };
            return View(ProductVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Products products)
        {
            _context.Database.ExecuteSqlCommand("insert into Product (iduser,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active,sp, splower, barcode, BrandID, ProductNameUrdu, cmpbarcode, itmdisc, tax, leaf, prdunit, gallon, drum, CapDubbi, CapQuarter, CapGallon, CapDrum, ReorderDubbi, ReorderQuarter, ReorderGallon,ReorderDrum, OpeningDubbi, OpeningQuarter, OpeningGallon, OpeningDrum) values(" + products.iduser + ",'" + products.ProductName + "'," + products.ProductID + "," + products.UnitPrice + "," + products.ReorderLevel + "," + products.vattax + "," + products.CategoryID + ",N'" + products.desc + "','" + Request["active"] + "',0,0,0,1,'','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)");
              _context.Database.ExecuteSqlCommand("insert into ProductExtend  values(" + products.ProductID + "," + products.MaxLevelRaw + ",'" + products.ProductType + "')");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var products = _context.Database.SqlQuery<Products>("select ISNULL((SELECT MaxLevelRaw FROM ProductExtend WHERE (ProductID = Product.ProductID)), 0) AS MaxLevelRaw, ISNULL((SELECT ProductType FROM ProductExtend WHERE (ProductID = Product.ProductID)), '') AS ProductType,iduser,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where ProductID = " + ID+"").SingleOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where rawproductcheck=1").ToList();
            var QualityCategories = _context.Database.SqlQuery<QualityCategories>("SELECT * from QualityCategories").ToList();
            var product_Type = _context.Database.SqlQuery<Product_Type>("SELECT * from Product_Type order by id").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                products = products,
                QualityCategories = QualityCategories,
                Product_Type_list= product_Type
            };
            return View(ProductVM);
        }
        [HttpPost]
        public ActionResult Edit(Products products)
        {
            _context.Database.ExecuteSqlCommand("Update Product set iduser = "+products.iduser + ",ProductName  = '" + products.ProductName + "', [desc]  = '" + products.desc + "', Active  = '" + Request["active"] + "', CategoryID=" + products.CategoryID + ",vattax=" + products.vattax + ", ReorderLevel=" + products.ReorderLevel + ", UnitPrice=" + products.UnitPrice + " where ProductID = " + products.ProductID + "");
                _context.Database.ExecuteSqlCommand("Update ProductExtend set MaxLevelRaw = " + products.MaxLevelRaw + ",ProductType = '" + products.ProductType + "' where ProductID = " + products.ProductID + "");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Product where ProductID = " + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From ProductExtend where ProductID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}