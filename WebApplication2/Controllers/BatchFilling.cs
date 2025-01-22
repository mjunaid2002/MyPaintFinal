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
using System.Text.RegularExpressions;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class BatchFillingController : Controller
    {
        private ApplicationDbContext _context;
        private List<PoDetail> getgrosspackage;

        public BatchFillingController()
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

            string strquery = " where fillingdate ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where fillingdate between '" + StartDate + "' and '" + Enddate + "'  ";

            var list = _context.Database.SqlQuery<BatchFillingMasterQUery>("SELECT (select top(1) batchn from tbl_BatchFillingBatches where sr=tbl_BatchFillingMaster.id) as batchn ,*  from tbl_BatchFillingMaster " + strquery).ToList();
            return View(list);
        }
        public ActionResult Indexstatus()
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                //var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2,tbl_BatchRequest.username,tbl_BatchRequest.userid FROM ProductIngrMaster INNER JOIN tbl_BatchRequest ON ProductIngrMaster.OrderId = tbl_BatchRequest.orderid WHERE (tbl_BatchRequest.status = 'Requested') ORDER BY ProductIngrMaster.Ready, ProductIngrMaster.Date, ProductIngrMaster.OrderId").ToList();
                //return View(list);
                var list = _context.Database.SqlQuery<BatchFillingMasterQUery>("SELECT (select top(1) batchn from tbl_BatchFillingBatches where sr=tbl_BatchFillingMaster.id) as batchn,tbl_BatchRequest.username,tbl_BatchRequest.userid ,*  from tbl_BatchFillingMaster INNER JOIN  tbl_BatchRequest ON tbl_BatchFillingMaster.Id = tbl_BatchRequest.orderid WHERE (tbl_BatchRequest.status = 'Requested') AND department='Filling'").ToList();
                return View(list);
            }
            return RedirectToAction("Login", "Home");
        }
        public ActionResult InvoiceReport(int ID)
        {
            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT tbl_section.name, tbl_section.id as sectionid, ProductIngrDetail.Id, ProductIngrDetail.ItemId, ProductIngrDetail.ItemName, ProductIngrDetail.weight, ProductIngrDetail.percentage, ProductIngrDetail.Rate, ProductIngrDetail.Cost, ProductIngrDetail.OrderId, ProductIngrDetail.Date, ProductIngrDetail.status FROM ProductIngrDetail INNER JOIN tbl_section ON ProductIngrDetail.status = tbl_section.id where OrderId=" + ID + "").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var ColorName = _context.Database.SqlQuery<string>("SELECT ColorName FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var ColorNo = _context.Database.SqlQuery<string>("SELECT ColorNo FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var Quality = _context.Database.SqlQuery<string>("SELECT Quality FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var BatchNo = _context.Database.SqlQuery<string>("SELECT BatchNo FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var ProductName = _context.Database.SqlQuery<string>("SELECT ProductName FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var ph = _context.Database.SqlQuery<string>("SELECT ph FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var note = _context.Database.SqlQuery<string>("SELECT note FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var kgcoast = _context.Database.SqlQuery<decimal>("SELECT kgcoast FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var ltrcoast = _context.Database.SqlQuery<decimal>("SELECT ltrcoast FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var viscosity = _context.Database.SqlQuery<decimal>("SELECT viscosity FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var whc = _context.Database.SqlQuery<decimal>("SELECT whc FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var yield = _context.Database.SqlQuery<decimal>("SELECT yield FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var totalweight = _context.Database.SqlQuery<decimal>("SELECT totalweight FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptbatch.rpt"));

            rd.SetDataSource(list);

            rd.SetParameterValue("OrderId", ID);
            rd.SetParameterValue("date", date);
            rd.SetParameterValue("ColorName", ColorName);
            rd.SetParameterValue("ColorNo", ColorNo);
            rd.SetParameterValue("Quality", Quality);
            rd.SetParameterValue("BatchNo", BatchNo);
            rd.SetParameterValue("ProductName", ProductName);
            rd.SetParameterValue("ph", ph);
            rd.SetParameterValue("note", note);
            rd.SetParameterValue("kgcoast", kgcoast);
            rd.SetParameterValue("ltrcoast", ltrcoast);
            rd.SetParameterValue("viscosity", viscosity);
            rd.SetParameterValue("whc", whc);
            rd.SetParameterValue("yield", yield);
            rd.SetParameterValue("totalweight", totalweight);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "BatchReport.pdf");
        }
        [HttpPost]
        public ActionResult Action7(string code)
        {
            var getgrosspackage = _context.Database.SqlQuery<BatchReceivingQuery>("select kgcoast as cost from ProductIngrMaster where BatchNo = '" + code + "'").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

         public ActionResult Create(BatchFillingMasterQUery batchFillingMasterQUery)
        {
            batchFillingMasterQUery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from tbl_BatchFillingMaster").FirstOrDefault();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var color1 = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and   IsPacking=1) ").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
         
            batchFillingMasterQUery.ColorName = "0";
            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                color = color,
                color1 = color1,
                batchFillingMasterQUery = batchFillingMasterQUery,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name2, string[] batch, decimal[] f_qty, decimal[] w_qty, decimal[] cost, string[] packing, decimal[] weight2, decimal[] rate1, decimal[] cost2, BatchFillingMasterQUery batchFillingMasterQUery)
        {
            batchFillingMasterQUery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from tbl_BatchFillingMaster").FirstOrDefault();

            decimal ProductID, id;
            string sqlQuery = "";
            for (int i = 0; i < batch.Count(); i++)
            {
                id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingBatches").FirstOrDefault();
                sqlQuery = sqlQuery + "INSERT INTO tbl_BatchFillingBatches (Id,sr,batchn,filled,waste,cost,date) VALUES (" + id + "," + batchFillingMasterQUery.Id + ",'" + batch[i] + "'," + f_qty[i] + "," + w_qty[i] + "," + cost[i] + ",'" + batchFillingMasterQUery.Fillingdate + "');";
            }
            for (int i = 0; i < packing.Count(); i++)
            {
                ProductID = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + item_name2[i].Trim() + "'").FirstOrDefault();
                id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingDetail").FirstOrDefault();
                sqlQuery = sqlQuery + "INSERT INTO tbl_BatchFillingDetail (Id,sr,ItemId,ItemName,type,qty,Rate,Total,Date) VALUES (" + id + "," + batchFillingMasterQUery.Id + "," + ProductID + ",'" + item_name2[i] + "','" + packing[i] + "'," + weight2[i] + "," + rate1[i] + "," + cost2[i] + ",'" + batchFillingMasterQUery.Fillingdate + "');";
            }

            ProductID = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchFillingMasterQUery.ColorName.Trim() + "'").FirstOrDefault();
            sqlQuery = sqlQuery + "INSERT INTO tbl_BatchFillingMaster (status,RegionId,Id,Fillingdate,ColorName,ColorID,FilledDubbiQTY,FilledDubbiWeight,SampleDubbiQTY,SampleDubbiWeight,FilledQuarterQTY,FilledQuarterWeight,SampleQuarterQTY,SampleQuarterWeight,FilledGallonQTY,FilledGallonWeight,SampleGallonQTY,SampleGallonWeight,FilledDrumQTY,FilledDrumWeight,SampleDrumQTY,SampleDrumWeight,supervisedby,filledby,filledby1,totalfillweight,yeild,batchespercost,packingcost,batchcost,totalcost) VALUES" +
                " ('Request'," + batchFillingMasterQUery.RegionId + "," + batchFillingMasterQUery.Id + ",'" + batchFillingMasterQUery.Fillingdate + "','" + batchFillingMasterQUery.ColorName + "'," + ProductID + "," + batchFillingMasterQUery.FilledDubbiQTY + "," + batchFillingMasterQUery.FilledDubbiWeight + "" +
                "," + batchFillingMasterQUery.SampleDubbiQTY + "," + batchFillingMasterQUery.SampleDubbiWeight + "," + batchFillingMasterQUery.FilledQuarterQTY + "," + batchFillingMasterQUery.FilledQuarterWeight + "," + batchFillingMasterQUery.SampleQuarterQTY + "," + batchFillingMasterQUery.SampleQuarterWeight + "," + batchFillingMasterQUery.FilledGallonQTY + "," + batchFillingMasterQUery.FilledGallonWeight + "," + batchFillingMasterQUery.SampleGallonQTY + "," +
                "" + batchFillingMasterQUery.SampleGallonWeight + "," + batchFillingMasterQUery.FilledDrumQTY + "," + batchFillingMasterQUery.FilledDrumWeight + "," + batchFillingMasterQUery.SampleDrumQTY + "," + batchFillingMasterQUery.SampleDrumWeight + ",'" + batchFillingMasterQUery.supervisedby + "','" + batchFillingMasterQUery.filledby + "','" + batchFillingMasterQUery.filledby1 + "'," + batchFillingMasterQUery.totalfillweight + "," + batchFillingMasterQUery.yeild + "," + batchFillingMasterQUery.batchespercost + "," + batchFillingMasterQUery.packingcost + "," + batchFillingMasterQUery.batchcost + "," + batchFillingMasterQUery.totalcost + ");";

            _context.Database.ExecuteSqlCommand(sqlQuery);

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var batchFillingMasterQUery = _context.Database.SqlQuery<BatchFillingMasterQUery>("select *,(SELECT TOP (1) CapDubbi FROM Product WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Dubi,(SELECT TOP (1) CapQuarter FROM Product AS Product_3 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Quarter,(SELECT TOP (1) CapGallon FROM Product AS Product_2 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Gallon,(SELECT TOP (1) CapDrum FROM Product AS Product_1 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Drum from tbl_BatchFillingMaster where id =" + ID + "").SingleOrDefault();
            batchFillingMasterQUery.Dubi_tot = batchFillingMasterQUery.FilledDubbiQTY * batchFillingMasterQUery.Dubi;
            batchFillingMasterQUery.Quarter_tot = batchFillingMasterQUery.FilledQuarterQTY * batchFillingMasterQUery.Quarter;
            batchFillingMasterQUery.Gallon_tot = batchFillingMasterQUery.FilledGallonQTY * batchFillingMasterQUery.Gallon;
            batchFillingMasterQUery.Drum_tot = batchFillingMasterQUery.FilledDrumQTY * batchFillingMasterQUery.Drum;
            
            batchFillingMasterQUery.total_packing_dubbi = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Dubbi')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_quarter = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Quarter')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_gallon = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Gallon')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_drum = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Drum')").SingleOrDefault();
            
            batchFillingMasterQUery.batchespercost = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(cost), 0) AS Expr1 FROM tbl_BatchFillingBatches WHERE (sr = " + ID + ")").SingleOrDefault();

            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var color1 = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and   IsPacking=1) ").ToList();
            batchFillingMasterQUery.ColorID = 0;
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var batchfillingdetail = _context.Database.SqlQuery<BatchFillingDetailQUery>("SELECT * from tbl_BatchFillingDetail where sr=" + ID + "").ToList();
            var BatchFillingBatches = _context.Database.SqlQuery<BatchFillingBatches>("SELECT * from tbl_BatchFillingBatches where sr=" + ID + "").ToList();
            decimal sumyield = 0;
            foreach(var Bfill in BatchFillingBatches)
            {
                sumyield = sumyield + Bfill.filled - Bfill.waste;
            }
            batchFillingMasterQUery.yeild = Math.Round(batchFillingMasterQUery.totalfillweight * 100 / sumyield,2);

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                color = color,
                color1 = color1,
                batchFillingMasterQUery = batchFillingMasterQUery,
                batchfillingdetail = batchfillingdetail,
                BatchFillingBatches = BatchFillingBatches,
                rawtotal = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr =" + ID + ")").FirstOrDefault()
        };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Update(string[] item_name2, string[] batch, decimal[] f_qty, decimal[] w_qty, decimal[] cost, string[] packing, decimal[] weight2, decimal[] rate1, decimal[] cost2, BatchFillingMasterQUery batchFillingMasterQUery)
        {
            decimal ProductID;
            _context.Database.ExecuteSqlCommand("Delete From tbl_BatchFillingDetail where sr =" + batchFillingMasterQUery.Id + "");
            _context.Database.ExecuteSqlCommand("Delete From tbl_BatchFillingBatches where sr =" + batchFillingMasterQUery.Id + "");
            _context.Database.ExecuteSqlCommand("Delete From tbl_BatchFillingMaster where id =" + batchFillingMasterQUery.Id + "");
            for (int i = 0; i < batch.Count(); i++)
            {
                decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingBatches").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchFillingBatches (Id,sr,batchn,filled,waste,cost,date) VALUES (" + id + "," + batchFillingMasterQUery.Id + ",'" + batch[i] + "'," + f_qty[i] + "," + w_qty[i] + "," + cost[i] + ",'" + batchFillingMasterQUery.Fillingdate + "')");
            }
            for (int i = 0; i < packing.Count(); i++)
            {
                ProductID = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + item_name2[i].Trim() + "'").FirstOrDefault();
                decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingDetail").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchFillingDetail (Id,sr,ItemId,ItemName,type,qty,Rate,Total,Date) VALUES (" + id + "," + batchFillingMasterQUery.Id + "," + ProductID + ",'" + item_name2[i] + "','" + packing[i] + "'," + weight2[i] + "," + rate1[i] + "," + cost2[i] + ",'" + batchFillingMasterQUery.Fillingdate + "')");
            }

            ProductID = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchFillingMasterQUery.ColorName.Trim() + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchFillingMaster (status,RegionId,Id,Fillingdate,ColorName,ColorID,FilledDubbiQTY,FilledDubbiWeight,SampleDubbiQTY,SampleDubbiWeight,FilledQuarterQTY,FilledQuarterWeight,SampleQuarterQTY,SampleQuarterWeight,FilledGallonQTY,FilledGallonWeight,SampleGallonQTY,SampleGallonWeight,FilledDrumQTY,FilledDrumWeight,SampleDrumQTY,SampleDrumWeight,supervisedby,filledby,filledby1,totalfillweight,yeild,batchespercost,packingcost,batchcost,totalcost) VALUES" +
                " ('Request'," + batchFillingMasterQUery.RegionId + "," + batchFillingMasterQUery.Id + ",'" + batchFillingMasterQUery.Fillingdate + "','" + batchFillingMasterQUery.ColorName + "'," + ProductID + "," + batchFillingMasterQUery.FilledDubbiQTY + "," + batchFillingMasterQUery.FilledDubbiWeight + "" +
                "," + batchFillingMasterQUery.SampleDubbiQTY + "," + batchFillingMasterQUery.SampleDubbiWeight + "," + batchFillingMasterQUery.FilledQuarterQTY + "," + batchFillingMasterQUery.FilledQuarterWeight + "," + batchFillingMasterQUery.SampleQuarterQTY + "," + batchFillingMasterQUery.SampleQuarterWeight + "," + batchFillingMasterQUery.FilledGallonQTY + "," + batchFillingMasterQUery.FilledGallonWeight + "," + batchFillingMasterQUery.SampleGallonQTY + "," +
                "" + batchFillingMasterQUery.SampleGallonWeight + "," + batchFillingMasterQUery.FilledDrumQTY + "," + batchFillingMasterQUery.FilledDrumWeight + "," + batchFillingMasterQUery.SampleDrumQTY + "," + batchFillingMasterQUery.SampleDrumWeight + ",'" + batchFillingMasterQUery.supervisedby + "','" + batchFillingMasterQUery.filledby + "','" + batchFillingMasterQUery.filledby1 + "'," + batchFillingMasterQUery.totalfillweight + "," + batchFillingMasterQUery.yeild + "," + batchFillingMasterQUery.batchespercost + "," + batchFillingMasterQUery.packingcost + "," + batchFillingMasterQUery.batchcost + "," + batchFillingMasterQUery.totalcost + ")");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select itmdisc,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where ProductID = " + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCatNew(string value)
        {
            var allow_list = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT TOP (1) ISNULL((SELECT TOP (1) kgcoast FROM ProductIngrMaster WHERE     (ColorName = '" + value + "')), 0) AS cost, CapDubbi AS Dubi, CapQuarter AS Quarter, CapGallon AS Gallon, CapDrum AS Drum, 0 AS total_cost FROM         Product WHERE     (ProductName = '" + value + "')").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCat(string value)
        {
            var allow_list = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT isnull((select Top(1) kgcoast from ProductIngrMaster where BatchNo=tbl_BatchReceiving.batch and ColorName='" + value + "'),0) as cost,isnull((select Top(1) CapDubbi from Product where ProductID=tbl_BatchReceiving.pid),0) as dubbi, isnull((select Top(1) isnull(CapQuarter,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as quarter, isnull((select Top(1) isnull(CapGallon,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as Gallon, isnull((select Top(1) isnull(CapDrum,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as Drum, * from tbl_BatchReceiving where color = '" + value + "' ORDER BY batch DESC").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetRemqtyTotal(decimal itemId,string itemPacking)
        {
            //getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0)-(SELECT     ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE      (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = " + itemId + ")) AS derivedtbl_1").ToList();
            List<decimal> targetIds = new List<decimal> { 2685, 2687, 2689, 2691, 2693, 2695, 2697, 2699, 2701, 2703, 2705, 2707, 2831, 2832, 2833 };
            //var str = "SELECT vattax + PurchaseIn - PurchaseRtnOut - SaleOut + SaleReturnIn - BatchStockOut - wastageout - wastageout2 AS qty FROM (SELECT TOP (1) Product.ProductID, Product.vattax, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM purchasedetail WHERE (pid = Product.ProductID)) AS PurchaseIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srpdetail WHERE (pid = Product.ProductID)) AS PurchaseRtnOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM Order_detail WHERE (prid = Product.ProductID)) AS SaleOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)) AS SaleReturnIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)) AS BatchStockOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS wastageout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail2 WHERE (pid = Product.ProductID)) AS wastageout2 FROM Product INNER JOIN ProductExtend ON Product.ProductID = ProductExtend.ProductID WHERE (Product.ProductID = " + itemId + ") AND (ProductExtend.ProductType = '" + itemPacking + "')) AS derivedtbl_1";
            if (targetIds.Contains(itemId))
            {

                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT vattax + PurchaseIn - PurchaseRtnOut - SaleOut + SaleReturnIn - BatchStockOut - wastageout - wastageout2 AS qty FROM (SELECT TOP (1) Product.ProductID, Product.vattax, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM purchasedetail WHERE (pid = Product.ProductID)) AS PurchaseIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srpdetail WHERE (pid = Product.ProductID)) AS PurchaseRtnOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM Order_detail WHERE (prid = Product.ProductID)) AS SaleOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)) AS SaleReturnIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)) AS BatchStockOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS wastageout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail2 WHERE (pid = Product.ProductID)) AS wastageout2 FROM Product INNER JOIN ProductExtend ON Product.ProductID = ProductExtend.ProductID WHERE (Product.ProductID = " + itemId + ") ) AS derivedtbl_1").ToList();
            }
            else
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT vattax + PurchaseIn - PurchaseRtnOut - SaleOut + SaleReturnIn - BatchStockOut - wastageout - wastageout2 AS qty FROM (SELECT TOP (1) Product.ProductID, Product.vattax, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM purchasedetail WHERE (pid = Product.ProductID)) AS PurchaseIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srpdetail WHERE (pid = Product.ProductID)) AS PurchaseRtnOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM Order_detail WHERE (prid = Product.ProductID)) AS SaleOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM srsdetail WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)) AS SaleReturnIn, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)) AS BatchStockOut, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS wastageout, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail2 WHERE (pid = Product.ProductID)) AS wastageout2 FROM Product INNER JOIN ProductExtend ON Product.ProductID = ProductExtend.ProductID WHERE (Product.ProductID = " + itemId + ") AND (ProductExtend.ProductType = '" + itemPacking + "')) AS derivedtbl_1").ToList();

            }



            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From srpm where InvId =" + ID + " and status='TPRINV' ");
            _context.Database.ExecuteSqlCommand("Delete From srpdetail where InvId =" + ID + "  and Stauts='TPRINV' ");
            return RedirectToAction("Index");
        }
        public ActionResult statusrequest(int? ID, string batchno)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchFillingMaster SET  status ='Pending'  where id="+ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','" + batchno + "','Filling')");
            return RedirectToAction("Index");
        }

        public ActionResult GetFillingBatchCount()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Filling").Count();
            ViewBag.BatchCount = batchCount;
            return PartialView("GetFillingBatchCount");
        }
        [HttpGet]
        public JsonResult GetFillingBatchCountJson()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Filling").Count();
            return Json(new { batchCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchFillingMaster SET status = 'Complete' where   Id =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Filling' AND  userid='" + userid + "' AND  OrderId =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchFillingMaster SET status = 'Request' where   Id =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Filling' AND userid='" + userid + "' AND  OrderId =" + ID);
                }


                return RedirectToAction("Indexstatus");
            }
            return RedirectToAction("Login", "Home");
        }

        private object NumberToWords(string number)
        {
            throw new NotImplementedException();
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            string new_word = Regex.Replace(words, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            return new_word;
        }

        public ActionResult ViewBatch(int? ID)
        {
            var batchFillingMasterQUery = _context.Database.SqlQuery<BatchFillingMasterQUery>("select *,(SELECT TOP (1) CapDubbi FROM Product WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Dubi,(SELECT TOP (1) CapQuarter FROM Product AS Product_3 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Quarter,(SELECT TOP (1) CapGallon FROM Product AS Product_2 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Gallon,(SELECT TOP (1) CapDrum FROM Product AS Product_1 WHERE (ProductID= tbl_BatchFillingMaster.ColorID)) AS Drum from tbl_BatchFillingMaster where id =" + ID + "").SingleOrDefault();
            batchFillingMasterQUery.Dubi_tot = batchFillingMasterQUery.FilledDubbiQTY * batchFillingMasterQUery.Dubi;
            batchFillingMasterQUery.Quarter_tot = batchFillingMasterQUery.FilledQuarterQTY * batchFillingMasterQUery.Quarter;
            batchFillingMasterQUery.Gallon_tot = batchFillingMasterQUery.FilledGallonQTY * batchFillingMasterQUery.Gallon;
            batchFillingMasterQUery.Drum_tot = batchFillingMasterQUery.FilledDrumQTY * batchFillingMasterQUery.Drum;

            batchFillingMasterQUery.total_packing_dubbi = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Dubbi')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_quarter = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Quarter')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_gallon = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Gallon')").SingleOrDefault();
            batchFillingMasterQUery.total_packing_drum = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr = " + ID + ") AND (ItemId IN (SELECT ProductID FROM Product WHERE (CategoryID = 43))) AND (type = 'Drum')").SingleOrDefault();

            batchFillingMasterQUery.batchespercost = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(cost), 0) AS Expr1 FROM tbl_BatchFillingBatches WHERE (sr = " + ID + ")").SingleOrDefault();

            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var color1 = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and   IsPacking=1) ").ToList();
            batchFillingMasterQUery.ColorID = 0;
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var batchfillingdetail = _context.Database.SqlQuery<BatchFillingDetailQUery>("SELECT * from tbl_BatchFillingDetail where sr=" + ID + "").ToList();
            var BatchFillingBatches = _context.Database.SqlQuery<BatchFillingBatches>("SELECT * from tbl_BatchFillingBatches where sr=" + ID + "").ToList();
            decimal sumyield = 0;
            foreach (var Bfill in BatchFillingBatches)
            {
                sumyield = sumyield + Bfill.filled - Bfill.waste;
            }
            batchFillingMasterQUery.yeild = Math.Round(batchFillingMasterQUery.totalfillweight * 100 / sumyield, 2);

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                color = color,
                color1 = color1,
                batchFillingMasterQUery = batchFillingMasterQUery,
                batchfillingdetail = batchfillingdetail,
                BatchFillingBatches = BatchFillingBatches,
                rawtotal = _context.Database.SqlQuery<decimal>("SELECT ISNULL(SUM(Total), 0) AS Expr1 FROM tbl_BatchFillingDetail WHERE (sr =" + ID + ")").FirstOrDefault()
            };
            return View(SaleInvVM);
        }
    }
}