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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PreventDuplicateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request["__RequestVerificationToken"] == null)
                return;

            var currentToken = HttpContext.Current.Request["__RequestVerificationToken"].ToString();

            if (HttpContext.Current.Session["LastProcessedToken"] == null)
            {
                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
                return;
            }

            lock (HttpContext.Current.Session["LastProcessedToken"])
            {
                var lastToken = HttpContext.Current.Session["LastProcessedToken"].ToString();

                if (lastToken == currentToken)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError("", "Looks like you accidentally tried to double post.");
                    return;
                }

                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
            }
        }
    }

    [SessionTimeout]
    public class BatchController : Controller
    {
        private ApplicationDbContext _context;
        private List<PoDetail> getgrosspackage;

        string sqlLogHistoryQuery = "";
        public BatchController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' OR  (Ready = 0)";
            var StartDate = Convert.ToDateTime(Request["batch_s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["batch_e_date"]).ToString("yyyy-MM-dd");
            var date = DateTime.Today.ToString("yyyy-MM-dd");

            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
            {


                strquery = " where date between '" + StartDate + "' and '" + Enddate + "' OR  (Ready = 0) ";
                //if (StartDate != date || Enddate != date)
                //{
                //}

            }


            var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2   from    ProductIngrMaster " + strquery + " order by Ready,Date,OrderId").ToList();
            return View(list);
        }

        public ActionResult Indexstatus()
        {
            //string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            //var StartDate = Convert.ToDateTime(Request["batch_s_date"]).ToString("yyyy-MM-dd");
            //var Enddate = Convert.ToDateTime(Request["batch_e_date"]).ToString("yyyy-MM-dd");

            //if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
            //    strquery = " where  date between '" + StartDate + "' and '" + Enddate + "'";

            //var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2   from    ProductIngrMaster " + strquery + " order by Ready,Date,OrderId").ToList();
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2,tbl_BatchRequest.username,tbl_BatchRequest.userid FROM ProductIngrMaster INNER JOIN tbl_BatchRequest ON ProductIngrMaster.OrderId = tbl_BatchRequest.orderid WHERE (tbl_BatchRequest.status = 'Requested') AND department='Formulation' ORDER BY ProductIngrMaster.Ready, ProductIngrMaster.Date, ProductIngrMaster.OrderId").ToList();
                return View(list);
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  ProductIngrMaster SET Ready = 'False' where  OrderId =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where  department='Formulation' AND userid='" + userid + "' AND  OrderId =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where  department='Formulation' AND  userid='" + userid + "' AND  OrderId =" + ID);
                }


                return RedirectToAction("Indexstatus");
            }
            return RedirectToAction("Login", "Home");
        }
        public ActionResult statusrequest(int ID, string batchno)
        {

            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','" + batchno + "','Formulation')");



            return RedirectToAction("Index");
        }
        public ActionResult GetBatchCount()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Formulation").Count();
            ViewBag.BatchCount = batchCount;
            return PartialView("GetBatchCount");
        }
        [HttpGet]
        public JsonResult GetBatchCountJson()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Formulation").Count();
            return Json(new { batchCount }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult History()
        {
            var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2   from ProductIngrMaster order by Ready,Date,OrderId").ToList();
            return View(list);
        }
        public ActionResult CompHistory()
        {
            var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2   from ProductIngrMaster order by Ready,Date,OrderId").ToList();
            return View(list);
        }
        public ActionResult InCompHistory()
        {
            var list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT *,convert(nvarchar,Ready) as ready2   from ProductIngrMaster order by Ready,Date,OrderId").ToList();
            return View(list);
        }
        public ActionResult InvoiceReport(int ID, string type)
        {
            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT tbl_section.name, tbl_section.id as sectionid, ProductIngrDetail.Id, ProductIngrDetail.ItemId, ProductIngrDetail.ItemName, ProductIngrDetail.weight, ProductIngrDetail.percentage, ProductIngrDetail.Rate, ProductIngrDetail.Cost, ProductIngrDetail.OrderId, ProductIngrDetail.Date, ProductIngrDetail.status FROM ProductIngrDetail INNER JOIN tbl_section ON ProductIngrDetail.status = tbl_section.id where OrderId=" + ID + "  order by name desc ").ToList();
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
            var totalweight = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(weight), 0) AS Stand_total_weight FROM ProductIngrDetail where OrderId=" + ID).FirstOrDefault();
            var op1 = _context.Database.SqlQuery<string>("SELECT machineoper1 FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var op2 = _context.Database.SqlQuery<string>("SELECT machineoper2 FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var testby = _context.Database.SqlQuery<string>("SELECT testby FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var filledby = _context.Database.SqlQuery<string>("SELECT transferedby FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var Regionid = _context.Database.SqlQuery<decimal>("SELECT RegionId FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var Region = _context.Database.SqlQuery<string>("SELECT Name FROM Region where id=" + Regionid + "").FirstOrDefault();

            if (type == "pdf")
            {
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
                rd.SetParameterValue("op1", op1);
                rd.SetParameterValue("op2", op2);
                rd.SetParameterValue("testby", testby);
                rd.SetParameterValue("filledby", filledby);


                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "BatchReport.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("MM/dd/yyy");

                ViewData["OrderId"] = ID;
                ViewData["ColorName"] = ColorName;
                ViewData["Region"] = Region;
                ViewData["ColorNo"] = ColorNo;
                ViewData["Quality"] = Quality;
                ViewData["BatchNo"] = BatchNo;
                ViewData["ProductName"] = ProductName;
                ViewData["ph"] = ph;
                ViewData["note"] = note;
                ViewData["kgcoast"] = kgcoast;
                ViewData["ltrcoast"] = ltrcoast;
                ViewData["viscosity"] = viscosity;
                ViewData["whc"] = whc;
                ViewData["yield"] = yield;
                ViewData["totalweight"] = totalweight;
                ViewData["op1"] = op1;
                ViewData["op2"] = op2;
                ViewData["testby"] = testby;
                ViewData["filledby"] = filledby;
                return View("InvoiceReport", list);

            }
        }
        public ActionResult InvoiceReportQTY(int ID, string type)
        {
            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT tbl_section.name, tbl_section.id as sectionid, ProductIngrDetail.Id, ProductIngrDetail.ItemId, ProductIngrDetail.ItemName, ProductIngrDetail.weight, ProductIngrDetail.percentage, ProductIngrDetail.Rate, ProductIngrDetail.Cost, ProductIngrDetail.OrderId, ProductIngrDetail.Date, ProductIngrDetail.status FROM ProductIngrDetail INNER JOIN tbl_section ON ProductIngrDetail.status = tbl_section.id where OrderId=" + ID + "  order by name desc ").ToList();
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
            var totalweight = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(weight), 0) AS Stand_total_weight FROM ProductIngrDetail where OrderId=" + ID).FirstOrDefault();
            var op1 = _context.Database.SqlQuery<string>("SELECT machineoper1 FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var op2 = _context.Database.SqlQuery<string>("SELECT machineoper2 FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var testby = _context.Database.SqlQuery<string>("SELECT testby FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var filledby = _context.Database.SqlQuery<string>("SELECT transferedby FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var Regionid = _context.Database.SqlQuery<decimal>("SELECT RegionId FROM ProductIngrMaster where OrderId=" + ID + "").FirstOrDefault();
            var Region = _context.Database.SqlQuery<string>("SELECT Name FROM Region where id=" + Regionid + "").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptbatch2.rpt"));

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
                rd.SetParameterValue("op1", op1);
                rd.SetParameterValue("op2", op2);
                rd.SetParameterValue("testby", testby);
                rd.SetParameterValue("filledby", filledby);


                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "BatchReport.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("MM/dd/yyy");
                ViewData["Region"] = Region;
                ViewData["OrderId"] = ID;
                ViewData["type"] = type;
                ViewData["ColorName"] = ColorName;
                ViewData["ColorNo"] = ColorNo;
                ViewData["Quality"] = Quality;
                ViewData["BatchNo"] = BatchNo;
                ViewData["ProductName"] = ProductName;
                ViewData["ph"] = ph;
                ViewData["note"] = note;
                ViewData["kgcoast"] = kgcoast;
                ViewData["ltrcoast"] = ltrcoast;
                ViewData["viscosity"] = viscosity;
                ViewData["whc"] = whc;
                ViewData["yield"] = yield;
                ViewData["totalweight"] = totalweight;
                ViewData["op1"] = op1;
                ViewData["op2"] = op2;
                ViewData["testby"] = testby;
                ViewData["filledby"] = filledby;
                return View("InvoiceReportQTY", list);
            }
        }
        public ActionResult Create(ProductIngrMasterQuery productIngrMasterQuery)
        {
            productIngrMasterQuery.OrderId = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderId),0)+1 from ProductIngrMaster").FirstOrDefault();
            productIngrMasterQuery.Date = DateTime.Today;
            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var product = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and IsPacking=0) ").ToList();
            var region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var ProductIngrMasterQuery = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT Stand_total_weight, CONVERT(VARCHAR, Date, 120) AS ready2,ColorName,OrderId,BatchNo from BatchIngrMaster").ToList();

            var SaleInvVM = new SaleInvVM
            {
                ProductIngrMasterQuerylist = ProductIngrMasterQuery,
                Region_list = region,
                quality = quality,
                product = product,
                color = color,
                productIngrMasterQuery = productIngrMasterQuery,
            };
            return View(SaleInvVM);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [PreventDuplicateRequest]
        public ActionResult Save(string ProductName, string ColorName, int[] id1, string[] item_name1, string[] date1, decimal[] weight1, decimal[] per1, decimal[] rate1, decimal[] cost1, int[] id2, string[] item_name2, string[] date2, decimal[] weight2, decimal[] per2, decimal[] rate2, decimal[] cost2, ProductIngrMasterQuery productIngrMasterQuery)
        {

            decimal id = 0;
            //decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from ProductIngrDetail").FirstOrDefault();

            _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrMasterExtension (UserName, Date, DateTime) VALUES ('" + Session["CurrentUserName"] + "',getdate(),getdate())");

            productIngrMasterQuery.OrderId = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0) from ProductIngrMasterExtension where UserName='" + Session["CurrentUserName"] + "'").FirstOrDefault();
            productIngrMasterQuery.pid = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + ColorName.Trim() + "'").FirstOrDefault();
            productIngrMasterQuery.maincateg = _context.Database.SqlQuery<decimal>("select MainCategoryID from MianCategories where MainCategoryName ='" + productIngrMasterQuery.Quality + "'").FirstOrDefault();
            productIngrMasterQuery.categid = _context.Database.SqlQuery<decimal>("select CategoryID from Categories where CategoryName ='" + ProductName + "'").FirstOrDefault();

            if (item_name1 != null)
            {
                for (int i = 0; i < item_name1.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id1[i] + ",'" + item_name1[i] + "'," + weight1[i] + "," + per1[i] + "," + rate1[i] + "," + cost1[i] + "," + productIngrMasterQuery.OrderId + ",'" + date1[i] + "',0)");
                    sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','New','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId + ",'Item:" + item_name1[i] + " Weight:" + weight1[i] + " Percentage:" + per1[i] + " Rate:" + rate1[i] + " Cost:" + cost1[i] + " Date:" + date1[i] + "')";
                }
            }
            if (item_name2 != null)
            {
                for (int i = 0; i < item_name2.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id2[i] + ",'" + item_name2[i] + "'," + weight2[i] + "," + per2[i] + "," + rate2[i] + "," + cost2[i] + "," + productIngrMasterQuery.OrderId + ",'" + date2[i] + "',1)");
                    sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','New','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId + ",'Item:" + item_name2[i] + " Weight:" + weight2[i] + " Percentage:" + per2[i] + " Rate:" + rate2[i] + " Cost:" + cost2[i] + " Date:" + date2[i] + "')";
                }
            }

            _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrMaster (RegionId,estimated,Total_cost,Stand_total_weight,Stand_total_cost,Stand_total_per,Add_total_weight,Add_total_cost,Add_total_per,Ready,machineoper2,machineoper1,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,totalweight,yield,ph,Prdwaste,testby,transferedby,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,FilledDrum,FilledGallon,FilledQuarter,FilledDubbi,totalweightreceived,SampleQTY,SampleWeight,transfered,Pkgwaste,pid,categid,maincateg) VALUES" +
      " ('" + productIngrMasterQuery.RegionId + "','" + Request["estimated"] + "','" + Request["tot_cost"] + "','" + Request["Stand_total_weight"] + "','" + Request["Stand_total_cost"] + "','" + Request["Stand_total_per"] + "','" + Request["Add_total_weight"] + "','" + Request["Add_total_cost"] + "','" + Request["Add_total_per"] + "','" + productIngrMasterQuery.ready + "','" + productIngrMasterQuery.machineoper2 + "','" + productIngrMasterQuery.machineoper1 + "'," + productIngrMasterQuery.OrderId + ",'" + productIngrMasterQuery.Quality + "','" + Request["ProductName"] + "','" + Request["ColorName"] + "'" +
      ",'" + productIngrMasterQuery.BatchNo + "'," + productIngrMasterQuery.ltrcoast + "," + productIngrMasterQuery.kgcoast + "," + productIngrMasterQuery.viscosity + "," + productIngrMasterQuery.whc + "," +
      "" + productIngrMasterQuery.totalweight + "," + productIngrMasterQuery.yield + ",'" + productIngrMasterQuery.ph + "'," + productIngrMasterQuery.Prdwaste + "," +
      "'" + productIngrMasterQuery.testby + "','" + productIngrMasterQuery.transferedby + "','" + productIngrMasterQuery.note + "',0,0,0,0, '" + productIngrMasterQuery.Date.ToString("yyyy-MM-dd") + "','" + DateTime.Now + "',0,0,0,0,0,0,0,0,0,'" + productIngrMasterQuery.pid + "','" + productIngrMasterQuery.categid + "','" + productIngrMasterQuery.maincateg + "')");

            sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','New','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId +
                ",'ColorName:" + Request["ColorName"] + " Quality:" + productIngrMasterQuery.Quality + " BatchNo:" + productIngrMasterQuery.BatchNo + " Ready:" + productIngrMasterQuery.ready + " ProductName:" + Request["ProductName"] +
                " Per KG Cost:" + productIngrMasterQuery.kgcoast + " Per Ltr Cost:" + productIngrMasterQuery.ltrcoast + " Viscosity:" + productIngrMasterQuery.viscosity + " Whc:" + productIngrMasterQuery.whc + " PH:" + productIngrMasterQuery.ph + " Total Weight:" + productIngrMasterQuery.totalweight +
                " Machine Operator 1:" + productIngrMasterQuery.machineoper1 + " Machine Operator 2:" + productIngrMasterQuery.machineoper2 +
                " Test By:" + productIngrMasterQuery.testby + " Transfered By:" + productIngrMasterQuery.transferedby + " Note:" + productIngrMasterQuery.note + " Date:" + DateTime.Now + "')";

            if (sqlLogHistoryQuery.Length > 10)
                _context.Database.ExecuteSqlCommand(sqlLogHistoryQuery);
            sqlLogHistoryQuery = "";

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var ProductIngrMasterQuery = _context.Database.SqlQuery<ProductIngrMasterQuery>("select top(1) * from ProductIngrMaster where OrderId =" + ID + "").SingleOrDefault();
            var ProductIngrDetailQuery = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT * from ProductIngrDetail where OrderId=" + ID + "").ToList();

            ProductIngrMasterQuery.Stand_total_weight = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(weight), 0) AS Stand_total_weight FROM ProductIngrDetail where OrderId=" + ID + " AND (status = 0)").FirstOrDefault();
            ProductIngrMasterQuery.totalweight = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(weight), 0) AS Stand_total_weight FROM ProductIngrDetail where OrderId=" + ID).FirstOrDefault();
            ProductIngrMasterQuery.Add_total_weight = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(weight), 0) AS Add_total_weight FROM ProductIngrDetail where OrderId=" + ID + " AND (status > 0)").FirstOrDefault();
            ProductIngrMasterQuery.Add_total_cost = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(percentage), 0) AS Add_total_cost FROM ProductIngrDetail where OrderId=" + ID + " AND (status > 0)").FirstOrDefault();
            ProductIngrMasterQuery.Add_total_per = _context.Database.SqlQuery<decimal>("SELECT  ISNULL(SUM(Cost), 0) AS Add_total_per FROM ProductIngrDetail where OrderId=" + ID + " AND (status > 0)").FirstOrDefault();
            ProductIngrMasterQuery.Total_cost = ProductIngrMasterQuery.Add_total_per + ProductIngrMasterQuery.Stand_total_per;

            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
            var product = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1 and IsPacking=0)").ToList();
            var region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = region,
                quality = quality,
                product = product,
                color = color,
                productIngrMasterQuery = ProductIngrMasterQuery,
                ProductIngrDetailQuery = ProductIngrDetailQuery,

            };
            return View(SaleInvVM);
        }

        [HttpPost]
        public ActionResult Update(string ProductName, string ColorName, int[] id1, string[] item_name1, string[] date1, decimal[] weight1, decimal[] per1, decimal[] rate1, decimal[] cost1, int[] id2, string[] item_name2, string[] date2, decimal[] weight2, decimal[] per2, decimal[] rate2, decimal[] cost2, ProductIngrMasterQuery productIngrMasterQuery)
        {

            _context.Database.ExecuteSqlCommand("Delete From ProductIngrDetail where OrderId =" + productIngrMasterQuery.OrderId + "");
            _context.Database.ExecuteSqlCommand("Delete From ProductIngrMaster where OrderId =" + productIngrMasterQuery.OrderId + "");
            decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from ProductIngrDetail").FirstOrDefault();
            if (item_name1 != null)
            {
                for (int i = 0; i < item_name1.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id1[i] + ",'" + item_name1[i] + "'," + weight1[i] + "," + per1[i] + "," + rate1[i] + "," + cost1[i] + "," + productIngrMasterQuery.OrderId + ",'" + date1[i] + "',0)");
                    sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','Edit','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId + ",'Item:" + item_name1[i] + " Weight:" + weight1[i] + " Percentage:" + per1[i] + " Rate:" + rate1[i] + " Cost:" + cost1[i] + " Date:" + date1[i] + "')";
                }
            }
            if (item_name2 != null)
            {
                for (int i = 0; i < item_name2.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrDetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id2[i] + ",'" + item_name2[i] + "'," + weight2[i] + "," + per2[i] + "," + rate2[i] + "," + cost2[i] + "," + productIngrMasterQuery.OrderId + ",'" + date2[i] + "',1)");
                    sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','Edit','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId + ",'Item:" + item_name2[i] + " Weight:" + weight2[i] + " Percentage:" + per2[i] + " Rate:" + rate2[i] + " Cost:" + cost2[i] + " Date:" + date2[i] + "')";
                }
            }

            productIngrMasterQuery.pid = _context.Database.SqlQuery<decimal>("select ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + ColorName.Trim() + "'").FirstOrDefault();
            productIngrMasterQuery.maincateg = _context.Database.SqlQuery<decimal>("select MainCategoryID from MianCategories where MainCategoryName ='" + productIngrMasterQuery.Quality + "'").FirstOrDefault();
            productIngrMasterQuery.categid = _context.Database.SqlQuery<decimal>("select CategoryID from Categories where CategoryName ='" + ProductName + "'").FirstOrDefault();


            _context.Database.ExecuteSqlCommand("INSERT INTO ProductIngrMaster (RegionId,estimated,Total_cost,Stand_total_weight,Stand_total_cost,Stand_total_per,Add_total_weight,Add_total_cost,Add_total_per,Ready,machineoper2,machineoper1,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,totalweight,yield,ph,Prdwaste,testby,transferedby,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,FilledDrum,FilledGallon,FilledQuarter,FilledDubbi,totalweightreceived,SampleQTY,SampleWeight,transfered,Pkgwaste,pid,categid,maincateg) VALUES" +
    " ('" + productIngrMasterQuery.RegionId + "','" + Request["estimated"] + "','" + Request["tot_cost"] + "','" + Request["Stand_total_weight"] + "','" + Request["Stand_total_cost"] + "','" + Request["Stand_total_per"] + "','" + Request["Add_total_weight"] + "','" + Request["Add_total_cost"] + "','" + Request["Add_total_per"] + "','" + productIngrMasterQuery.ready + "','" + productIngrMasterQuery.machineoper2 + "','" + productIngrMasterQuery.machineoper1 + "'," + productIngrMasterQuery.OrderId + ",'" + productIngrMasterQuery.Quality + "','" + Request["ProductName"] + "','" + Request["ColorName"] + "'" +
    ",'" + productIngrMasterQuery.BatchNo + "'," + productIngrMasterQuery.ltrcoast + "," + productIngrMasterQuery.kgcoast + "," + productIngrMasterQuery.viscosity + "," + productIngrMasterQuery.whc + "," +
    "" + productIngrMasterQuery.totalweight + "," + productIngrMasterQuery.yield + ",'" + productIngrMasterQuery.ph + "'," + productIngrMasterQuery.Prdwaste + "," +
    "'" + productIngrMasterQuery.testby + "','" + productIngrMasterQuery.transferedby + "','" + productIngrMasterQuery.note + "',0,0,0,0, '" + productIngrMasterQuery.Date.ToString("yyyy-MM-dd") + "','" + DateTime.Now + "',0,0,0,0,0,0,0,0,0,'" + productIngrMasterQuery.pid + "','" + productIngrMasterQuery.categid + "','" + productIngrMasterQuery.maincateg + "')");

            sqlLogHistoryQuery = sqlLogHistoryQuery + "INSERT INTO tblLogHistory (date,datetime, ActivityForm, ActionTaken, CurrentUser,BillNo ,detail) VALUES (GETDATE(),GETDATE(),'Batch Formulation','Edit','" + Session["CurrentUserName"] + "'," + productIngrMasterQuery.OrderId +
                ",'ColorName:" + Request["ColorName"] + " Quality:" + productIngrMasterQuery.Quality + " BatchNo:" + productIngrMasterQuery.BatchNo + " Ready:" + productIngrMasterQuery.ready + " ProductName:" + Request["ProductName"] +
                " Per KG Cost:" + productIngrMasterQuery.kgcoast + " Per Ltr Cost:" + productIngrMasterQuery.ltrcoast + " Viscosity:" + productIngrMasterQuery.viscosity + " Whc:" + productIngrMasterQuery.whc + " PH:" + productIngrMasterQuery.ph + " Total Weight:" + productIngrMasterQuery.totalweight +
                " Machine Operator 1:" + productIngrMasterQuery.machineoper1 + " Machine Operator 2:" + productIngrMasterQuery.machineoper2 +
                " Test By:" + productIngrMasterQuery.testby + " Transfered By:" + productIngrMasterQuery.transferedby + " Note:" + productIngrMasterQuery.note + " Date:" + DateTime.Now + "')";

            if (sqlLogHistoryQuery.Length > 10)
                _context.Database.ExecuteSqlCommand(sqlLogHistoryQuery);
            sqlLogHistoryQuery = "";


            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select ProductID,UnitPrice from Product where ProductID = " + code + "").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Action1(string code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select itmdisc from Product where ProductName = '" + code + "'").ToList();
            //var detaillist = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT BatchIngrDetail.ItemId, BatchIngrDetail.ItemName, BatchIngrDetail.weight, BatchIngrDetail.percentage, BatchIngrDetail.Rate, BatchIngrDetail.Cost, BatchIngrDetail.OrderId, BatchIngrDetail.Date, BatchIngrDetail.status FROM BatchIngrMaster INNER JOIN BatchIngrDetail ON BatchIngrMaster.OrderId = BatchIngrDetail.OrderId WHERE (BatchIngrMaster.ColorName = '"+code+"') AND (BatchIngrMaster.ProductName = '"+ productname + "') AND (BatchIngrMaster.Quality = '"+qtyname+")").ToList();


            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult addrecord(string code)
        {
            // 1var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate, Cost, OrderId, CONVERT(VARCHAR, Date, 120) AS Date1, status FROM   BatchIngrDetail WHERE OrderId = " + code).ToList();
            //2var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate, ISNULL ((SELECT TOP (1) cp FROM purchasedetail WHERE (pid = BatchIngrDetail.ItemId) ORDER BY invid DESC), Cost) AS Cost, OrderId, CONVERT(VARCHAR, Date, 120) AS Date1, status FROM   BatchIngrDetail WHERE OrderId = " + code).ToList();
            //3var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate1, Rate, Rate * weight AS COST, OrderId, Date1, status FROM (SELECT Id, ItemId, ItemName, weight, percentage, Rate AS Rate1, ISNULL ((SELECT TOP (1) cp FROM purchasedetail WHERE (pid = BatchIngrDetail.ItemId) ORDER BY invid DESC), Rate) AS Rate, OrderId, CONVERT(VARCHAR, Date, 120) AS Date1, status FROM BatchIngrDetail WHERE (OrderId = " + code +")) AS derivedtbl_1 ").ToList();
            // var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate1, Rate, Rate * weight AS COST, OrderId, Date1, status, REM AS ext1 FROM (SELECT Id, ItemId, ItemName, weight, percentage, Rate AS Rate1, ISNULL ((SELECT TOP (1) cp FROM purchasedetail WHERE (pid = BatchIngrDetail.ItemId) ORDER BY invid DESC), Rate) AS Rate, (SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail AS purchasedetail_1 WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE (Status IN ('TSINV', 'WTSINV')) AND (prid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail AS srsdetail_1 WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0) - (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = BatchIngrDetail.ItemId)) AS derivedtbl_1_1) AS REM, OrderId, CONVERT(VARCHAR, Date, 120) AS Date1, status FROM BatchIngrDetail WHERE (OrderId = " + code +")) AS derivedtbl_1 ").ToList();

            //var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate1, Rate, Rate * weight AS COST, OrderId, Date1, status, REM AS ext1 FROM (SELECT Id, ItemId, ItemName, weight, percentage, Rate AS Rate1, ISNULL ((SELECT TOP (1) cp FROM purchasedetail WHERE (pid = BatchIngrDetail.ItemId) ORDER BY invid DESC), Rate) AS Rate, (SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail AS purchasedetail_1 WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE (Status IN ('TSINV', 'WTSINV')) AND (prid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail AS srsdetail_1 WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0) - (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = BatchIngrDetail.ItemId)) AS derivedtbl_1_1) AS REM, OrderId, CONVERT(VARCHAR, GETDATE(), 23) AS Date1, status FROM BatchIngrDetail WHERE (OrderId = " + code +")) AS derivedtbl_1 ").ToList();

            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT Id, ItemId, ItemName, weight, percentage, Rate1, Rate, Rate * weight AS COST, OrderId, Date1, status, REM AS ext1 FROM (SELECT Id, ItemId, ItemName, weight, percentage, Rate AS Rate1, ISNULL ((SELECT TOP (1) purchasedetail.cp FROM purchasedetail INNER JOIN Product ON purchasedetail.pid = Product.ProductID WHERE (purchasedetail.pid = BatchIngrDetail.ItemId) AND (Product.CategoryID <> 38) ORDER BY purchasedetail.invid DESC), ISNULL ((SELECT UnitPrice FROM Product AS Product_2 WHERE (ProductID = BatchIngrDetail.ItemId)), Rate)) AS Rate, (SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail AS purchasedetail_1 WHERE (pid = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE (Status IN ('TSINV', 'WTSINV')) AND (prid = Product_1.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail AS srsdetail_1 WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product_1.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product_1.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product_1.ProductID)), 0) - (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product_1.ProductID)) AS qtyreturn FROM Product AS Product_1 WHERE (ProductID = BatchIngrDetail.ItemId)) AS derivedtbl_1_1) AS REM, OrderId, CONVERT(VARCHAR, GETDATE(), 23) AS Date1, status FROM BatchIngrDetail WHERE (OrderId = " + code + ")) AS derivedtbl_1 ").ToList();


            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCat(string value)
        {
            decimal MainCategoryID = _context.Database.SqlQuery<decimal>("select MainCategoryID from MianCategories where MainCategoryName ='" + value + "'").FirstOrDefault();
            var allow_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where MainCategoryID = " + MainCategoryID + " order by CategoryName asc ").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProd(string value)
        {
            decimal MainCategoryID = _context.Database.SqlQuery<decimal>("select CategoryID from Categories where CategoryName ='" + value + "'").FirstOrDefault();
            var allow_list = _context.Database.SqlQuery<Products>("select ProductName from Product where CategoryID = " + MainCategoryID + " order by ProductName asc").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public JsonResult GetBatch(string q,string p,string c)
        //{
        //    var allow_list = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT CONVERT(VARCHAR, Date, 120) AS ready2,ColorName,OrderId,BatchNo FROM   BatchIngrMaster where Quality='" + q+"' AND ProductName='" +p +"' AND ColorName='" +c +"' ").ToList();
        //    return Json(allow_list, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public JsonResult GetstockValue(int value)
        {
            decimal a, b;
            int icntr;
            var allow_list = _context.Database.SqlQuery<Products>("Select cp from purchasedetail where pid=" + value + " ORDER BY invid desc").ToList();
            var stockDistributionDetailList = _context.Database.SqlQuery<Products>("SELECT price as cp FROM StockDistributionDetail WHERE pid = " + value).ToList();
            var allow_list1 = _context.Database.SqlQuery<Products>("Select SUM(vattax),UnitPrice as cp from Product where ProductID=" + value + " group by UnitPrice").ToList();

            if (allow_list.Count == 0 && stockDistributionDetailList.Count==0)
            {
                return Json(allow_list1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //if (allow_list1.Count > 0)
                //{
                //    allow_list.Clear();
                //    foreach (var itsd in allow_list1)
                //    {
                //        allow_list.Add(itsd);
                //    }

                //    allow_list1 = _context.Database.SqlQuery<Products>("Select SUM(qty),cp from purchasedetail where pid=" + value + " group by cp").ToList();
                //    foreach (var itsd in allow_list1)
                //    {
                //        allow_list.Add(itsd);
                //    }

                //    allow_list1.Clear();
                //    allow_list1 = _context.Database.SqlQuery<Products>("SELECT SUM(Stand_total_weight) AS qty, kgcoast as cp FROM labm WHERE (pid = " + value + ") GROUP BY kgcoast").ToList();
                //    foreach (var itsd in allow_list1)
                //    {
                //        allow_list.Add(itsd);
                //    }

                //}
                if (allow_list1.Count > 0)
                {
                    allow_list.Clear();
                    //foreach (var itsd in allow_list1)
                    //{
                    //    allow_list.Add(itsd);
                    //}
                    allow_list1 = _context.Database.SqlQuery<Products>("Select cp from purchasedetail where pid=" + value + " ORDER BY invid desc").ToList();
                    if (allow_list1.Count == 1)
                    {
                        foreach (var itsd in allow_list1)
                        {
                            allow_list.Add(itsd);
                        }
                    }
                    if (allow_list1.Count > 1)
                    {
                        a = 0;
                        b = 0;
                        icntr = 1;
                        foreach (var itsd in allow_list1)
                        {
                            if (icntr == 1)
                            {
                                allow_list.Add(itsd);
                                a = itsd.cp;
                                icntr++;
                            }
                            if (icntr > 1)
                            {
                                b = itsd.cp;
                                if (a != b)
                                {
                                    allow_list.Add(itsd);
                                    icntr++;
                                    a = itsd.cp;
                                }
                            }

                        }
                    }

                    allow_list1.Clear();
                  
                    allow_list1 = _context.Database.SqlQuery<Products>("SELECT SUM(Stand_total_weight) AS qty, kgcoast as cp FROM labm WHERE (pid = " + value + ") GROUP BY kgcoast").ToList();
                    foreach (var itsd in allow_list1)
                    {
                        allow_list.Add(itsd);
                    }
                    allow_list1 = _context.Database.SqlQuery<Products>("Select SUM(vattax),UnitPrice as cp from Product where ProductID=" + value + " group by UnitPrice").ToList();

                    foreach (var itsd in allow_list1)
                    {
                        allow_list.Add(itsd);
                    }

                    //stockDistributionDetailList = _context.Database.SqlQuery<Products>("SELECT price as cp FROM StockDistributionDetail WHERE pid = " + value).ToList();
                    allow_list.AddRange(stockDistributionDetailList);

                }

                return Json(allow_list, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult GetRemqty(decimal code, decimal pid)
        {
            //var getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  qty,isnull((select sum(Qty) from temp_rows_rate where Item="+ rate + "  and Rate="+code+"),0) as qty2,isnull(qty-(select sum(weight) from ProductIngrDetail where ItemId=" + rate + "  and Rate=" + code + "),0) as qty1 from purchasedetail where pid=" + rate + " and cp=" + code + " ").ToList();
            List<PoDetail> parts = new List<PoDetail>();

            var allow_list = _context.Database.SqlQuery<PoDetail>("Select SUM(qty),cp from purchasedetail where pid=" + pid + " group by cp").ToList();
            foreach (var prdlist in allow_list)
            {

            }
            if (allow_list.Count != 0)
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  isnull(sum(qty),0) as qty,isnull((select vattax from Product where UnitPrice=" + code + " and ProductID=" + pid + "),0) as op,isnull((select sum(weight) from ProductIngrDetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qty1,isnull((select sum(weight) from labdetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qtylab,isnull((select sum(qty) from tbl_BatchFillingDetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qtybatchfilling,isnull((select sum(qty) from srpdetail where pid=" + pid + "  and cp=" + code + "),0) as qtyreturn from purchasedetail where pid=" + pid + " and cp=" + code + "").ToList();
            }
            else
            {
                getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  isnull((select sum(qty) from purchasedetail where cp=" + code + " and pid=" + pid + "),0) as qty,isnull((select vattax from Product where UnitPrice=" + code + " and ProductID=" + pid + "),0) as op,isnull((select sum(weight) from ProductIngrDetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qty1,isnull((select sum(weight) from labdetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qtylab,isnull((select sum(qty) from tbl_BatchFillingDetail where ItemId=" + pid + "  and Rate=" + code + "),0) as qtybatchfilling,isnull((select sum(qty) from srpdetail where pid=" + pid + "  and cp=" + code + "),0) as qtyreturn from Product where ProductID=" + pid + " and UnitPrice=" + code + "").ToList();
            }
            //var getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  isnull(qty - (select sum(weight) from ProductIngrDetail where ItemId=" + rate + "  and Rate=" + code + "),0) as rem from purchasedetail where pid=" + rate + " ").ToList();



            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetRemqtyTotal(decimal code)
        {
            getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail WHERE (pid = Product.ProductID)), 0)  + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM StockDistributionDetail WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE Status in ('TSINV','WTSINV') and (prid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE Status in ('SRINV','TSRINV') and (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0)-(SELECT     ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE      (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = " + code + ")) AS derivedtbl_1").ToList();
            //  getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT qtyreturn AS qty FROM (SELECT vattax + (SELECT ISNULL(SUM(StockDistributionDetail.qty), 0) AS Expr1 FROM StockDistributionDetail INNER JOIN StockDistribution ON StockDistributionDetail.invid = StockDistribution.invid WHERE (StockDistribution.ToStore = 'ProductionDept') AND (StockDistributionDetail.pid = Product.ProductID)) - (SELECT ISNULL(SUM(StockDistributionDetail.qty), 0) AS Expr1 FROM StockDistributionDetail INNER JOIN StockDistribution ON StockDistributionDetail.invid = StockDistribution.invid WHERE (StockDistribution.FromStore = 'ProductionDept') AND (StockDistributionDetail.pid = Product.ProductID))  + (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM tbl_BatchReceiving WHERE (pid = Product.ProductID) AND (status = 'Return')) + (SELECT ISNULL(SUM(totalweight), 0) AS Expr1 FROM ProductIngrMaster WHERE (pid = Product.ProductID) AND (Ready = 'true')) - (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM tbl_BatchReceiving AS tbl_BatchReceiving_1 WHERE (pid = Product.ProductID) AND (status <> 'Return')) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail AS labdetail_1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE (Status IN ('TSINV', 'WTSINV')) AND (prid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail AS srsdetail_1 WHERE (Status IN ('SRINV', 'TSRINV')) AND (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0) - (SELECT ISNULL(SUM(weight), 0) AS Expr1 FROM labdetail WHERE (ItemId = Product.ProductID)) - (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = " + code + ")) AS derivedtbl_1").ToList();

            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult GetRemqtyTotal(decimal code)
        //{
        //    getgrosspackage = _context.Database.SqlQuery<PoDetail>("SELECT qtyreturn AS qty FROM (SELECT vattax + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM purchasedetail WHERE (pid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM ProductIngrDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS weight FROM ProductIngrDetail1 WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(weight), 0) AS weight FROM labdetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM tbl_BatchFillingDetail WHERE (ItemId = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(Order_detail.qty), 0) AS Expr1 FROM Order_detail INNER JOIN Order_Master ON Order_detail.OrderID = Order_Master.OrderID WHERE (Order_detail.prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE Status in ('TSINV','WTSINV') and (prid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srsdetail WHERE Status in ('SRINV','TSRINV') and (prid = Product.ProductID)), 0) - ISNULL ((SELECT ISNULL(SUM(qty), 0) AS qty FROM srpdetail WHERE (pid = Product.ProductID)), 0) + ISNULL ((SELECT ISNULL(SUM(Stand_total_weight), 0) AS qty FROM labm WHERE (pid = Product.ProductID)), 0)-(SELECT     ISNULL(SUM(qty), 0) AS Expr1 FROM sampledetail WHERE      (pid = Product.ProductID)) AS qtyreturn FROM Product WHERE (ProductID = " + code + ")) AS derivedtbl_1").ToList();

        //    return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult Tempadd(decimal code, decimal rate, decimal sp)
        {
            //_context.Database.ExecuteSqlCommand("truncate table temp_rows_rate ");
            _context.Database.ExecuteSqlCommand("INSERT INTO temp_rows_rate (Qty,Rate,Item) VALUES (" + sp + "," + rate + "," + code + ")");
            var getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  qty,isnull((select sum(weight) from ProductIngrDetail where ItemId=" + rate + "  and Rate=" + code + "),0) as qty1 from purchasedetail where pid=" + rate + " and cp=" + code + " ").ToList();
            //var getgrosspackage = _context.Database.SqlQuery<PoDetail>("Select  isnull(qty - (select sum(weight) from ProductIngrDetail where ItemId=" + rate + "  and Rate=" + code + "),0) as rem from purchasedetail where pid=" + rate + " ").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public ActionResult Action(int code)
        //{

        //    var getgrosspackage = _context.Database.SqlQuery<Customer>("Select * From Customers where AccountNo =" + code + "").ToList();
        //    return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        //}
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
        
        public ActionResult DeleteB(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("Delete From ProductIngrDetail where OrderId =" + ID + " ");
            _context.Database.ExecuteSqlCommand("Delete From ProductIngrMaster where OrderId =" + ID + " ");
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