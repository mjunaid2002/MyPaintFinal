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
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers ").ToList();
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
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductionOrderDetail (ID, pid, pname, dubbi, quarter, gallon, drum, totalqty,quarter_c, gallon_c, drum_c, totalqty_c, dubbi_c, totalqty_r,  totalqty_m) " +
                        "VALUES ('" + maxid + "','" + id[i] + "','" + item_name[i] + "','" + dubbi[i] + "','" + quarter[i] + "','" + drum[i] + "','" + gallon[i] + "','" + totalqty[i] + "','0','0','0','0','0','"+ totalqty[i] + "','0')");
                }

                st = "Create";

            }
            //else {
            //    _context.Database.ExecuteSqlCommand("Delete from ProductionOrderDetail where ID="+ ProductionOrder.Id);
            //    _context.Database.ExecuteSqlCommand("UPDATE  ProductionOrder SET curdate ='"+ ProductionOrder.curdate + "', Promiseddate ='" + ProductionOrder.Promiseddate + "', customerid ='" + ProductionOrder.customerid + "', regionid ='" + ProductionOrder.regionid + "' where ID=" + ProductionOrder.Id);
               

            //    for (int i = 0; i < item_name.Count(); i++)
            //    {
            //        _context.Database.ExecuteSqlCommand("INSERT INTO ProductionOrderDetail (ID, pid, pname, dubbi, quarter, gallon, drum, totalqty) "x +
            //            "VALUES (" + ProductionOrder.Id + "," + id[i] + ",'" + item_name[i] + "'," + dubbi[i] + "," + quarter[i] + "," + drum[i] + "," + gallon[i] + "," + totalqty[i] + ")");
            //    }
            //    st = "Index";
            //}
            _context.SaveChanges();
            return RedirectToAction(st);
        }

        public ActionResult Edit(int? ID)
        {
            var ProductionOrder= _context.Database.SqlQuery<ProductionOrder>("SELECT * from ProductionOrder where Id="+ID).SingleOrDefault();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers").ToList();
            var ProductionOrderDetail = _context.Database.SqlQuery<ProductionOrderDetail>("SELECT * from ProductionOrderDetail where Id="+ID).ToList();
            //var pro_listsss = _context.Database.SqlQuery<Products>("select CapDubbi, CapQuarter, CapGallon, CapDrum,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product ").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0)").ToList();


            var vm = new SaleInvVM
            {
                Cus_list = Cus_list,
                ProductionOrder = ProductionOrder,
                ProductionOrderDetaillist= ProductionOrderDetail,
                pro_listsss = pro_listsss,
                Region_list = Region,
            };
            return View(vm);
            //return View("Create",vm);
        }
        [HttpPost]
        public ActionResult Update(ProductionOrder ProductionOrder, string[] item_name, string[] dubbi, decimal[] quarter, decimal[] drum, decimal[] gallon, decimal[] totalqty,string[] dubbi_c, decimal[] quarter_c, decimal[] drum_c, decimal[] gallon_c, decimal[] totalqty_c, decimal[] totalqty_r, decimal[] totalqty_m, decimal[] id)
        {
            var st = "";
          
                _context.Database.ExecuteSqlCommand("Delete from ProductionOrderDetail where ID=" + ProductionOrder.Id);
                _context.Database.ExecuteSqlCommand("UPDATE  ProductionOrder SET curdate ='" + ProductionOrder.curdate + "', Promiseddate ='" + ProductionOrder.Promiseddate + "', customerid ='" + ProductionOrder.customerid + "', regionid ='" + ProductionOrder.regionid + "' where ID=" + ProductionOrder.Id);


                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductionOrderDetail (ID, pid, pname, dubbi, quarter, gallon, drum, totalqty, dubbi_c,quarter_c, gallon_c, drum_c, totalqty_c, totalqty_r, totalqty_m) " +
                        "VALUES (" + ProductionOrder.Id + "," + id[i] + ",'" + item_name[i] + "'," + dubbi[i] + "," + quarter[i] + "," + gallon[i] + "," + drum[i] + "," + totalqty[i] + "," + dubbi_c[i] + "," + quarter_c[i] + "," + gallon_c[i] + "," + drum_c[i] + "," + totalqty_c[i] + "," + totalqty_r[i] + "," + totalqty_m[i] + ")");
                }
                st = "Index";
            
            _context.SaveChanges();
            return RedirectToAction(st);
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
        public ActionResult getweight(int code,int regionid)
       {
            //var pid = _context.Database.SqlQuery<decimal>("SELECT ProductID  FROM   Product where ProductName='" + code+"' ").FirstOrDefault();

          //  var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT ProductID , CapDubbi, CapQuarter, CapGallon, CapDrum FROM   Product where ProductID=" + code ).ToList();
            var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT pid ,dubi_w as CapDubbi,quarter_w AS CapQuarter,gallon_w AS CapGallon,drum_w AS CapDrum FROM   ProductFinishedRegion where pid=" + code + " AND regionid= "+ regionid).ToList();
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


        public ActionResult reportproduction(string type)
        {
         
            var StartDate = DateTime.Today;
            var Enddate = DateTime.Today;
            var region = "0";
          
            var region_id ="0";
            var str = "";
            var query = "";
            if (region != "0")
            {
                str = " AND regionid =" + region;
                query = "SELECT ProductID, ProductName, StkOpenDubbi, StkOpenQuarter, StkOpenGallon, StkOpenDrum, StkInDubbi, StkInQuarter, StkInGallon, StkInDrum, StkOutDubbi, StkOutQuarter, StkOutGallon, StkOutDrum, StkOpenDubbi + StkInDubbi - StkOutDubbi AS StkDubbi, StkOpenQuarter + StkInQuarter - StkOutQuarter AS StkQuarter, StkOpenGallon + StkInGallon - StkOutGallon AS StkGallon, StkOpenDrum + StkInDrum - StkOutDrum AS StkDrum FROM (SELECT ProductID, ProductName, OpeningDubbi + BatchFillingDubbiBefore + SaleReturnDubbiBefore - SaleDubbiBefore - SaleTwoDubbiBefore - DemageDubbiBefore + InDubbiBefore - OutDubbiBefore - StockDistDubbiBefore AS StkOpenDubbi, OpeningQuarter + BatchFillingQuarterBefore + SaleReturnQuarterBefore - SaleQuarterBefore - SaleTwoQuarterBefore - DemageQuarterBefore + InQuarterBefore - OutQuarterBefore -StockDistQuarterBefore AS StkOpenQuarter, OpeningGallon + BatchFillingGallonBefore + SaleReturnGallonBefore - SaleGallonBefore - SaleTwoGallonBefore - DemageGallonBefore + InGallonBefore - OutGallonBefore - StockDistGallonBefore AS StkOpenGallon, OpeningDrum + BatchFillingDrumBefore + SaleReturnDrumBefore - SaleDrumBefore - SaleTwoDrumBefore - DemageDrumBefore + InDrumBefore - OutDrumBefore -StockDistDrumBefore AS StkOpenDrum, BatchFillingDubbiAfter + SaleReturnDubbiAfter + InDubbiAfter AS StkInDubbi, BatchFillingQuarterAfter + SaleReturnQuarterAfter + InQuarterAfter AS StkInQuarter, BatchFillingGallonAfter + SaleReturnGallonAfter + InGallonAfter AS StkInGallon, BatchFillingDrumAfter + SaleReturnDrumAfter + InDrumAfter AS StkInDrum, SaleDubbiAfter + SaleTwoDubbiAfter + DemageDubbiAfter + OutDubbiAfter + StockDistDubbiAfter AS StkOutDubbi, SaleQuarterAfter + SaleTwoQuarterAfter + DemageQuarterAfter + OutQuarterAfter + StockDistQuarterAfter AS StkOutQuarter, SaleGallonAfter + SaleTwoGallonAfter + DemageGallonAfter + OutGallonAfter + StockDistGallonAfter AS StkOutGallon, SaleDrumAfter + SaleTwoDrumAfter + DemageDrumAfter + OutDrumAfter + StockDistDrumAfter AS StkOutDrum FROM (SELECT ProductID, ProductName, (SELECT ISNULL(SUM(dubi_o), 0) AS Expr1 FROM ProductFinishedRegion WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningDubbi, (SELECT ISNULL(SUM(quarter_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_3 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningQuarter, (SELECT ISNULL(SUM(gallon_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_2 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningGallon, (SELECT ISNULL(SUM(drum_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_1 WHERE (pid = Product.ProductID) AND (regionid =" + region_id + ")) AS OpeningDrum, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistDubbiBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistQuarterBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistGallonBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date < '" + StartDate + "')) AS StockDistDrumBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDubbiAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistQuarterAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistGallonAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) AND (sd.RegionId =" + region_id + ") AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDrumAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InDubbiAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InQuarterAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InGallonAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS InDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (ToRegion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS InDrumAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Dubbi') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutDubbiAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Quarter') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutQuarterAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Gallon') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutGallonAfter, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date < '" + StartDate + "')) AS OutDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM FinishTransferM INNER JOIN FinishTransferDetail ON FinishTransferM.invid=FinishTransferDetail.invid WHERE (pid = Product.ProductID) AND (SIZE = 'Drum') AND (Fromregion=" + region_id + ") AND (FinishTransferM.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS OutDrumAfter, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDubbiBefore, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDubbiAfter, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingQuarterBefore, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingQuarterAfter, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingGallonBefore, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingGallonAfter, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDrumBefore, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) AND (RegionId =" + region_id + ") AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDrumAfter, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID AND srsdetail.Status = srsm.title WHERE (srsdetail.packing = 'Dubbi') AND (srsdetail.Status IN ('SRINV', 'TSRINV')) AND (srsdetail.prid = Product.ProductID) AND (srsm.RegionId =" + region_id + ") AND (srsm.date < '" + StartDate + "')) AS SaleReturnDubbiBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Dubbi') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T) AS SaleReturnDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_4.prid = Product.ProductID) AND (srsm_4.RegionId =" + region_id + ") AND (srsm_4.date < '" + StartDate + "')) AS SaleReturnQuarterBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Quarter') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_7) AS SaleReturnQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_3.prid = Product.ProductID) AND (srsm_3.RegionId =" + region_id + ") AND (srsm_3.date < '" + StartDate + "')) AS SaleReturnGallonBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Gallon') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_6) AS SaleReturnGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_2.prid = Product.ProductID) AND (srsm_2.RegionId =" + region_id + ") AND (srsm_2.date < '" + StartDate + "')) AS SaleReturnDrumBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Drum') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) AND (RegionId =" + region_id + ") AND (title IN ('SRINV', 'TSRINV'))))) AS T_5) AS SaleReturnDrumAfter, (SELECT ISNULL(SUM(srsdetail_5.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_5 INNER JOIN srsm AS srsm_5 ON srsdetail_5.OrderID = srsm_5.OrderID AND srsdetail_5.Status = srsm_5.title WHERE (srsdetail_5.packing = 'Dubbi') AND (srsdetail_5.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_5.prid = Product.ProductID) AND (srsm_5.RegionId =" + region_id + ") AND (srsm_5.date < '" + StartDate + "')) AS SaleDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_9 WHERE (packing = 'Dubbi') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_9 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_9.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_4.prid = Product.ProductID) AND (srsm_4.RegionId =" + region_id + ") AND (srsm_4.date < '" + StartDate + "')) AS SaleQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_8 WHERE (packing = 'Quarter') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_8 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_8.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_3.prid = Product.ProductID) AND (srsm_3.RegionId =" + region_id + ") AND (srsm_3.date < '" + StartDate + "')) AS SaleGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_7 WHERE (packing = 'Gallon') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_7 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_7.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_2.prid = Product.ProductID) AND (srsm_2.RegionId =" + region_id + ") AND (srsm_2.date < '" + StartDate + "')) AS SaleDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 WHERE (packing = 'Drum') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_1 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_1.Status) AND (RegionId =" + region_id + ") AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDrumAfter, (SELECT ISNULL(SUM(Orderdetail.qty), 0) AS Expr1 FROM Orderdetail INNER JOIN orderm ON Orderdetail.OrderID = orderm.OrderID WHERE (Orderdetail.packing = 'Dubbi') AND (Orderdetail.prid = Product.ProductID) AND (orderm.RegionId =" + region_id + ") AND (orderm.date < '" + StartDate + "')) AS SaleTwoDubbiBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Dubbi') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDubbiAfter, (SELECT ISNULL(SUM(Orderdetail_4.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_4 INNER JOIN orderm AS orderm_4 ON Orderdetail_4.OrderID = orderm_4.OrderID WHERE (Orderdetail_4.packing = 'Quarter') AND (Orderdetail_4.prid = Product.ProductID) AND (orderm_4.RegionId =" + region_id + ") AND (orderm_4.date < '" + StartDate + "')) AS SaleTwoQuarterBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Quarter') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoQuarterAfter, (SELECT ISNULL(SUM(Orderdetail_3.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_3 INNER JOIN orderm AS orderm_3 ON Orderdetail_3.OrderID = orderm_3.OrderID WHERE (Orderdetail_3.packing = 'Gallon') AND (Orderdetail_3.prid = Product.ProductID) AND (orderm_3.RegionId =" + region_id + ") AND (orderm_3.date < '" + StartDate + "')) AS SaleTwoGallonBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Gallon') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoGallonAfter, (SELECT ISNULL(SUM(Orderdetail_2.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_2 INNER JOIN orderm AS orderm_2 ON Orderdetail_2.OrderID = orderm_2.OrderID WHERE (Orderdetail_2.packing = 'Drum') AND (Orderdetail_2.prid = Product.ProductID) AND (orderm_2.RegionId =" + region_id + ") AND (orderm_2.date < '" + StartDate + "')) AS SaleTwoDrumBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Drum') AND (Orderdetail_1.prid = Product.ProductID) AND (orderm_1.RegionId =" + region_id + ") AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDrumAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (sampledetail2.packing = 'Dubbi') AND (sampledetail2.pid = Product.ProductID) AND (samplem2.regionid =" + region_id + ") AND (sampledetail2.date < '" + StartDate + "')) AS DemageDubbiBefore, (SELECT ISNULL(SUM(sampledetail2_7.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_7 INNER JOIN samplem2 AS samplem2_7 ON sampledetail2_7.invid = samplem2_7.invid WHERE (sampledetail2_7.packing = 'Dubbi') AND (sampledetail2_7.pid = Product.ProductID) AND (samplem2_7.regionid =" + region_id + ") AND (sampledetail2_7.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDubbiAfter, (SELECT ISNULL(SUM(sampledetail2_6.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_6 INNER JOIN samplem2 AS samplem2_6 ON sampledetail2_6.invid = samplem2_6.invid WHERE (sampledetail2_6.packing = 'Quarter') AND (sampledetail2_6.pid = Product.ProductID) AND (samplem2_6.regionid =" + region_id + ") AND (sampledetail2_6.date < '" + StartDate + "')) AS DemageQuarterBefore, (SELECT ISNULL(SUM(sampledetail2_5.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_5 INNER JOIN samplem2 AS samplem2_5 ON sampledetail2_5.invid = samplem2_5.invid WHERE (sampledetail2_5.packing = 'Quarter') AND (sampledetail2_5.pid = Product.ProductID) AND (samplem2_5.regionid =" + region_id + ") AND (sampledetail2_5.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageQuarterAfter, (SELECT ISNULL(SUM(sampledetail2_4.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_4 INNER JOIN samplem2 AS samplem2_4 ON sampledetail2_4.invid = samplem2_4.invid WHERE (sampledetail2_4.packing = 'Gallon') AND (sampledetail2_4.pid = Product.ProductID) AND (samplem2_4.regionid =" + region_id + ") AND (sampledetail2_4.date < '" + StartDate + "')) AS DemageGallonBefore, (SELECT ISNULL(SUM(sampledetail2_3.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_3 INNER JOIN samplem2 AS samplem2_3 ON sampledetail2_3.invid = samplem2_3.invid WHERE (sampledetail2_3.packing = 'Gallon') AND (sampledetail2_3.pid = Product.ProductID) AND (samplem2_3.regionid =" + region_id + ") AND (sampledetail2_3.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageGallonAfter, (SELECT ISNULL(SUM(sampledetail2_2.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_2 INNER JOIN samplem2 AS samplem2_2 ON sampledetail2_2.invid = samplem2_2.invid WHERE (sampledetail2_2.packing = 'Drum') AND (sampledetail2_2.pid = Product.ProductID) AND (samplem2_2.regionid =" + region_id + ") AND (sampledetail2_2.date < '" + StartDate + "')) AS DemageDrumBefore, (SELECT ISNULL(SUM(sampledetail2_1.qty), 0) AS Expr1 FROM sampledetail2 AS sampledetail2_1 INNER JOIN samplem2 AS samplem2_1 ON sampledetail2_1.invid = samplem2_1.invid WHERE (sampledetail2_1.packing = 'Drum') AND (sampledetail2_1.pid = Product.ProductID) AND (samplem2_1.regionid =" + region_id + ") AND (sampledetail2_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDrumAfter FROM Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))) AS derivedtbl_1) AS derivedtbl_2 ORDER BY ProductName";

            }
            else
            {
               query = "SELECT ProductID, ProductName, StkOpenDubbi, StkOpenQuarter, StkOpenGallon, StkOpenDrum, StkInDubbi, StkInQuarter, StkInGallon, StkInDrum, StkOutDubbi, StkOutQuarter, StkOutGallon, StkOutDrum, StkOpenDubbi + StkInDubbi - StkOutDubbi AS StkDubbi, StkOpenQuarter + StkInQuarter - StkOutQuarter AS StkQuarter, StkOpenGallon + StkInGallon - StkOutGallon AS StkGallon, StkOpenDrum + StkInDrum - StkOutDrum AS StkDrum FROM (SELECT ProductID, ProductName, OpeningDubbi + BatchFillingDubbiBefore + SaleReturnDubbiBefore - SaleDubbiBefore - SaleTwoDubbiBefore - DemageDubbiBefore - StockDistDubbiBefore AS StkOpenDubbi, OpeningQuarter + BatchFillingQuarterBefore + SaleReturnQuarterBefore - SaleQuarterBefore - SaleTwoQuarterBefore - DemageQuarterBefore - StockDistQuarterBefore AS StkOpenQuarter, OpeningGallon + BatchFillingGallonBefore + SaleReturnGallonBefore - SaleGallonBefore - SaleTwoGallonBefore - DemageGallonBefore - StockDistGallonBefore AS StkOpenGallon, OpeningDrum + BatchFillingDrumBefore + SaleReturnDrumBefore - SaleDrumBefore - SaleTwoDrumBefore - DemageDrumBefore - StockDistDrumBefore AS StkOpenDrum, BatchFillingDubbiAfter + SaleReturnDubbiAfter AS StkInDubbi, BatchFillingQuarterAfter + SaleReturnQuarterAfter AS StkInQuarter, BatchFillingGallonAfter + SaleReturnGallonAfter AS StkInGallon, BatchFillingDrumAfter + SaleReturnDrumAfter AS StkInDrum, SaleDubbiAfter + SaleTwoDubbiAfter + DemageDubbiAfter + StockDistDubbiAfter AS StkOutDubbi, SaleQuarterAfter + SaleTwoQuarterAfter + DemageQuarterAfter + StockDistQuarterAfter AS StkOutQuarter, SaleGallonAfter + SaleTwoGallonAfter + DemageGallonAfter + StockDistGallonAfter AS StkOutGallon, SaleDrumAfter + SaleTwoDrumAfter + DemageDrumAfter + StockDistDrumAfter AS StkOutDrum FROM (SELECT ProductID, ProductName, (SELECT ISNULL(SUM(dubi_o), 0) AS Expr1 FROM ProductFinishedRegion WHERE (pid = Product.ProductID " + str + ")) AS OpeningDubbi, (SELECT ISNULL(SUM(quarter_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_3 WHERE (pid = Product.ProductID " + str + ")) AS OpeningQuarter, (SELECT ISNULL(SUM(gallon_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_2 WHERE (pid = Product.ProductID " + str + ")) AS OpeningGallon, (SELECT ISNULL(SUM(drum_o), 0) AS Expr1 FROM ProductFinishedRegion AS ProductFinishedRegion_1 WHERE (pid = Product.ProductID " + str + ")) AS OpeningDrum, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date < '" + StartDate + "')) AS StockDistDubbiBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date < '" + StartDate + "')) AS StockDistQuarterBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date < '" + StartDate + "')) AS StockDistGallonBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date < '" + StartDate + "')) AS StockDistDrumBefore, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Dubbi') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDubbiAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Quarter') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistQuarterAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Gallon') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistGallonAfter, (SELECT ISNULL(SUM(sdd1.qty), 0) AS Expr1 FROM StockDistributionDetail1 sdd1 INNER JOIN StockDistribution sd ON sdd1.invid = sd.invid WHERE (sdd1.packing = 'Drum') AND (sdd1.pid = Product.ProductID) " + str + " AND (sd.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS StockDistDrumAfter, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDubbiBefore, (SELECT ISNULL(SUM(FilledDubbiQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDubbiAfter, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate < '" + StartDate + "')) AS BatchFillingQuarterBefore, (SELECT ISNULL(SUM(FilledQuarterQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingQuarterAfter, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate < '" + StartDate + "')) AS BatchFillingGallonBefore, (SELECT ISNULL(SUM(FilledGallonQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingGallonAfter, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_2 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate < '" + StartDate + "')) AS BatchFillingDrumBefore, (SELECT ISNULL(SUM(FilledDrumQTY), 0) AS Expr1 FROM tbl_BatchFillingMaster AS tbl_BatchFillingMaster_1 WHERE (ColorID = Product.ProductID) " + str + " AND (Fillingdate BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS BatchFillingDrumAfter, (SELECT ISNULL(SUM(srsdetail.qty), 0) AS Expr1 FROM srsdetail INNER JOIN srsm ON srsdetail.OrderID = srsm.OrderID AND srsdetail.Status = srsm.title WHERE (srsdetail.packing = 'Dubbi') AND (srsdetail.Status IN ('SRINV', 'TSRINV')) AND (srsdetail.prid = Product.ProductID) " + str + " AND (srsm.date < '" + StartDate + "')) AS SaleReturnDubbiBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Dubbi') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) " + str + " AND (title IN ('SRINV', 'TSRINV'))))) AS T) AS SaleReturnDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_4.prid = Product.ProductID) " + str + " AND (srsm_4.date < '" + StartDate + "')) AS SaleReturnQuarterBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Quarter') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) " + str + " AND (title IN ('SRINV', 'TSRINV'))))) AS T_7) AS SaleReturnQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_3.prid = Product.ProductID) " + str + " AND (srsm_3.date < '" + StartDate + "')) AS SaleReturnGallonBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Gallon') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) " + str + " AND (title IN ('SRINV', 'TSRINV'))))) AS T_6) AS SaleReturnGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SRINV', 'TSRINV')) AND (srsdetail_2.prid = Product.ProductID) " + str + " AND (srsm_2.date < '" + StartDate + "')) AS SaleReturnDrumBefore, (SELECT ISNULL(SUM(Expr1), 0) AS Expr1 FROM (SELECT DISTINCT OrderID, qty AS Expr1 FROM srsdetail AS srsdetail_6 WHERE (Status IN ('SRINV', 'TSRINV')) AND (packing = 'Drum') AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_6 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_6.Status) " + str + " AND (title IN ('SRINV', 'TSRINV'))))) AS T_5) AS SaleReturnDrumAfter, (SELECT ISNULL(SUM(srsdetail_5.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_5 INNER JOIN srsm AS srsm_5 ON srsdetail_5.OrderID = srsm_5.OrderID AND srsdetail_5.Status = srsm_5.title WHERE (srsdetail_5.packing = 'Dubbi') AND (srsdetail_5.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_5.prid = Product.ProductID) " + str + " AND (srsm_5.date < '" + StartDate + "')) AS SaleDubbiBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_9 WHERE (packing = 'Dubbi') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_9 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_9.Status) " + str + " AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDubbiAfter, (SELECT ISNULL(SUM(srsdetail_4.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_4 INNER JOIN srsm AS srsm_4 ON srsdetail_4.OrderID = srsm_4.OrderID AND srsdetail_4.Status = srsm_4.title WHERE (srsdetail_4.packing = 'Quarter') AND (srsdetail_4.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_4.prid = Product.ProductID) " + str + " AND (srsm_4.date < '" + StartDate + "')) AS SaleQuarterBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_8 WHERE (packing = 'Quarter') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_8 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_8.Status) " + str + " AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleQuarterAfter, (SELECT ISNULL(SUM(srsdetail_3.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_3 INNER JOIN srsm AS srsm_3 ON srsdetail_3.OrderID = srsm_3.OrderID AND srsdetail_3.Status = srsm_3.title WHERE (srsdetail_3.packing = 'Gallon') AND (srsdetail_3.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_3.prid = Product.ProductID) " + str + " AND (srsm_3.date < '" + StartDate + "')) AS SaleGallonBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_7 WHERE (packing = 'Gallon') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_7 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_7.Status) " + str + " AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleGallonAfter, (SELECT ISNULL(SUM(srsdetail_2.qty), 0) AS Expr1 FROM srsdetail AS srsdetail_2 INNER JOIN srsm AS srsm_2 ON srsdetail_2.OrderID = srsm_2.OrderID AND srsdetail_2.Status = srsm_2.title WHERE (srsdetail_2.packing = 'Drum') AND (srsdetail_2.Status IN ('SINV', 'TSINV', 'WTSINV')) AND (srsdetail_2.prid = Product.ProductID) " + str + " AND (srsm_2.date < '" + StartDate + "')) AS SaleDrumBefore, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail AS srsdetail_1 WHERE (packing = 'Drum') AND (Status IN ('SINV', 'TSINV', 'WTSINV')) AND (prid = Product.ProductID) AND (OrderID IN (SELECT OrderID FROM srsm AS srsm_1 WHERE (date BETWEEN '" + StartDate + "' AND '" + Enddate + "') AND (title = srsdetail_1.Status) " + str + " AND (title IN ('SINV', 'TSINV', 'WTSINV'))))) AS SaleDrumAfter, (SELECT ISNULL(SUM(Orderdetail.qty), 0) AS Expr1 FROM Orderdetail INNER JOIN orderm ON Orderdetail.OrderID = orderm.OrderID WHERE (Orderdetail.packing = 'Dubbi') AND (Orderdetail.prid = Product.ProductID) " + str + " AND (orderm.date < '" + StartDate + "')) AS SaleTwoDubbiBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Dubbi') AND (Orderdetail_1.prid = Product.ProductID) " + str + " AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDubbiAfter, (SELECT ISNULL(SUM(Orderdetail_4.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_4 INNER JOIN orderm AS orderm_4 ON Orderdetail_4.OrderID = orderm_4.OrderID WHERE (Orderdetail_4.packing = 'Quarter') AND (Orderdetail_4.prid = Product.ProductID) " + str + " AND (orderm_4.date < '" + StartDate + "')) AS SaleTwoQuarterBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Quarter') AND (Orderdetail_1.prid = Product.ProductID) " + str + " AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoQuarterAfter, (SELECT ISNULL(SUM(Orderdetail_3.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_3 INNER JOIN orderm AS orderm_3 ON Orderdetail_3.OrderID = orderm_3.OrderID WHERE (Orderdetail_3.packing = 'Gallon') AND (Orderdetail_3.prid = Product.ProductID) " + str + " AND (orderm_3.date < '" + StartDate + "')) AS SaleTwoGallonBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Gallon') AND (Orderdetail_1.prid = Product.ProductID) " + str + " AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoGallonAfter, (SELECT ISNULL(SUM(Orderdetail_2.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_2 INNER JOIN orderm AS orderm_2 ON Orderdetail_2.OrderID = orderm_2.OrderID WHERE (Orderdetail_2.packing = 'Drum') AND (Orderdetail_2.prid = Product.ProductID) " + str + " AND (orderm_2.date < '" + StartDate + "')) AS SaleTwoDrumBefore, (SELECT ISNULL(SUM(Orderdetail_1.qty), 0) AS Expr1 FROM Orderdetail AS Orderdetail_1 INNER JOIN orderm AS orderm_1 ON Orderdetail_1.OrderID = orderm_1.OrderID WHERE (Orderdetail_1.packing = 'Drum') AND (Orderdetail_1.prid = Product.ProductID) " + str + " AND (orderm_1.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS SaleTwoDrumAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Dubbi') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date < '" + StartDate + "')) AS DemageDubbiBefore, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Dubbi') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDubbiAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Quarter') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date < '" + StartDate + "')) AS DemageQuarterBefore, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Quarter') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageQuarterAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Gallon') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date < '" + StartDate + "')) AS DemageGallonBefore, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Gallon') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageGallonAfter, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Drum') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date < '" + StartDate + "')) AS DemageDrumBefore, (SELECT ISNULL(SUM(sampledetail2.qty), 0) AS Expr1 FROM sampledetail2 INNER JOIN samplem2 ON sampledetail2.invid = samplem2.invid WHERE (packing = 'Drum') AND (pid = Product.ProductID) " + str + " AND (sampledetail2.date BETWEEN '" + StartDate + "' AND '" + Enddate + "')) AS DemageDrumAfter FROM Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))) AS derivedtbl_1) AS derivedtbl_2 ORDER BY ProductName";
            }


           // var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT pid, pname, du - tdu AS dubbi, qu - tqu AS quarter, ga - tga AS gallon, dr - tdr AS drum, qty - tqty AS Qty FROM(SELECT pid, pname, ISNULL(SUM(dubbi), 0) AS du, ISNULL(SUM(quarter), 0) AS qu, ISNULL(SUM(gallon), 0) AS ga, ISNULL(SUM(drum), 0) AS dr, ISNULL(SUM(totalqty), 0) AS qty, ISNULL(SUM(dubbi_c), 0) AS tdu, ISNULL(SUM(quarter_c), 0) AS tqu, ISNULL(SUM(gallon_c), 0) AS tga, ISNULL(SUM(drum_c), 0) AS tdr, ISNULL(SUM(totalqty_c), 0) AS tqty FROM ProductionOrderDetail GROUP BY pid, pname) AS table1 ").ToList();
          //  var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT pid AS ProductID1, pname AS ProductName, du AS dubbi_o, qu AS quarter_o, ga AS gallon_o, dr AS drum_o, qty AS qty_o, tdu AS dubbi_c, tqu AS quarter_c, tga AS gallon_c, tdr AS drum_c, tqty AS qty_c, du - tdu AS dubbi, qu - tqu AS quarter, ga - tga AS gallon, dr - tdr AS drum, qty - tqty AS Qty1 FROM (SELECT pid, pname, ISNULL(SUM(dubbi), 0) AS du, ISNULL(SUM(quarter), 0) AS qu, ISNULL(SUM(gallon), 0) AS ga, ISNULL(SUM(drum), 0) AS dr, ISNULL(SUM(totalqty), 0) AS qty, ISNULL(SUM(dubbi_c), 0) AS tdu, ISNULL(SUM(quarter_c), 0) AS tqu, ISNULL(SUM(gallon_c), 0) AS tga, ISNULL(SUM(drum_c), 0) AS tdr, ISNULL(SUM(totalqty_c), 0) AS tqty FROM ProductionOrderDetail GROUP BY pid, pname) AS table1 ").ToList();
            var getgrosspackage = _context.Database.SqlQuery<ProductCapShow>("SELECT pid AS ProductID1, pname AS ProductName, du AS dubbi_o, qu AS quarter_o, ga AS gallon_o, dr AS drum_o, qty AS qty_o, tdu AS dubbi_c, tqu AS quarter_c, tga AS gallon_c, tdr AS drum_c, tqty  AS qty_c, du - tdu AS dubbi, qu - tqu AS quarter, ga - tga AS gallon, dr - tdr AS drum, qty - tqty AS Qty1, rqty AS qty_r, mqty AS qty_m FROM (SELECT pid, pname, ISNULL(SUM(dubbi), 0) AS du, ISNULL(SUM(quarter), 0) AS qu, ISNULL(SUM(gallon), 0) AS ga, ISNULL(SUM(drum), 0) AS dr, ISNULL(SUM(totalqty), 0) AS qty, ISNULL(SUM(dubbi_c), 0) AS tdu, ISNULL(SUM(quarter_c), 0) AS tqu, ISNULL(SUM(gallon_c), 0) AS tga, ISNULL(SUM(drum_c), 0) AS tdr, ISNULL(SUM(totalqty_c), 0) AS tqty, ISNULL(SUM(totalqty_r), 0) AS rqty, ISNULL(SUM(totalqty_m), 0) AS mqty FROM ProductionOrderDetail              WHERE  (totalqty_r <> totalqty_m) OR  (dubbi <> dubbi_c) OR  (quarter <> quarter_c) OR  (gallon <> gallon_c) OR (drum <> drum_c) GROUP BY pid, pname) AS table1 ").ToList();

            var item_ledger = _context.Database.SqlQuery<FinishedGoodsQuery2>(query).ToList();

            ViewBag.getgrosspackage = getgrosspackage.ToList();
            ViewBag.type = type;
        
            return View(item_ledger);
        
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