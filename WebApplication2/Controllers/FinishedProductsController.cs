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
    public class FinishedProductsController : Controller
    {
        private ApplicationDbContext _context;

        public FinishedProductsController()
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
            var list = _context.Database.SqlQuery<Products>("SELECT P.OpeningDubbi, P.OpeningQuarter, P.OpeningGallon, P.OpeningDrum, P.ProductName, P.ProductID, P.[desc], P.Active, P.ReorderDubbi, P.ReorderQuarter, P.ReorderGallon, P.ReorderDrum, PE.MaxDubbi, PE.MaxQuarter, PE.MaxGallon, PE.MaxDrum FROM Product AS P INNER JOIN ProductExtendF AS PE ON P.ProductID = PE.ProductID WHERE (P.CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))").ToList();
            return View(list);
        }
        public ActionResult Create(Products products)
        {
            products.ProductID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(ProductID),0)+1 from Product  ").FirstOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories  where rawproductcheck!=1").ToList();
            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                quality = quality,
                products = products,
            };
            return View(ProductVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Products products)
        {
            _context.Database.ExecuteSqlCommand("insert into Product (UnitPrice,BrandID,barcode,sp,splower,gallon,drum,CapDubbi,CapQuarter,CapGallon,CapDrum,ProductName,ProductID,itmdisc,ReorderLevel,vattax,CategoryID,[desc],Active,ReorderDubbi,ReorderQuarter,ReorderGallon,ReorderDrum,OpeningDubbi,OpeningQuarter,OpeningGallon,OpeningDrum) values(0," + products.BrandID + "," + products.barcode + "," + products.sp + "," + products.splower + "," + products.gallon + "," + products.drum + "," + products.CapDubbi + "," + products.CapQuarter + "," + products.CapGallon + "," + products.CapDrum + ",'" + products.ProductName + "'," + products.ProductID + "," + products.itmdisc + "," + products.ReorderLevel + "," + products.vattax + "," + products.CategoryID + ",N'" + products.desc + "','" + Request["active"] + "'," + products.ReorderDubbi + "," + products.ReorderQuarter + "," + products.ReorderDubbi + "," + products.ReorderDrum + "," + products.OpeningDubbi + "," + products.OpeningQuarter + "," + products.OpeningGallon + "," + products.OpeningDrum + ")");
            _context.Database.ExecuteSqlCommand("insert into ProductExtendF values (" + products.ProductID + "," + products.MaxDubbi + "," + products.MaxQuarter + "," + products.MaxGallon + "," + products.MaxDrum + ")");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var products = _context.Database.SqlQuery<Products>("SELECT P.OpeningDubbi, P.OpeningQuarter, P.OpeningGallon, P.OpeningDrum, P.BrandID, P.barcode, P.sp, P.splower, P.gallon, P.drum, P.CapDubbi, P.CapQuarter, P.CapGallon, P.CapDrum, P.ProductName, P.ProductID, P.itmdisc, P.ReorderLevel, P.vattax, P.CategoryID, P.[desc], P.Active, P.ReorderDubbi, P.ReorderQuarter, P.ReorderGallon, P.ReorderDrum, PE.MaxDubbi, PE.MaxQuarter, PE.MaxGallon, PE.MaxDrum FROM Product AS P INNER JOIN ProductExtendF AS PE ON P.ProductID = PE.ProductID WHERE (P.ProductID = " + ID + ")").SingleOrDefault();
            var Categories_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories  where rawproductcheck!=1").ToList();
            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                Categories_list = Categories_list,
                products = products,
                quality = quality,
            };
            return View(ProductVM);
        }
        [HttpPost]
        public ActionResult Edit(Products products)
        {
            _context.Database.ExecuteSqlCommand("Update Product set BrandID=" + products.BrandID + ",barcode=" + products.barcode + ",sp=" + products.sp + ",splower=" + products.splower + ",gallon=" + products.gallon + ",drum=" + products.drum + ",CapDubbi=" + products.CapDubbi + ",CapQuarter=" + products.CapQuarter + ",CapGallon=" + products.CapGallon + ",CapDrum=" + products.CapDrum + ", ProductName  = '" + products.ProductName + "', [desc]  = '" + products.desc + "', Active  = '" + Request["active"] + "', CategoryID=" + products.CategoryID + ",vattax=" + products.vattax + ", ReorderLevel=" + products.ReorderLevel + ", itmdisc=" + products.itmdisc + ",ReorderDubbi=" + products.ReorderDubbi + ",ReorderQuarter=" + products.ReorderQuarter + ",ReorderGallon=" + products.ReorderGallon + ",ReorderDrum=" + products.ReorderDrum + ",OpeningDubbi=" + products.OpeningDubbi + ",OpeningQuarter=" + products.OpeningQuarter + ",OpeningGallon=" + products.OpeningGallon + ",OpeningDrum=" + products.OpeningDrum + " where ProductID = " + products.ProductID);
            _context.Database.ExecuteSqlCommand("Update ProductExtendF set MaxDubbi=" + products.MaxDubbi + ",MaxQuarter=" + products.MaxQuarter + ",MaxGallon=" + products.MaxGallon + ",MaxDrum=" + products.MaxDrum + " where ProductID = " + products.ProductID);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Product where ProductID = " + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From ProductExtendF where ProductID = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}