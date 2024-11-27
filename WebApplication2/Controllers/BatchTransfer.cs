using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class BatchTransferController : Controller
    {
        private ApplicationDbContext _context;

        public BatchTransferController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "'  ";

            var list = _context.Database.SqlQuery<BatchTransferQuery>("SELECT * from tbl_BatchTransfer" + strquery).ToList();
            return View(list);
        }
        public ActionResult Create(BatchTransferQuery batchTransferQuery)
        {
            batchTransferQuery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchTransfer  ").FirstOrDefault();
            var color = _context.Database.SqlQuery<Products>("select colorname as ProductName  from ProductIngrMaster where Ready=1 ").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                color = color,
                batchTransferQuery = batchTransferQuery,
            };
            return View(SaleInvVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(BatchTransferQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchTransferQuery.color.Trim() + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into tbl_BatchTransfer (RegionId,Id,date,daterec,batch,color,pid,weight,status) values(" + batchTransferQuery.RegionId + "," + batchTransferQuery.Id + ",'"+ batchTransferQuery.date + "','"+ batchTransferQuery.daterec + "','" + Request["batch"] + "','" + batchTransferQuery.color + "',"+ ProductID + ","+ batchTransferQuery.weight + ",'"+Request["opt"] +"')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var batchTransferQuery = _context.Database.SqlQuery<BatchTransferQuery>("SELECT * from tbl_BatchTransfer where Id=" + ID + "").SingleOrDefault();
            batchTransferQuery.totalweight = _context.Database.SqlQuery<decimal>("SELECT totalweight from ProductIngrMaster where batchno='" + batchTransferQuery.batch + "'").SingleOrDefault();
           
            var color = _context.Database.SqlQuery<Products>("select colorname as ProductName,totalweight  from ProductIngrMaster  where Ready=1  ").ToList();
           
           var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                color = color,
                batchTransferQuery = batchTransferQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Edit(BatchTransferQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchTransferQuery.color.Trim() + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("Update tbl_BatchTransfer set RegionId  = '" + batchTransferQuery.RegionId + "',date  = N'" + batchTransferQuery.date + "',daterec  = N'" + batchTransferQuery.daterec + "',batch  = N'" + Request["batch"] + "',color  = N'" + batchTransferQuery.color + "',pid=" + ProductID + ",weight=" + batchTransferQuery.weight + " ,status='" + Request["opt"] + "' where id = " + batchTransferQuery.Id + "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetProd(string value)
        {
            var allow_list = _context.Database.SqlQuery<Products>("select BatchNo,totalweight from ProductIngrMaster where Ready=1 and colorname='" + value + "' ORDER BY OrderId DESC ").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
    }
}