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
            var list = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT * from tbl_BatchReceiving").ToList();
            return View(list);
        }
        public ActionResult Create(BatchReceivingQuery batchReceivingQuery)
        {
            batchReceivingQuery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchReceiving  ").FirstOrDefault();
            var batchTransferQuerylist = _context.Database.SqlQuery<BatchTransferQuery>("SELECT color from tbl_BatchTransfer where status='Transfer' group by color ").ToList();
            var SaleInvVM = new SaleInvVM
            {
                batchTransferQuerylist = batchTransferQuerylist,
                batchReceivingQuery = batchReceivingQuery,
            };
            return View(SaleInvVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(BatchReceivingQuery batchReceivingQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where ProductName ='" + batchReceivingQuery.color + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into tbl_BatchReceiving (Id,date,daterec,batch,color,pid,weight,status) values(" + batchReceivingQuery.Id + ",'"+ batchReceivingQuery.date + "','"+ batchReceivingQuery.daterec + "','" + Request["batch"] + "','" + batchReceivingQuery.color + "',"+ ProductID + ","+ batchReceivingQuery.weight + ",'"+Request["opt"] +"')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var batchReceivingQuery = _context.Database.SqlQuery<BatchReceivingQuery>("SELECT * from tbl_BatchReceiving where Id=" + ID + "").SingleOrDefault();
            var batchTransferQuerylist = _context.Database.SqlQuery<BatchTransferQuery>("SELECT color from tbl_BatchTransfer where status='Transfer' group by color").ToList();
            var SaleInvVM = new SaleInvVM
            {
                batchTransferQuerylist = batchTransferQuerylist,
                batchReceivingQuery = batchReceivingQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Edit(BatchReceivingQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where ProductName ='" + batchTransferQuery.color + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("Update tbl_BatchReceiving set date  = N'" + batchTransferQuery.date + "',daterec  = N'" + batchTransferQuery.daterec + "',batch  = N'" + Request["batch"] + "',color  = N'" + batchTransferQuery.color + "',pid=" + ProductID + ",weight=" + batchTransferQuery.weight + " ,status='" + Request["opt"] + "' where id = " + batchTransferQuery.Id + "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetProd(string value)
        {
            var allow_list = _context.Database.SqlQuery<Products>("select BatchNo from tbl_BatchTransfer where status='Transfer' and ProductName='" + value + "'").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
    }
}