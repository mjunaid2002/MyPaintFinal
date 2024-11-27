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
    public class ProductController : Controller
    {
        // GET: Product
        private ApplicationDbContext _context;
        public ProductController()
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
            string b_unit = Convert.ToString(Session["BusinessUnit"]);
            var product_list = _context.Product.Where(z => z.BusinessUnit == Bunit).ToList();
            // customer.Name= "";

            var lp_lisr = _context.Database.SqlQuery<ProductQuery>("select *,(select (opening + QTYIN) - QTYOUT as bal from (select OpeningStock +(openingnopur-openingno) as opening,QTYOUT,QTYIN from ( SELECT OpeningStock,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID = Products.id) and b_unit = '" + b_unit + "' ) AS openingnopur,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and b_unit = '" + b_unit + "') AS openingno,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM SaleDetails WHERE (ItemID = Products.id) and b_unit = '" + b_unit + "') AS QTYOUT,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM PurDetails WHERE (ItemID =Products.id) and b_unit = '" + b_unit + "') AS QTYIN ) as der ) as der1) as bal from products where BusinessUnit="+ b_unit + " ").ToList();

            var cat_list = _context.Category.Where(z => z.b_unit == b_unit).ToList();
            var br_list = _context.Brands.Where(z => z.b_unit == b_unit).ToList();
            var MeasuringUnit = _context.tbl_MeasuringUnit.Where(c => c.b_unit == b_unit).ToList();
            var ProductVM = new ProductVM
            {
                lp_lisr = lp_lisr,
                MeasuringUnit = MeasuringUnit,
                product_list = product_list,
                cat_list = cat_list,
                br_list = br_list
            };
            return View(ProductVM);
        }
        public ActionResult Create(Product Product)
        {
            string b_unit = Convert.ToString(Session["BusinessUnit"]);
            Product.ID = _context.Database.SqlQuery<int>("select ISNULL(Max(ID),0)+1 from Products").FirstOrDefault();
            // customer.Name= "";
            var MeasuringUnit = _context.tbl_MeasuringUnit.Where(c => c.b_unit == b_unit).ToList();
            var cat_list = _context.Category.Where(c=>c.b_unit== b_unit).ToList();
            var br_list = _context.Brands.Where(c => c.b_unit == b_unit).ToList();
            var ProductVM = new ProductVM
            {
                MeasuringUnit = MeasuringUnit,
                Product = Product,
                cat_list = cat_list,
                br_list = br_list
            };
            return View(ProductVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, string type, ProductVM products, int[] shelf_number, string[] name, string[] cost_price, decimal[] sale_price,decimal[] sale_price_euro, string[] least_price, int[] pieces_boc, int[] op_stock, string[] stock_date, string[] bar_code, string[] wat, string[] brand, string[] m_unit, string[] re_level)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            string img;

            for (int i = 0; i < name.Count(); i++)
            {
                if (file[i] == null)
                {
                    img = "demo.jpg";
                }
                else
                {
                    ImageName = System.IO.Path.GetFileName(file[i].FileName);
                    img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file[i].SaveAs(physicalPath);
                }
                _context.Database.ExecuteSqlCommand("INSERT INTO Products (SalePrice_euro,type,BusinessUnit,CatID,BrandID,Image,Name,CostPrice,SalePrice,LeastPrice,PiecesBox,OpeningStock,StockDate,Barcode,Wat,Brand,MUnit,ReOrder,ShelfNumber) VALUES ("+ sale_price_euro[i] + ",'" + type + "'," + Session["BusinessUnit"] + ",'" + products.Product.CatID + "','" + products.Product.BrandID + "','" + img + "',N'" + name[i] + "'," + cost_price[i] + "," + sale_price[i] + "," + least_price[i] + "," + pieces_boc[i] + "," + op_stock[i] + ",'" + stock_date[i] + "','" + bar_code[i] + "','" + wat[i] + "','" + brand[i] + "','" + m_unit[i] + "'," + re_level[i] + "," + shelf_number[i] + ")");
            }
            return RedirectToAction("Create");
        }
        [HttpPost]
        public ActionResult SearchProduct(int[] ID, string button)
        {
            string b_unit = Convert.ToString(Session["BusinessUnit"]);
            if (button == "edit")
            {
                var name =Request["Name"];
                var des = Request["CatID"];
                var type = Request["type"];
                var CostPrice =Request["CostPrice"];
                var SalePrice = Request["SalePrice"];
                var LeastPrice = Request["LeastPrice"];
                var IDs = Request["ID"];
                _context.Database.ExecuteSqlCommand("UPDATE Products set CatID ='" + des + "', type ='" + type + "',  Name = '" + name + "', CostPrice = " + CostPrice + ", SalePrice = " + SalePrice + ", LeastPrice = " + LeastPrice + " where ID = " + IDs + "");
                return RedirectToAction("Index");
            }
            else
            {
                string inv_ids = "";
                for (int i = 0; i < ID.Count(); i++)
                {
                    inv_ids = inv_ids + "'" + ID[i] + "'";
                    if (i != ID.Count() - 1)
                    {
                        inv_ids = inv_ids + ",";
                    }
                }
                var product_list = _context.Database.SqlQuery<Product>("SELECT * from Products Where Id in (" + inv_ids + ")").ToList();
                var cat_list = _context.Category.Where(c => c.b_unit == b_unit).ToList();
                var br_list = _context.Brands.Where(c => c.b_unit == b_unit).ToList();
                var ProductVM = new ProductVM
                {
                    product_list = product_list,
                    cat_list = cat_list,
                    br_list = br_list
                };
                return View("Edit", ProductVM);
            }

        }
        [HttpPost]
        public ActionResult Edit(string[] des, string[] type, int[] ID, HttpPostedFileBase[] file, ProductVM products, decimal[] shelf_number, string[] name, string[] cost_price, string[] sale_price, string[] least_price, decimal[] pieces_boc, int[] op_stock, string[] stock_date, string[] bar_code, string[] wat, string[] brand, string[] m_unit, string[] re_level)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            string img;

            for (int i = 0; i < name.Count(); i++)
            {
                if (file[i] == null)
                {
                    img = "demo.jpg";
                }
                else
                {
                    ImageName = System.IO.Path.GetFileName(file[i].FileName);
                    img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file[i].SaveAs(physicalPath);
                }
                _context.Database.ExecuteSqlCommand("UPDATE Products set CatID ='" + des[i] + "',type ='" + type[i] + "',  Image = '" + img + "', Name = N'" + name[i] + "', CostPrice = " + cost_price[i] + ", SalePrice = " + sale_price[i] + ", LeastPrice = " + least_price[i] + ", PiecesBox = " + pieces_boc[i] + ", OpeningStock = " + op_stock[i] + ",StockDate = '" + stock_date[i] + "', Barcode = '" + bar_code[i] + "', Wat = '" + wat[i] + "', Brand = '" + brand[i] + "', MUnit = '" + m_unit[i] + "', ReOrder = " + re_level[i] + ", ShelfNumber = " + shelf_number[i] + " where ID = " + ID[i] + "");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Update(string ShelfNumber, HttpPostedFileBase file, string brandper,  string vat,  string ReOrder, string Barcode, string cat,string brand, string unit, string op_stock, string p_box,  string type, string p_id, string name, string cost_price, string sale_price, string SalePrice_euro, string least_price)
        {
            string img = "";
            if (file == null)
            {
                img = "demo.jpg";
                _context.Database.ExecuteSqlCommand("UPDATE Products set wat='" + vat + "', brand='" + brandper + "', Image='" + img + "',ShelfNumber='" + ShelfNumber + "', ReOrder='" + ReOrder + "', Barcode='" + Barcode + "', catid='" + cat + "',BrandId='" + brand + "',MUnit='" + unit + "',OpeningStock=" + op_stock + ",PiecesBox=" + p_box + ",SalePrice_euro =" + SalePrice_euro + ",type ='" + type + "',  Name = N'" + name + "', CostPrice = " + cost_price + ", SalePrice = " + sale_price + ", LeastPrice = " + least_price + " where ID = " + p_id + "");

            }
            else
            {
                Random r = new Random();
                int num = r.Next();
                string ImageName = "";
                string physicalPath;
                ImageName = System.IO.Path.GetFileName(file.FileName);
                 img = num + ImageName;
                physicalPath = Server.MapPath("~/Uploads/" + img);
                file.SaveAs(physicalPath);
                _context.Database.ExecuteSqlCommand("UPDATE Products set Image='" + img + "',ShelfNumber='" + ShelfNumber + "', ReOrder='" + ReOrder + "', Barcode='" + Barcode + "', catid='" + cat + "',BrandId='" + brand + "',MUnit='" + unit + "',OpeningStock=" + op_stock + ",PiecesBox=" + p_box + ",SalePrice_euro =" + SalePrice_euro + ",type ='" + type + "',  Name = N'" + name + "', CostPrice = " + cost_price + ", SalePrice = " + sale_price + ", LeastPrice = " + least_price + " where ID = " + p_id + "");
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Products where Id = " + ID + "");
            return RedirectToAction("Index");
        }
    }
}