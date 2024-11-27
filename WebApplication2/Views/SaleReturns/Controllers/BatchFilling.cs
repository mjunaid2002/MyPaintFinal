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
            var list = _context.Database.SqlQuery<BatchFillingMasterQUery>("SELECT (select top(1) batchn from tbl_BatchFillingBatches where id=tbl_BatchFillingMaster.id) as batchn ,*  from tbl_BatchFillingMaster").ToList();
            return View(list);
        }
        public ActionResult InvoiceReport(int ID)
        {
            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT tbl_section.name, tbl_section.id as sectionid, ProductIngrDetail.Id, ProductIngrDetail.ItemId, ProductIngrDetail.ItemName, ProductIngrDetail.weight, ProductIngrDetail.percentage, ProductIngrDetail.Rate, ProductIngrDetail.Cost, ProductIngrDetail.OrderId, ProductIngrDetail.Date, ProductIngrDetail.status FROM ProductIngrDetail INNER JOIN tbl_section ON ProductIngrDetail.status = tbl_section.id where OrderId="+ID+"").ToList();
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

        public ActionResult Create(BatchFillingMasterQUery batchFillingMasterQUery)
        {
            batchFillingMasterQUery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from tbl_BatchFillingMaster").FirstOrDefault();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=0) ").ToList();
            var color1 = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and   IsPacking=1) ").ToList();
            var SaleInvVM = new SaleInvVM
            {
                color = color,
                color1 = color1,
                batchFillingMasterQUery = batchFillingMasterQUery,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] batch, decimal[] f_qty, decimal[] w_qty, decimal[] cost, string[] packing, decimal[] weight2, decimal[] rate1, decimal[] cost2, BatchFillingMasterQUery batchFillingMasterQUery)
        {
            for (int i = 0; i < batch.Count(); i++)
            {
                decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingBatches").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchFillingBatches (Id,sr,batchn,filled,waste,cost,date) VALUES (" + id + "," + i + ",'" + batch[i] + "'," + f_qty[i] + "," + w_qty[i] + "," + cost[i] + ",'" + batchFillingMasterQUery.Fillingdate+ "')");
            }
            for (int i = 0; i < packing.Count(); i++)
            {
                decimal ProductID = _context.Database.SqlQuery<decimal>("select ProductID from Product where ProductName ='" + batchFillingMasterQUery.ColorName + "'").FirstOrDefault();
                decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchFillingDetail").FirstOrDefault();
                _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchFillingDetail (Id,sr,ItemId,ItemName,type,qty,Rate,Total,Date) VALUES (" + id + "," + batchFillingMasterQUery.Id + "," + ProductID + ",'" + batchFillingMasterQUery.ColorName + "','" + packing[i] + "'," + weight2[i] + "," + rate1[i] + "," + cost2[i] + ",'" + batchFillingMasterQUery.Fillingdate + "')");
            }
            //_context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrMaster (Ready,machineoper2,machineoper1,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,totalweight,yield,ph,Prdwaste,testby,transferedby,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,FilledDrum,FilledGallon,FilledQuarter,FilledDubbi,totalweightreceived,SampleQTY,SampleWeight,transfered,Pkgwaste,pid) VALUES" +
            //    " ('"+Request["ready"] +"','"+productIngrMasterQuery.machineoper2 + "','"+productIngrMasterQuery.machineoper1 + "'," + productIngrMasterQuery.OrderId + ",'" + productIngrMasterQuery.Quality + "','" + Request["ProductName"] + "','" + Request["ColorName"] + "'" +
            //    ",'" + productIngrMasterQuery.BatchNo + "',"+ productIngrMasterQuery.ltrcoast + ","+ productIngrMasterQuery.kgcoast + "," + productIngrMasterQuery.viscosity + "," + productIngrMasterQuery.whc + "," +
            //    "" + productIngrMasterQuery.totalweight + "," + productIngrMasterQuery.yield + ",'" + productIngrMasterQuery.ph + "'," + productIngrMasterQuery.Prdwaste + "," +
            //    "'" + productIngrMasterQuery.testby + "','" + productIngrMasterQuery.transferedby + "','"+ productIngrMasterQuery.note + "',0,0,0,0,'"+DateTime.Now.ToString("yyyy-MM-dd")+"','"+DateTime.Now+"',0,0,0,0,0,0,0,0,0,0)");

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var ProductIngrMasterQuery = _context.Database.SqlQuery<ProductIngrMasterQuery>("select * from ProductIngrMaster where OrderId =" + ID + "").SingleOrDefault();
            var ProductIngrDetailQuery = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT * from ProductIngrDetail where OrderId=" + ID + "").ToList();

            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var product = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and IsPacking=0)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                quality = quality,
                product = product,
                color = color,
                productIngrMasterQuery = ProductIngrMasterQuery,
                ProductIngrDetailQuery = ProductIngrDetailQuery,

            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(int[] id1, string[] item_name1, string[] date1, decimal[] weight1, decimal[] per1, decimal[] rate1, decimal[] cost1, int[] id2, string[] item_name2, string[] date2, decimal[] weight2, decimal[] per2, decimal[] rate2, decimal[] cost2, ProductIngrMasterQuery productIngrMasterQuery)
        {

            _context.Database.ExecuteSqlCommand("Delete From ProductIngrDetail where OrderId =" + productIngrMasterQuery.OrderId + "");
            _context.Database.ExecuteSqlCommand("Delete From ProductIngrMaster where OrderId =" + productIngrMasterQuery.OrderId + "");
            decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from ProductIngrDetail").FirstOrDefault();
            for (int i = 0; i < item_name1.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id1[i] + ",'" + item_name1[i] + "'," + weight1[i] + "," + per1[i] + "," + rate1[i] + "," + cost1[i] + "," + productIngrMasterQuery.OrderId + ",'" + date1[i] + "',0)");
            }
            for (int i = 0; i < item_name2.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id2[i] + ",'" + item_name2[i] + "'," + weight2[i] + "," + per2[i] + "," + rate2[i] + "," + cost2[i] + "," + productIngrMasterQuery.OrderId + ",'" + date2[i] + "',1)");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrMaster (machineoper2,machineoper1,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,totalweight,yield,ph,Prdwaste,testby,transferedby,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,Ready,FilledDrum,FilledGallon,FilledQuarter,FilledDubbi,totalweightreceived,SampleQTY,SampleWeight,transfered,Pkgwaste,pid) VALUES" +
                " ('" + productIngrMasterQuery.machineoper2 + "','" + productIngrMasterQuery.machineoper1 + "'," + productIngrMasterQuery.OrderId + ",'" + productIngrMasterQuery.Quality + "','" + productIngrMasterQuery.ProductName + "','" + productIngrMasterQuery.ColorName + "'" +
                ",'" + productIngrMasterQuery.BatchNo + "'," + productIngrMasterQuery.ltrcoast + "," + productIngrMasterQuery.kgcoast + "," + productIngrMasterQuery.viscosity + "," + productIngrMasterQuery.whc + "," +
                "" + productIngrMasterQuery.totalweight + "," + productIngrMasterQuery.yield + ",'" + productIngrMasterQuery.ph + "'," + productIngrMasterQuery.Prdwaste + "," +
                "'" + productIngrMasterQuery.testby + "','" + productIngrMasterQuery.transferedby + "','" + productIngrMasterQuery.note + "',0,0,0,0,'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now + "',0,0,0,0,0,0,0,0,0,0,0)");

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select itmdisc,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where ProductID = " + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCat(string value)
        {
            var allow_list = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT isnull((select Top(1) CapDubbi from Product where ProductID=tbl_BatchReceiving.pid),0) as dubbi, isnull((select Top(1) isnull(CapQuarter,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as quarter, isnull((select Top(1) isnull(CapGallon,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as Gallon, isnull((select Top(1) isnull(CapDrum,0) from Product where ProductID=tbl_BatchReceiving.pid),0) as Drum, * from tbl_BatchReceiving where color = '" + value + "'").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
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

    }
}