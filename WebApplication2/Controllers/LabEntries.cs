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
    public class LabEntriesController : Controller
    {
        private ApplicationDbContext _context;
        public LabEntriesController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "'  ";

            var list = _context.Database.SqlQuery<LabMasterQuery>("SELECT *  from labm " + strquery).ToList();
            return View(list);
        }
       
        public ActionResult InvoiceReport(int ID, string type)
        {
            var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT 'Basic Formula' as name,* from labdetail where OrderId=" + ID + "").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var ColorName = _context.Database.SqlQuery<string>("SELECT ColorName FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var ColorNo = _context.Database.SqlQuery<string>("SELECT isnull(ColorNo,0) as ColorNo  FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var Quality = _context.Database.SqlQuery<string>("SELECT Quality FROM labm where OrderId=" + ID + "").FirstOrDefault();
           // var Regionid = _context.Database.SqlQuery<decimal>("SELECT RegionId FROM labm where OrderId=" + ID + "").FirstOrDefault();
           // var Region = _context.Database.SqlQuery<string>("SELECT Name FROM Region where id=" + Regionid + "").FirstOrDefault();
            var BatchNo = _context.Database.SqlQuery<string>("SELECT BatchNo FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var ProductName = _context.Database.SqlQuery<string>("SELECT ProductName FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var ph = _context.Database.SqlQuery<string>("SELECT ph FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var note = _context.Database.SqlQuery<string>("SELECT note FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var kgcoast = _context.Database.SqlQuery<decimal>("SELECT kgcoast FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var ltrcoast = _context.Database.SqlQuery<decimal>("SELECT ltrcoast FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var viscosity = _context.Database.SqlQuery<decimal>("SELECT viscosity FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var whc = _context.Database.SqlQuery<decimal>("SELECT whc FROM labm where OrderId=" + ID + "").FirstOrDefault();
            //var yield = _context.Database.SqlQuery<decimal>("SELECT yield FROM labm where OrderId=" + ID + "").FirstOrDefault();
            var totalweight = _context.Database.SqlQuery<decimal>("SELECT Stand_total_weight FROM labm where OrderId=" + ID + "").FirstOrDefault();
            //var op1 = _context.Database.SqlQuery<string>("SELECT machineoper1 FROM labm where OrderId=" + ID + "").FirstOrDefault();
            //var op2 = _context.Database.SqlQuery<string>("SELECT machineoper2 FROM labm where OrderId=" + ID + "").FirstOrDefault();
            //var testby = _context.Database.SqlQuery<string>("SELECT testby FROM labm where OrderId=" + ID + "").FirstOrDefault();
            //var filledby = _context.Database.SqlQuery<string>("SELECT transferedby FROM labm where OrderId=" + ID + "").FirstOrDefault();
            if (type == "pdf")
            {
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptlab.rpt"));

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
                //rd.SetParameterValue("yield", yield);
                rd.SetParameterValue("totalweight", totalweight);
                rd.SetParameterValue("op1", 0);
                rd.SetParameterValue("op2", 0);
                rd.SetParameterValue("testby", 0);
                rd.SetParameterValue("filledby", 0);


                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "LabReport.pdf");
            }
            else
            {
                ViewData["date"] = date.ToString("MM/dd/yyy");

                ViewData["OrderId"] = ID;
                ViewData["type"] = type;
                ViewData["ColorName"] = ColorName;
               // ViewData["Region"] = Region;
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

                ViewData["totalweight"] = totalweight;
                ViewData["op1"] = "";
                ViewData["op2"] = "";
                ViewData["testby"] = "";
                ViewData["filledby"] = "";
                return View("InvoiceReport", list);
            }
        }
        //public ActionResult InvoiceReport(int ID)
        //{
        //    var list = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT 'Basic Formula' as name,* from labdetail where OrderId=" + ID + "").ToList();
        //    var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var ColorName = _context.Database.SqlQuery<string>("SELECT ColorName FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var ColorNo = _context.Database.SqlQuery<string>("SELECT isnull(ColorNo,0) as ColorNo  FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var Quality = _context.Database.SqlQuery<string>("SELECT Quality FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var BatchNo = _context.Database.SqlQuery<string>("SELECT BatchNo FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var ProductName = _context.Database.SqlQuery<string>("SELECT ProductName FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var ph = _context.Database.SqlQuery<string>("SELECT ph FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var note = _context.Database.SqlQuery<string>("SELECT note FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var kgcoast = _context.Database.SqlQuery<decimal>("SELECT kgcoast FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var ltrcoast = _context.Database.SqlQuery<decimal>("SELECT ltrcoast FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var viscosity = _context.Database.SqlQuery<decimal>("SELECT viscosity FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var whc = _context.Database.SqlQuery<decimal>("SELECT whc FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    //var yield = _context.Database.SqlQuery<decimal>("SELECT yield FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    var totalweight = _context.Database.SqlQuery<decimal>("SELECT Stand_total_weight FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    //var op1 = _context.Database.SqlQuery<string>("SELECT machineoper1 FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    //var op2 = _context.Database.SqlQuery<string>("SELECT machineoper2 FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    //var testby = _context.Database.SqlQuery<string>("SELECT testby FROM labm where OrderId=" + ID + "").FirstOrDefault();
        //    //var filledby = _context.Database.SqlQuery<string>("SELECT transferedby FROM labm where OrderId=" + ID + "").FirstOrDefault();

        //    ReportDocument rd = new ReportDocument();

        //    rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptlab.rpt"));

        //    rd.SetDataSource(list);

        //    rd.SetParameterValue("OrderId", ID);
        //    rd.SetParameterValue("date", date);
        //    rd.SetParameterValue("ColorName", ColorName);
        //    rd.SetParameterValue("ColorNo", ColorNo);
        //    rd.SetParameterValue("Quality", Quality);
        //    rd.SetParameterValue("BatchNo", BatchNo);
        //    rd.SetParameterValue("ProductName", ProductName);
        //    rd.SetParameterValue("ph", ph);
        //    rd.SetParameterValue("note", note);
        //    rd.SetParameterValue("kgcoast", kgcoast);
        //    rd.SetParameterValue("ltrcoast", ltrcoast);
        //    rd.SetParameterValue("viscosity", viscosity);
        //    rd.SetParameterValue("whc", whc);
        //    //rd.SetParameterValue("yield", yield);
        //    rd.SetParameterValue("totalweight", totalweight);
        //    rd.SetParameterValue("op1", 0);
        //    rd.SetParameterValue("op2", 0);
        //    rd.SetParameterValue("testby", 0);
        //    rd.SetParameterValue("filledby", 0);


        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();

        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, "application/pdf", "LabReport.pdf");
        //}

        public ActionResult Create(LabMasterQuery labMasterQuery)
        {
            labMasterQuery.OrderId = _context.Database.SqlQuery<decimal>("select ISNULL(Max(OrderId),0)+1 from labm").FirstOrDefault();
            labMasterQuery.Date = DateTime.Today;
            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
          //  var region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();
            var product = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories  where IsPacking=0) ").ToList();
            var ProductIngrMasterQuery = _context.Database.SqlQuery<ProductIngrMasterQuery>("SELECT Stand_total_weight, CONVERT(VARCHAR, Date, 120) AS ready2,ColorName,OrderId,BatchNo from BatchIngrMaster").ToList();

            var SaleInvVM = new SaleInvVM
            {
                // Region_list = region,
                ProductIngrMasterQuerylist = ProductIngrMasterQuery,
                quality = quality,
                product = product,
                color = color,
                labMasterQuery = labMasterQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(int[] id1, string[] item_name1, string[] date1, decimal[] weight1, decimal[] per1, decimal[] rate1, decimal[] cost1, LabMasterQuery labMasterQuery)
        {
            decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from labdetail").FirstOrDefault();

            labMasterQuery.pid = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + Request["ColorName"].Trim() + "'").FirstOrDefault();
            labMasterQuery.maincateg = _context.Database.SqlQuery<decimal>("select top(1) MainCategoryID from MianCategories where MainCategoryName ='" + labMasterQuery.Quality + "'").FirstOrDefault();
            labMasterQuery.categid = _context.Database.SqlQuery<decimal>("select top(1) CategoryID from Categories where CategoryName ='" + Request["ProductName"] + "'").FirstOrDefault();

            for (int i = 0; i < item_name1.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO labdetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id1[i] + ",'" + item_name1[i] + "'," + weight1[i] + "," + per1[i] + "," + rate1[i] + "," + cost1[i] + "," + labMasterQuery.OrderId + ",'" + date1[i] + "',0)");
            }

            if (Convert.ToDecimal(Request["Stand_total_weight"]) > 0)
                labMasterQuery.kgcoast = Convert.ToDecimal(Request["Stand_total_per"]) / Convert.ToDecimal(Request["Stand_total_weight"]);

            _context.Database.ExecuteSqlCommand("INSERT INTO labm (req_status ,ready,Stand_total_weight,Stand_total_cost,Stand_total_per,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,ph,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,pid,categid,maincateg) VALUES" +
                " ('Request','" + labMasterQuery.ready + "','" + Request["Stand_total_weight"] + "','" + Request["Stand_total_cost"] + "','" + Request["Stand_total_per"] + "'," + labMasterQuery.OrderId + ",'" + labMasterQuery.Quality + "','" + Request["ProductName"] + "','" + Request["ColorName"] + "'" +
                ",'" + labMasterQuery.BatchNo + "'," + labMasterQuery.ltrcoast + "," + labMasterQuery.kgcoast + "," + labMasterQuery.viscosity + "," + labMasterQuery.whc + "," +
                "'" + labMasterQuery.ph + "','" + labMasterQuery.note + "'," + labMasterQuery.Drum + "," + labMasterQuery.Gallon + "," + labMasterQuery.Quarter + "," + labMasterQuery.Dubbi + ",'" + labMasterQuery.Date.ToString("yyyy-MM-dd") + "','" + DateTime.Now + "'," + labMasterQuery.pid + "," + labMasterQuery.categid + "," + labMasterQuery.maincateg + ")");

            _context.Database.ExecuteSqlCommand("Update Product set UnitPrice=" + labMasterQuery.kgcoast + " where ProductID = " + labMasterQuery.pid + "");

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var labMasterQuery = _context.Database.SqlQuery<LabMasterQuery>("select * from labm where OrderId =" + ID + "").SingleOrDefault();
            var ProductIngrDetailQuery = _context.Database.SqlQuery<ProductIngrDetailQuery>("SELECT * from labdetail where OrderId=" + ID + "").ToList();

            var quality = _context.Database.SqlQuery<MianCategories>("SELECT * from MianCategories").ToList();
           // var region = _context.Database.SqlQuery<Region>("SELECT * from region").ToList();
            var product = _context.Database.SqlQuery<Categories>("SELECT * from Categories").ToList();
            var color = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where IsPacking=0)").ToList();
            var SaleInvVM = new SaleInvVM
            {
               // Region_list = region,
                quality = quality,
                product = product,
                color = color,
                labMasterQuery = labMasterQuery,
                ProductIngrDetailQuery = ProductIngrDetailQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Update(int[] id1, string[] item_name1, string[] date1, decimal[] weight1, decimal[] per1, decimal[] rate1, decimal[] cost1, LabMasterQuery labMasterQuery)
        {
            _context.Database.ExecuteSqlCommand("Delete From labdetail where OrderId =" + labMasterQuery.OrderId + "");
            _context.Database.ExecuteSqlCommand("Delete From labm where OrderId =" + labMasterQuery.OrderId + "");

            labMasterQuery.pid = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + Request["ColorName"].Trim() + "'").FirstOrDefault();
            labMasterQuery.maincateg = _context.Database.SqlQuery<decimal>("select top(1) MainCategoryID from MianCategories where MainCategoryName ='" + labMasterQuery.Quality + "'").FirstOrDefault();
            labMasterQuery.categid = _context.Database.SqlQuery<decimal>("select top(1) CategoryID from Categories where CategoryName ='" + Request["ProductName"] + "'").FirstOrDefault();

            decimal id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from labdetail").FirstOrDefault();
            for (int i = 0; i < item_name1.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO labdetail (Id,ItemId,ItemName,weight,percentage,Rate,Cost,OrderId,Date,status) VALUES (" + id + "," + id1[i] + ",'" + item_name1[i] + "'," + weight1[i] + "," + per1[i] + "," + rate1[i] + "," + cost1[i] + "," + labMasterQuery.OrderId + ",'" + date1[i] + "',0)");
            }

            if (Convert.ToDecimal(Request["Stand_total_weight"]) > 0)
                labMasterQuery.kgcoast = Convert.ToDecimal(Request["Stand_total_per"]) / Convert.ToDecimal(Request["Stand_total_weight"]);
            _context.Database.ExecuteSqlCommand("INSERT INTO labm (req_status,ready,Stand_total_weight,Stand_total_cost,Stand_total_per,OrderId,Quality,ProductName,ColorName,BatchNo,ltrcoast,kgcoast,viscosity,whc,ph,note,Drum,Gallon,Quarter,Dubbi,Date,datetime,pid,categid,maincateg) VALUES" +
                " ('Request','" + labMasterQuery.ready + "','" + Request["Stand_total_weight"] + "','" + Request["Stand_total_cost"] + "','" + Request["Stand_total_per"] + "'," + labMasterQuery.OrderId + ",'" + labMasterQuery.Quality + "','" + Request["ProductName"] + "','" + Request["ColorName"] + "'" +
                ",'" + labMasterQuery.BatchNo + "'," + labMasterQuery.ltrcoast + "," + labMasterQuery.kgcoast + "," + labMasterQuery.viscosity + "," + labMasterQuery.whc + "," +
                "'" + labMasterQuery.ph + "','" + labMasterQuery.note + "'," + labMasterQuery.Drum + "," + labMasterQuery.Gallon + "," + labMasterQuery.Quarter + "," + labMasterQuery.Dubbi + ",'" + labMasterQuery.Date.ToString("yyyy-MM-dd") + "','" + DateTime.Now + "'," + labMasterQuery.pid + "," + labMasterQuery.categid + "," + labMasterQuery.maincateg + ")");

            _context.Database.ExecuteSqlCommand("Update Product set UnitPrice=" + labMasterQuery.kgcoast + " where ProductID = " + labMasterQuery.pid + "");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Action(int code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select itmdisc,ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where ProductID = " + code+"").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Action1(string code)
        {
            var getgrosspackage = _context.Database.SqlQuery<Products>("select itmdisc from Product where ProductName = '" + code + "'").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCat(string value)
        {
            decimal MainCategoryID = _context.Database.SqlQuery<decimal>("select MainCategoryID from MianCategories where MainCategoryName ='"+ value + "'").FirstOrDefault();
            var allow_list = _context.Database.SqlQuery<Categories>("SELECT * from Categories where MainCategoryID = "+ MainCategoryID + " order by CategoryName asc ").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
       [HttpGet]
        public JsonResult GetProd(string value)
        {
            decimal MainCategoryID = _context.Database.SqlQuery<decimal>("select CategoryID from Categories where CategoryName ='" + value + "'").FirstOrDefault();
            var allow_list = _context.Database.SqlQuery<Products>("select ProductName from Product where CategoryID = " + MainCategoryID + " order by ProductName asc").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
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




        public ActionResult GetLabCount()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Lab").Count();
            ViewBag.BatchCount = batchCount;
            return PartialView("GetLabCount");
        }
        [HttpGet]
        public JsonResult GetLabCountJson()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Lab").Count();
            return Json(new { batchCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Indexstatus()
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                var list = _context.Database.SqlQuery<tbl_BatchRequest>("SELECT *  from tbl_BatchRequest where (tbl_BatchRequest.status = 'Requested') AND department='Lab'").ToList();
                return View(list);

            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult statusrequest(int? ID)
        {

            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("UPDATE  labm SET  req_status ='Pending'  where orderid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','LabEntries','Lab')");
            return RedirectToAction("Index");



        }
        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  labm SET req_status = 'Complete' where orderid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Lab' AND batchno='LabEntries' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  labm SET req_status = 'Request' where   orderid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Lab' AND batchno='LabEntries' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus");
            }
            return RedirectToAction("Login", "Home");
        }
    }
}