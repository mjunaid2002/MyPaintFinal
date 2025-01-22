using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication1.ViewModels;

namespace WebApplication2.Controllers
{
    public class BranchTransferController : Controller
    {
        private ApplicationDbContext _context;
        public BranchTransferController()
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
            string strquery = " where date ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ";
            var StartDate = Convert.ToDateTime(Request["s_date"]).ToString("yyyy-MM-dd");
            var Enddate = Convert.ToDateTime(Request["e_date"]).ToString("yyyy-MM-dd");
            if (StartDate != null && Enddate != null && StartDate != "0001-01-01" && Enddate != "0001-01-01")
                strquery = " where date between '" + StartDate + "' and '" + Enddate + "' ";

            var branchid = Request["branchid"];
            var branch = Session["Branch"];
            if (branchid == null && branch != "All")
            {
                branchid = Session["BranchId"].ToString();
            }
            if (!string.IsNullOrEmpty(branchid))
            {
                strquery += " and Tobranch = " + branchid;
            }
            strquery += " order by invid";
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from Branch").ToList();
            ViewBag.BranchList = Branch;
            var list = _context.Database.SqlQuery<BranchTransferM>("SELECT Br.invid, Br.date, Br.status,Br.Frombranch, Br.Tobranch, Br.note, branch.name AS Toname, Branch_1.name AS Fromname FROM BranchTransferM AS Br INNER JOIN Branch AS Branch_1 ON Br.Frombranch = Branch_1.id INNER JOIN Branch AS branch ON Br.Tobranch = branch.id" + strquery).ToList();
            return View(list);
        }
        public ActionResult Create(BranchTransferM BranchTransfer)
        {
            BranchTransfer.date = DateTime.Now;
            BranchTransfer.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from BranchTransferM").FirstOrDefault();
            var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))").ToList();
            //var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck<>1)").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("Select id,name from Branch").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Branch_list = Branch,

                BranchTransfer = BranchTransfer,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Save(string[] item_name, int[] id, decimal[] qty, string[] size, BranchTransferM BranchTransfer)
        {
            BranchTransfer.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from BranchTransferM").FirstOrDefault();

            if (item_name != null)
            {
                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT  INTO  BranchTransferDetail(pid, pname, qty,rqty, invid, size)  VALUES (" + id[i] + ",'" + item_name[i] + "','" + qty[i] + "','" + qty[i] + "','" + BranchTransfer.invid + "','" + size[i] + "' )");
                }

                _context.Database.ExecuteSqlCommand("INSERT INTO  BranchTransferM(invid, date, Frombranch, Tobranch, note, status) VALUES (" + BranchTransfer.invid + ",'" + BranchTransfer.date + "','" + BranchTransfer.Frombranch + "','" + BranchTransfer.Tobranch + "','" + BranchTransfer.note + "','" + BranchTransfer.status + "')  ");

            }
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int? ID)
        {
            //var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID <> 1)").ToList();
            var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))").ToList();
            //var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck<>1)").ToList();
            var list = _context.Database.SqlQuery<BranchTransferM>("SELECT * from BranchTransferM where invid=" + ID).SingleOrDefault();
            var detaillist = _context.Database.SqlQuery<BranchTransferDetail>("SELECT * from BranchTransferDetail where invid=" + ID).ToList();
            var Branch = _context.Database.SqlQuery<Branch>("Select id,name from Branch").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Branch_list = Branch,
                BranchTransfer = list,
                BranchTransferDetailList = detaillist,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);

        }
        public ActionResult PrintInvoice(int? ID)
        {

            var CompanyName = _context.Database.SqlQuery<string>("SELECT company FROM   tbl_setting ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<string>("SELECT email FROM   tbl_setting ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<string>("SELECT telephone FROM   tbl_setting ").FirstOrDefault();
            byte[] Image = _context.Database.SqlQuery<byte[]>("SELECT logo FROM tbl_setting").FirstOrDefault();
            var Address = _context.Database.SqlQuery<string>("SELECT address FROM   tbl_setting ").FirstOrDefault();
            var comntn = _context.Database.SqlQuery<string>("SELECT ntn FROM   tbl_setting ").FirstOrDefault();
            var STRN = _context.Database.SqlQuery<string>("SELECT strn FROM   tbl_setting ").FirstOrDefault();

            ViewData["compname"] = CompanyName;
            ViewData["email"] = Email;
            ViewData["phone"] = Phone;
            ViewData["address"] = Address;
            ViewData["cusntn"] = comntn;
            if (Image != null)
            {
                ViewData["logo"] = Convert.ToBase64String(Image);
            }
            else
            {
                ViewData["logo"] = "";
            }

            var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck<>1)").ToList();
            // var list = _context.Database.SqlQuery<BranchTransferM>("SELECT  BranchTransferM.invid, BranchTransferM.date, BranchTransferM.Fromregion, BranchTransferM.ToRegion, BranchTransferM.note, Branch.name AS Fromname, Region_1.name AS Toname FROM BranchTransferM INNER JOIN Branch ON BranchTransferM.Fromregion = Branch.id INNER JOIN Branch AS Region_1 ON BranchTransferM.ToRegion = Region_1.id where invid=" + ID).SingleOrDefault();
            var list = _context.Database.SqlQuery<BranchTransferM>("SELECT Br.invid, Br.date, Br.Frombranch, Br.Tobranch, Br.note, branch.name AS Toname, Branch_1.name AS Fromname FROM BranchTransferM AS Br INNER JOIN Branch AS Branch_1 ON Br.Frombranch = Branch_1.id INNER JOIN Branch AS branch ON Br.Tobranch = branch.id where invid=" + ID).SingleOrDefault();
            var detaillist = _context.Database.SqlQuery<BranchTransferDetail>("SELECT * from BranchTransferDetail where invid=" + ID).ToList();

            var SaleInvVM = new SaleInvVM
            {
                BranchTransfer = list,
                BranchTransferDetailList = detaillist,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);

        }

        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] qty, decimal[] rqty, string[] size, BranchTransferM BranchTransfer)
        {
            if (item_name != null)
            {
                //_context.Database.ExecuteSqlCommand("INSERT INTO  BranchTransferM(invid, date, Fromregion, ToRegion, note) VALUES (" + BranchTransfer.invid + ",'" + BranchTransfer.date + "','" + BranchTransfer.Fromregion + "','" + BranchTransfer.ToRegion + "','" + BranchTransfer.note + "')  ");

                _context.Database.ExecuteSqlCommand("UPDATE  BranchTransferM  SET  Frombranch ='" + BranchTransfer.Frombranch + "',Tobranch ='" + BranchTransfer.Tobranch + "',note ='" + BranchTransfer.note + "',status ='" + BranchTransfer.status + "'  where invid=" + BranchTransfer.invid);

                _context.Database.ExecuteSqlCommand("Delete From BranchTransferDetail where invid=" + BranchTransfer.invid);

                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT  INTO  BranchTransferDetail( pid, pname, qty,rqty, invid, size)  VALUES (" + id[i] + ",'" + item_name[i] + "','" + qty[i] + "','" + rqty[i] + "','" + BranchTransfer.invid + "','" + size[i] + "' )");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? ID)
        {
            if (ID > 0)
            {
                _context.Database.ExecuteSqlCommand("Delete From BranchTransferM where invid=" + ID);
                _context.Database.ExecuteSqlCommand("Delete From BranchTransferDetail where invid=" + ID);
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public JsonResult GetTransferPendingCountJson()
        {
            var branchid = Session["BranchId"] as List<int>;
            var str = "";
            if (branchid != null && branchid.Any()) 
            {
                //str = " AND tobranch IN (@branchIds)";
                str = " AND tobranch IN (" + string.Join(",", branchid)+ ")";
            }
            int transferPending = _context.Database.SqlQuery<BranchTransferM>("Select invid from BranchTransferM where status='pending' " + str).Count();

            //int batchCount = _context.tbl_BatchRequest.Where(x => x.status == "Requested" && x.department == "Lab").Count();
            return Json(new { transferPending }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BranchStockIndex()
        {
            var list = _context.Database.SqlQuery<OpeningStockBranch>("SELECT op.pid, op.pname, op.qty, op.branchid, b.name as branchname FROM OpeningStockBranch AS op INNER JOIN Branch AS b ON op.branchid = b.id").ToList();
            return View(list);
        }
        public ActionResult BranchOpeningStockCreate(OpeningStockBranch OpeningStockBranch)
        {
            
            var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID IN (SELECT CategoryID FROM Categories WHERE (RawProductCheck = 0)))").ToList();
            var Branch = _context.Database.SqlQuery<Branch>("SELECT id,name from branch").ToList();

            var SaleInvVM = new SaleInvVM
            {
                OpeningStockBranch = OpeningStockBranch,
                pro_listsss = pro_listsss.OrderBy(x=>x.ProductName),
                Branch_list = Branch,
           
            };
            return View(SaleInvVM);

        }
        [HttpPost]
        public ActionResult BranchStockSave(decimal[] id, decimal[] qty, string[] pname,decimal branchiid)
        {
            var branch = branchiid;
            if (branch>0) {
                var check = _context.Database.SqlQuery<int>("select ISNULL(Max(branchid),0) from OpeningStockBranch where branchid= " + branch).FirstOrDefault();
                if (check != 0)
                {
                    _context.Database.ExecuteSqlCommand("Delete From OpeningStockBranch  where branchid= " + branch);
                }
                for (int i = 0; i < id.Length; i++)
                {
                    //var id1= _context.Database.SqlQuery<decimal>("select ISNULL(Max(id),0)+1 from ProductPricingRegion  ").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("insert into OpeningStockBranch (pid,pname,qty,branchid) values('" + id[i] + "','" + pname[i] + "','" + qty[i] + "','" + branch + "')");


                }
            }
            return RedirectToAction("BranchStockIndex");
        }
        [HttpGet]
        public ActionResult UpdateTable(int branchid)
        {
            var updatedData = _context.Database.SqlQuery<OpeningStockBranch>("SELECT * from OpeningStockBranch where branchid=" + branchid).OrderBy(x => x.pname).ToList();
            return Json(updatedData, JsonRequestBehavior.AllowGet);
        }
    }
}