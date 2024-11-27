using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using OfficeOpenXml;
using System.IO;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ProductionOrderController : Controller
    {
        private ApplicationDbContext _context;
        public ProductionOrderController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult Index()
        {
            string strquery = " where PO.curdate ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where PO.curdate between '" + StartDate + "' and '" + Enddate + "'  ";

            var list = _context.Database.SqlQuery<ProductionOrderView>("SELECT PO.Id, PO.curdate, PO.Promiseddate, C.Name AS customername, R.name AS regionname FROM ProductionOrder AS PO LEFT OUTER JOIN Customers AS C ON PO.customerid = C.customerid LEFT OUTER JOIN Region AS R ON PO.regionid = R.id" + strquery).ToList();

            return View(list);
        }
        public ActionResult Create(ProductionOrder ProductionOrder)
        {
           
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =0").ToList();
           // var pro_listsss = _context.Database.SqlQuery<Products>("select CapDubbi, CapQuarter, CapGallon, CapDrum, ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product ").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();
            ProductionOrder.curdate = DateTime.Today;
            ProductionOrder.Promiseddate = DateTime.Today;

            var vm = new SaleInvVM { 
                Cus_list= Cus_list,
                ProductionOrder = ProductionOrder,
                pro_listsss = pro_listsss,
                Region_list = Region,
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Save(ProductionOrder ProductionOrder, string[] item_name, string[] dubbi, decimal[] quarter, decimal[] drum, decimal[] gallon, decimal[] totalqty, decimal[] id)
        {
            var st = "";
            if (ProductionOrder.Id == 0)
            {
                // _context.ProductionOrder.Add(ProductionOrder);
          
                _context.Database.ExecuteSqlCommand("INSERT INTO     ProductionOrder( curdate, Promiseddate, customerid, regionid) " +
                  " VALUES ('" + ProductionOrder.curdate + "','" + ProductionOrder.Promiseddate + "','" + ProductionOrder.customerid + "','" + ProductionOrder.regionid +"') " );
               
                var maxid = _context.Database.SqlQuery<int>("Select ISNULL(MAX(id),0)  From    ProductionOrder ").FirstOrDefault();
               
                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductionOrderDetail (ID, pid, pname, dubbi, quarter, gallon, drum, totalqty) " +
                        "VALUES ('" + maxid + "','" + id[i] + "','" + item_name[i] + "','" + dubbi[i] + "','" + quarter[i] + "','" + drum[i] + "','" + gallon[i] + "','" + totalqty[i] + "')");
                }

                st = "Create";

            }
            else {
                _context.Database.ExecuteSqlCommand("Delete from ProductionOrderDetail where ID="+ ProductionOrder.Id);
                _context.Database.ExecuteSqlCommand("UPDATE  ProductionOrder SET curdate ='"+ ProductionOrder.curdate + "', Promiseddate ='" + ProductionOrder.Promiseddate + "', customerid ='" + ProductionOrder.customerid + "', regionid ='" + ProductionOrder.regionid + "' where ID=" + ProductionOrder.Id);
               

                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductionOrderDetail (ID, pid, pname, dubbi, quarter, gallon, drum, totalqty) " +
                        "VALUES (" + ProductionOrder.Id + "," + id[i] + ",'" + item_name[i] + "'," + dubbi[i] + "," + quarter[i] + "," + drum[i] + "," + gallon[i] + "," + totalqty[i] + ")");
                }
                st = "Index";
            }
            _context.SaveChanges();
            return RedirectToAction(st);
        }

        public ActionResult Edit(int? ID)
        {
            var ProductionOrder= _context.Database.SqlQuery<ProductionOrder>("SELECT * from ProductionOrder where Id="+ID).SingleOrDefault();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var ProductionOrderDetail = _context.Database.SqlQuery<ProductionOrderDetail>("SELECT * from ProductionOrderDetail where Id="+ID).ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select CapDubbi, CapQuarter, CapGallon, CapDrum,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product ").ToList();


            var vm = new SaleInvVM
            {
                Cus_list = Cus_list,
                ProductionOrder = ProductionOrder,
                ProductionOrderDetaillist= ProductionOrderDetail,
                pro_listsss = pro_listsss,
                Region_list = Region,
            };
            return View("Create",vm);
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From ProductionOrder where ID =" + ID + " ");
            _context.Database.ExecuteSqlCommand("Delete From ProductionOrderDetail where ID =" + ID + " ");
       
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult getweight(int code)
        {
            //var pid = _context.Database.SqlQuery<decimal>("SELECT ProductID  FROM   Product where ProductName='" + code+"' ").FirstOrDefault();

            var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT ProductID , CapDubbi, CapQuarter, CapGallon, CapDrum FROM   Product where ProductID=" + code ).ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadSheet()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Retrieve data from the database
           // var products = _context.Database.SqlQuery<Products>(" SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active FROM Product WHERE CategoryID IN (SELECT CategoryID FROM Categories WHERE RawProductCheck = 0) ").ToList();

            var products = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();



            // Generate Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                var row = 1;

                // Add headers
                worksheet.Cells[row, 1].Value = "Product ID";
                worksheet.Cells[row, 2].Value = "Product Name";

                worksheet.Cells[row, 3].Value = "Dubbi";
                worksheet.Cells[row, 4].Value = "Quarter";
                worksheet.Cells[row, 5].Value = "Gallon";
                worksheet.Cells[row, 6].Value = "Drum";
                worksheet.Cells[row, 7].Value = "Quantity";
              

                // Add data rows
                foreach (var product in products)
                {
                    row++;
                    worksheet.Cells[row, 1].Value = product.ProductID;
                    worksheet.Cells[row, 2].Value = product.ProductName;
                    var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT ProductID , CapDubbi, CapQuarter, CapGallon, CapDrum FROM   Product where ProductID=" + product.ProductID).ToList();
                    //double wdubbi = 1; 
                    //double wquarter = 1; 
                    //double wdrum = 1; 
                    //double wgallon = 1;
                    var dubbi = getgrosspackage[0].CapDubbi;
                    var quarter = getgrosspackage[0].CapQuarter;
                    var drum = getgrosspackage[0].CapDrum;
                    var gallon = getgrosspackage[0].CapGallon;

                    worksheet.Cells[row, 3].Value = 0;
                    worksheet.Cells[row, 4].Value = 0;
                    worksheet.Cells[row, 5].Value = 0;
                    worksheet.Cells[row, 6].Value = 0;
                    worksheet.Cells[row, 7].Formula = $"={dubbi} * { worksheet.Cells[row, 3]} + {quarter} * { worksheet.Cells[row, 4]} + {gallon} * { worksheet.Cells[row, 5]} + {drum} * { worksheet.Cells[row, 6]}";

                }

                // Save the Excel package to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);

                // Return the Excel file as a download
                stream.Position = 0;
                string fileName = "ProductionOrder.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(stream, contentType, fileName);
            }
        }

        [HttpPost]
        public JsonResult UploadExcel(HttpPostedFileBase fileInput)
        {
            if (fileInput == null || fileInput.ContentLength == 0)
            {
                return Json(new { success = false, message = "No file uploaded" });
            }

            var products = new List<ProductCapShow>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(fileInput.InputStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return Json(new { success = false, message = "Invalid Excel file" });
                }

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var product = new ProductCapShow
                    {
                        ProductID = worksheet.Cells[row, 1].GetValue<int>(),
                        ProductName = worksheet.Cells[row, 2].GetValue<string>(),
                        CapDubbi = worksheet.Cells[row, 3].GetValue<decimal>(),
                        CapQuarter = worksheet.Cells[row, 4].GetValue<decimal>(),
                        CapGallon = worksheet.Cells[row, 5].GetValue<decimal>(),
                        CapDrum = worksheet.Cells[row, 6].GetValue<decimal>(),
                        qty = worksheet.Cells[row, 7].GetValue<decimal>(),
                    };
                    if (product.CapDubbi != 0 || product.CapQuarter != 0 || product.CapGallon != 0 || product.CapDrum != 0  ) {
                        products.Add(product);
                    }
                  
                }
            }

            return Json(new { success = true, products = products });
        }
    }



    //public ActionResult downloadSheet()
    //{
    //    string filePath = Server.MapPath("~/path/to/your/file.xlsx");
    //    string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    //    string fileName = "YourFileName.xlsx";

    //    if (System.IO.File.Exists(filePath))
    //    {
    //        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
    //        return File(fileBytes, fileType, fileName);
    //    }
    //    else
    //    {
    //        return HttpNotFound("File not found");
    //    }
    //}

}