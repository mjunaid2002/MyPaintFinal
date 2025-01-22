using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class ProductPricingController : Controller
    {
        private ApplicationDbContext _context;

        public ProductPricingController()
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
            var list = _context.Database.SqlQuery<ProductPricing>("select Product.CategoryID,CategoryName,sp,splower,gallon,drum from Categories Inner join Product on Product.CategoryID=Categories.CategoryID where RawProductCheck=0 group by Product.CategoryID, CategoryName, sp, splower, gallon, drum").ToList();
            return View(list);
        }
        public ActionResult Create(Categories categories)
        {
            categories.CategoryID = _context.Database.SqlQuery<decimal>("select ISNULL(Max(CategoryID),0)+1 from Categories  ").FirstOrDefault();
            var listss = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                categories = categories,
                listss = listss,
            };
            return View(ProductVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(Categories categories)
        {
              _context.Database.ExecuteSqlCommand("insert into Categories (IsPacking,RawProductCheck,CategoryID,CategoryName,Description,MainCategoryID) values('" + Request["IsPacking"] +"','" + Request["RawProductCheck"] +"'," + categories.CategoryID + ",N'" + categories.CategoryName + "',N'" + categories.Description + "'," + categories.MainCategoryID + ")");
                return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var categories = _context.Database.SqlQuery<Categories>("SELECT * from Categories where CategoryID=" + ID+"").SingleOrDefault();
            var listss = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var ProductVM = new ProductVM
            {
                categories = categories,
                listss = listss,
            };
            return View(ProductVM);
        }
        [HttpPost]
        public ActionResult Update()
        {
                _context.Database.ExecuteSqlCommand("Update Product set sp="+Request["sp"] + ",splower=" + Request["splower"] + ",gallon=" + Request["gallon"] + ",drum=" + Request["drum"] + " where CategoryID=" + Request["catid"] +"");
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From Categories where CategoryID = " + ID + "");
            return RedirectToAction("Index");
        }


        public ActionResult IndexRegion()
        {
            var list = _context.Database.SqlQuery<ProductPricing>("SELECT ProductPricingRegion.CategoryID, ProductPricingRegion.RegionID, ProductPricingRegion.dubi as sp, ProductPricingRegion.quarter as splower, ProductPricingRegion.gallon as gallon, ProductPricingRegion.drum as drum, ProductPricingRegion.id, Region.name as RegionName, Categories.CategoryName as CategoryName FROM ProductPricingRegion INNER JOIN Region ON ProductPricingRegion.RegionID = Region.id INNER JOIN Categories ON ProductPricingRegion.CategoryID = Categories.CategoryID").ToList();
            return View(list);
        }
        public ActionResult CreateRegion(ProductPricingRegion ProductPricingRegion)
        {
            var listss = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var lists = _context.Database.SqlQuery<Categories>("SELECT * from Categories").OrderBy(x => x.CategoryName).ToList();
            var regions = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var ProductVM = new ProductVM
            {
                ProductPricingRegion = ProductPricingRegion,
                Region_list = regions,
                Categories_list = lists,
                listss = listss,
            };
            return View(ProductVM);

        }
        [HttpPost, ActionName("SaveRegion")]
        public ActionResult SaveRegion(ProductVM ProductVM, decimal[] id, decimal[] dubi, decimal[] Quarter, decimal[] gallon, decimal[] drum)
        {
            var region = ProductVM.ProductPricingRegion.RegionID;
            var check = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0) from ProductPricingRegion where RegionID= " + region).FirstOrDefault();
            if (check != 0)
            {
                _context.Database.ExecuteSqlCommand("Delete From ProductPricingRegion  where RegionID= " + region);
            }
            for (int i = 0; i < id.Length; i++)
            {
                //var id1= _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from ProductPricingRegion  ").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("insert into ProductPricingRegion (CategoryID,RegionID,dubi,quarter,gallon,drum) values('" + id[i] + "','" + region + "','" + dubi[i] + "','" + Quarter[i] + "','" + gallon[i] + "','" + drum[i] + "')");


            }

            return RedirectToAction("IndexRegion");
        }
        [HttpGet]
        public ActionResult UpdateTable(int regionId)
        {
            var updatedData = _context.Database.SqlQuery<ProductPricing>("SELECT ProductPricingRegion.CategoryID, ProductPricingRegion.RegionID, ProductPricingRegion.dubi AS sp, ProductPricingRegion.quarter AS splower, ProductPricingRegion.gallon, ProductPricingRegion.drum, ProductPricingRegion.id, Categories.CategoryName   FROM ProductPricingRegion INNER JOIN Categories ON ProductPricingRegion.CategoryID = Categories.CategoryID where RegionID=" + regionId).OrderBy(x => x.CategoryName).ToList();
            return Json(updatedData, JsonRequestBehavior.AllowGet);
        }



        public ActionResult IndexFinishRegion()
        {
            //var list = _context.Database.SqlQuery<ProductFinishedRegion>("Select PF.sr, PF.pid, PF.regionid, PF.dubi_o, PF.quarter_o, PF.gallon_o, PF.drum_o, PF.dubi_w, PF.quarter_w, PF.gallon_w, PF.drum_w, R.name as regionname,ProductName, dubi_min, quarter_min, gallon_min, drum_min, dubi_max, quarter_max, gallon_max, drum_max From ProductFinishedRegion  AS PF INNER JOIN   Region AS R ON PF.regionid = R.id  INNER JOIN     Product ON PF.pid = Product.ProductID ORDER BY PF.regionid").ToList();
            var list = _context.Database.SqlQuery<ProductFinishedRegion>("SELECT PF.sr, PF.pid, PF.regionid, PF.dubi_o, PF.quarter_o, PF.gallon_o, PF.drum_o, PF.dubi_w, PF.quarter_w, PF.gallon_w, PF.drum_w, R.name AS regionname, Product.ProductName, PF.dubi_min, PF.quarter_min, PF.gallon_min, PF.drum_min, PF.dubi_max, PF.quarter_max, PF.gallon_max, PF.drum_max, B.name AS branchname, PF.branchid FROM ProductFinishedRegion AS PF INNER JOIN Region AS R ON PF.regionid = R.id INNER JOIN Product ON PF.pid = Product.ProductID INNER JOIN Branch AS B ON PF.branchid = B.id ORDER BY PF.regionid").ToList();
            return View(list);
        }
        public ActionResult CreateFinishRegion(ProductFinishedRegion ProductFinishedRegion)
        {
            var listss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)  order by ProductID").ToList();
    
            var regions = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Branch  = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();

            var ProductVM = new ProductVM
            {
                ProductFinishedRegion = ProductFinishedRegion,
                Region_list = regions,
                branch_list = Branch,
                pro_list = listss,
           
            };
            return View(ProductVM);

        }
        [HttpPost, ActionName("SaveFinishRegion")]
        public ActionResult SaveFinishRegion(ProductVM ProductVM, decimal[] id, decimal[] dubi_o, decimal[] quarter_o, decimal[] gallon_o, decimal[] drum_o, decimal[] dubi_w, decimal[] quarter_w, decimal[] gallon_w, decimal[] drum_w, decimal[] dubi_min, decimal[] quarter_min, decimal[] gallon_min, decimal[] drum_min, decimal[] dubi_max, decimal[] quarter_max, decimal[] gallon_max, decimal[] drum_max)
        {
            var region = ProductVM.ProductFinishedRegion.regionid;
            var branch = ProductVM.ProductFinishedRegion.branchid;
            var check = _context.Database.SqlQuery<decimal>("select ISNULL(Max(sr),0) from ProductFinishedRegion where regionid= " + region+" AND branchid= "+branch  ).FirstOrDefault();
            if (check != 0)
            {
                _context.Database.ExecuteSqlCommand("Delete From ProductFinishedRegion  where regionid= " + region + " AND branchid= " + branch);
            }
            for (int i = 0; i < id.Length; i++)
            {
                //var id1= _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from ProductFinishedRegion  ").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("insert into ProductFinishedRegion (sr,pid,regionid,branchid, dubi_o, quarter_o, gallon_o, drum_o, dubi_w, quarter_w, gallon_w, drum_w, dubi_min, quarter_min, gallon_min, drum_min, dubi_max, quarter_max, gallon_max, drum_max) values('" + i+"','" + id[i] + "','" + region + "','" + branch + "','" + dubi_o[i] + "','" + quarter_o[i] + "','" + gallon_o[i] + "','" + drum_o[i] + "','" + dubi_w[i] + "','" + quarter_w[i] + "','" + gallon_w[i] + "','" + drum_w[i] + "','" + dubi_min[i] + "','" + quarter_min[i] + "','" + gallon_min[i] + "','" + drum_min[i] + "','" + dubi_max[i] + "','" + quarter_max[i] + "','" + gallon_max[i] + "','" + drum_max[i] + "')");


            }

            return RedirectToAction("IndexFinishRegion");
        }
        [HttpGet]
        public ActionResult UpdateFinishTable(int regionId,int branchId)
        {
            var updatedData = _context.Database.SqlQuery<ProductFinishedRegion>("SELECT pid,regionid,branchid, dubi_o, quarter_o, gallon_o, drum_o, dubi_w, quarter_w, gallon_w, drum_w, dubi_min, quarter_min, gallon_min, drum_min, dubi_max, quarter_max, gallon_max, drum_max from ProductFinishedRegion where regionid=" + regionId +" AND branchid="+ branchId + " order by pid ").OrderBy(x => x.pid).ToList();
            return Json(updatedData, JsonRequestBehavior.AllowGet);
        }
    }
}