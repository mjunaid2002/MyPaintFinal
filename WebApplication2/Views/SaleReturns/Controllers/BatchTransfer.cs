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
            var list = _context.Database.SqlQuery<BatchTransferQuery>("SELECT * from tbl_BatchTransfer").ToList();
            return View(list);
        }
        public ActionResult Create(BatchTransferQuery batchTransferQuery)
        {
            batchTransferQuery.Id = _context.Database.SqlQuery<decimal>("select ISNULL(Max(Id),0)+1 from tbl_BatchTransfer  ").FirstOrDefault();
            var color = _context.Database.SqlQuery<Products>("select colorname as ProductName  from ProductIngrMaster ").ToList();
            var SaleInvVM = new SaleInvVM
            {
                color = color,
                batchTransferQuery = batchTransferQuery,
            };
            return View(SaleInvVM);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult Save(BatchTransferQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where ProductName ='" + batchTransferQuery.color + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("insert into tbl_BatchTransfer (Id,date,daterec,batch,color,pid,weight,status) values(" + batchTransferQuery.Id + ",'"+ batchTransferQuery.date + "','"+ batchTransferQuery.daterec + "','" + Request["batch"] + "','" + batchTransferQuery.color + "',"+ ProductID + ","+ batchTransferQuery.weight + ",'"+Request["opt"] +"')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var batchTransferQuery = _context.Database.SqlQuery<BatchTransferQuery>("SELECT * from tbl_BatchTransfer where Id=" + ID + "").SingleOrDefault();
            var color = _context.Database.SqlQuery<Products>("select colorname as ProductName  from ProductIngrMaster ").ToList();
            var SaleInvVM = new SaleInvVM
            {
                color = color,
                batchTransferQuery = batchTransferQuery,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Edit(BatchTransferQuery batchTransferQuery)
        {
            decimal ProductID = _context.Database.SqlQuery<decimal>("select top(1) ProductID from Product where ProductName ='" + batchTransferQuery.color + "'").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("Update tbl_BatchTransfer set date  = N'" + batchTransferQuery.date + "',daterec  = N'" + batchTransferQuery.daterec + "',batch  = N'" + Request["batch"] + "',color  = N'" + batchTransferQuery.color + "',pid=" + ProductID + ",weight=" + batchTransferQuery.weight + " ,status='" + Request["opt"] + "' where id = " + batchTransferQuery.Id + "");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetProd(string value)
        {
            var allow_list = _context.Database.SqlQuery<Products>("select BatchNo from ProductIngrMaster where Ready=1 and ProductName='" + value + "'").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
    }
}