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
    public class BatchReturnController : Controller
    {
        private ApplicationDbContext _context;

        public BatchReturnController()
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
            string strquery = " where daterec ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where daterec between '" + StartDate + "' and '" + Enddate + "'  ";


            var list = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT * from tbl_BatchReceiving " + strquery).ToList();
            return View(list);
        }
        public ActionResult Create(BatchReceivingQuery batchReceivingQuery)
        {
            batchReceivingQuery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchReceiving  ").FirstOrDefault();
            var batchTransferQuerylist = _context.Database.SqlQuery<BatchTransferQuery>("SELECT color from tbl_BatchTransfer where status='Transfer' group by color ").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                batchTransferQuerylist = batchTransferQuerylist,
                batchReceivingQuery = batchReceivingQuery,
            };
            return View(SaleInvVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(BatchReceivingQuery batchReceivingQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchReceivingQuery.color.Trim() + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into tbl_BatchReceiving (RegionId,Id,date,daterec,batch,color,pid,weight,status) values(" + batchReceivingQuery.RegionId + "," + batchReceivingQuery.Id + ",'"+ batchReceivingQuery.date + "','"+ batchReceivingQuery.daterec + "','" + Request["batch"] + "','" + batchReceivingQuery.color + "',"+ ProductID + ","+ batchReceivingQuery.weight + ",'"+Request["opt"] +"')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var batchReceivingQuery = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT * from tbl_BatchReceiving where Id=" + ID + "").SingleOrDefault();
            batchReceivingQuery.totalweight = _context.Database.SqlQuery<decimal>("SELECT weight from tbl_BatchTransfer where batch='" + batchReceivingQuery.batch + "'").SingleOrDefault();

            var batchTransferQuerylist = _context.Database.SqlQuery<BatchTransferQuery>("SELECT color from tbl_BatchTransfer where status='Transfer' group by color").ToList();
            var Region = _context.Database.SqlQuery<Region>("SELECT * from Region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = Region,
                batchTransferQuerylist = batchTransferQuerylist,
                batchReceivingQuery = batchReceivingQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Edit(BatchReceivingQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where LTRIM(RTRIM(ProductName)) ='" + batchTransferQuery.color.Trim() + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("Update tbl_BatchReceiving set RegionId  = '" + batchTransferQuery.RegionId + "',date  = N'" + batchTransferQuery.date + "',daterec  = N'" + batchTransferQuery.daterec + "',batch  = N'" + Request["batch"] + "',color  = N'" + batchTransferQuery.color + "',pid=" + ProductID + ",weight=" + batchTransferQuery.weight + " ,status='" + Request["opt"] + "' where id = " + batchTransferQuery.Id + "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetProd(string value)
        {
            var allow_list = _context.Database.SqlQuery<Products>("select batch as BatchNo,weight as totalweight from tbl_BatchTransfer where status='Transfer' and color='" + value + "'").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
    }
}