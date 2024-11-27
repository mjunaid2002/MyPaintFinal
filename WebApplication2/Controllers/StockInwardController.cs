using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using CRM.Models;
using WebApplication1.ViewModels;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text.RegularExpressions;
using CrystalDecisions.Shared;

namespace WebApplication2.Controllers
{
    public class StockInwardController : Controller
    {
        private ApplicationDbContext _context;

        public StockInwardController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: StockInward
        public ActionResult Index()
        {
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by invid";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "' order by invid ";

            var list = _context.Database.SqlQuery<PoMaster>("SELECT invid,date,builty,status,req_status  from StockInward" + strquery).ToList();
            return View(list);
        }
        public ActionResult Create(PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from StockInward").FirstOrDefault();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poMaster = poMaster,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name, int[] id, string[] qty, PoMaster poMaster)
        {
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from StockInward").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO StockInwarddetail (Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES (''," + i + "," + id[i] + ",'" + item_name[i] + "',0,0," + qty[i] + ",0," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = "";
            _context.Database.ExecuteSqlCommand("INSERT INTO StockInward (builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status ) VALUES ('" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','Pending','" + DateTime.Now + "',0,0,0,0,0,'Request')");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? ID)
        {
            var poMaster = _context.Database.SqlQuery<PoMaster>("select * from StockInward where invid =" + ID).SingleOrDefault();
            var poDetail = _context.Database.SqlQuery<PoDetail>("select * from StockInwarddetail where invid =" + ID).ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poDetail = poDetail,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
            };
            return View(SaleInvVM);
        }
        public ActionResult InWardToInvoice(int? ID)
        {
            var poMaster = _context.Database.SqlQuery<PoMaster>("select * from StockInward where invid =" + ID).SingleOrDefault();
            var poDetail = _context.Database.SqlQuery<PoDetail>("select * from StockInwarddetail where invid =" + ID).ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck=1)").ToList();
            var Cus_list = _context.Database.SqlQuery<Customers>("SELECT * from customers where discount =1").ToList();
            var SaleInvVM = new SaleInvVM
            {
                poDetail = poDetail,
                poMaster = poMaster,
                pro_listsss = pro_listsss,
                Cus_list = Cus_list,
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Save(string[] item_name, int[] id, decimal[] sp, string[] qty, string[] n_total, PoMaster poMaster)
        {
            _context.Database.ExecuteSqlCommand("update StockInward set status='Finished' where invid=" + poMaster.invid);
            poMaster.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from purchasem where status='PINV'").FirstOrDefault();

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO purchasedetail (Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES ('PINV'," + i + "," + id[i] + ",'" + item_name[i] + "'," + sp[i] + ",0," + qty[i] + "," + n_total[i] + "," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = _context.Database.SqlQuery<string>("select name from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO purchasem (builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid ) VALUES ('" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','PINV',GETDATE(),0,0,0," + poMaster.discount + ",0)");


            decimal accountno = _context.Database.SqlQuery<decimal>("select Top(1) accno from customers where customerid=" + poMaster.supid + "").FirstOrDefault();
            int TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Supplier'," + accountno + ",0,'" + poMaster.total + "'," + poMaster.invid + ",'PINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('0'," + TransId + ",'" + poMaster.date.ToString("yyyy-MM-dd") + "','Stock',10000002,'" + poMaster.total + "',0," + poMaster.invid + ",'PINV')");


            return RedirectToAction("Index");
        }

        public ActionResult PrintInvoice(int? ID)
        {
            return View();
        }
        public ActionResult InvoiceReport(int ID)
        {
            var list = _context.Database.SqlQuery<PoDetail>("select Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll from StockInwarddetail where invid =" + ID + "").ToList();
            var date = _context.Database.SqlQuery<DateTime>("SELECT Date FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();

            var customer = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusemail = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusphone = _context.Database.SqlQuery<string>("SELECT (Select Phone From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();
            var cusaddress = _context.Database.SqlQuery<string>("SELECT (Select Address From Customers where customerid=qtnm.supid  ) as CustomerName  FROM qtnm where InvID =" + ID + " and status='PO'").FirstOrDefault();

            ViewData["date"] = date.ToString("dd-MMM-yyyy");

            ViewData["OrderId"] = ID;
            ViewData["customername"] = customer;
            ViewData["cusemail"] = cusemail;
            ViewData["cusphone"] = cusphone;
            ViewData["cusaddress"] = cusaddress;
            ViewData["cargo"] = _context.Database.SqlQuery<string>("SELECT isnull((Select name From cargo where id=orderm.cargoid),'') as CargoName  FROM orderm where OrderID =" + ID + "").FirstOrDefault();

            ViewData["compname"] = "MyPaint";
            ViewData["email"] = _context.Database.SqlQuery<String>("SELECT email FROM Settings ").FirstOrDefault();
            ViewData["phone"] = _context.Database.SqlQuery<String>("SELECT telephone FROM Settings ").FirstOrDefault();
            ViewData["address"] = _context.Database.SqlQuery<String>("SELECT address FROM Settings ").FirstOrDefault();

            return View("InvoiceReport", list);

        }
        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, string[] qty, PoMaster poMaster)
        {
            _context.Database.ExecuteSqlCommand("Delete From StockInward where InvId =" + poMaster.invid);
            _context.Database.ExecuteSqlCommand("Delete From StockInwarddetail where InvId =" + poMaster.invid);
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO StockInwarddetail (Stauts,sr,pid,pname,cp,cp2,qty,total,invid,box,roll ) VALUES ('PO'," + i + "," + id[i] + ",'" + item_name[i] + "',0,0," + qty[i] + ",0," + poMaster.invid + ",0,0)");
            }
            poMaster.supname = "";
            _context.Database.ExecuteSqlCommand("INSERT INTO StockInward (builty,note,supid,invid,date,total,supname,status,datetime,cargid,cargocharges,othercharges,discount,paid,req_status ) VALUES ('" + poMaster.builty + "','" + poMaster.note + "'," + poMaster.supid + "," + poMaster.invid + ",'" + poMaster.date + "'," + poMaster.total + ",'" + poMaster.supname + "','Pending','" + DateTime.Now + "',0,0,0,0,0,'Request')");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From SaleMasters where InvId =" + ID);
            _context.Database.ExecuteSqlCommand("Delete From StockInwarddetail where InvId =" + ID);
            return RedirectToAction("Index");
        }

        public ActionResult statusrequest(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //_context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + " and Vtype = 'SINVWCTN'");
            _context.Database.ExecuteSqlCommand("UPDATE  StockInward SET  req_status ='Pending'  where invid=" + ID);
            _context.Database.ExecuteSqlCommand("INSERT INTO tbl_BatchRequest(orderid, userid, username, status,batchno,department) " +
                       " VALUES('" + ID + "','" + Session["UserID"].ToString() + "','" + Session["CurrentUserName"].ToString() + "','Requested','StockInWard','Purchase')");
            return RedirectToAction("Index");
        }

       
        public ActionResult Updatestatus(int ID, string status, int userid)
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                if (status == "Approved")
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  StockInward SET req_status = 'Complete' where   invid =" + ID);

                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'Approved' where department='Purchase' AND batchno='StockInWard' AND  userid='" + userid + "' AND  orderid =" + ID);
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE  StockInward SET req_status = 'Request' where   invid =" + ID);
                    _context.Database.ExecuteSqlCommand("UPDATE  tbl_BatchRequest SET status = 'DisApproved' where department='Purchase' AND batchno='StockInWard' AND userid='" + userid + "' AND  orderid =" + ID);
                }


                return RedirectToAction("Indexstatus");
            }
            return RedirectToAction("Login", "Home");
        }



        public ActionResult GetSaleCount()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Purchase").Count();
            ViewBag.BatchCount = batchCount;
            return PartialView("GetSaleCount");
        }
        [HttpGet]
        public JsonResult GetSaleCountJson()
        {
            int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Purchase").Count();
            return Json(new { batchCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Indexstatus()
        {
            if (Session["CurrentUserName"].ToString() == "Super Admin")
            {
                var list = _context.Database.SqlQuery<tbl_BatchRequest>("SELECT *  from tbl_BatchRequest where (tbl_BatchRequest.status = 'Requested') AND department='Purchase'").ToList();
                return View(list);

            }
            return RedirectToAction("Login", "Home");
        }

    }
}