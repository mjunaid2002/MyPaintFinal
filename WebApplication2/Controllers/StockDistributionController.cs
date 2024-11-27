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
    public class StockDistributionController : Controller
    {
        // GET: StockDistribution
        private ApplicationDbContext _context;

        public StockDistributionController()
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

            var list = _context.Database.SqlQuery<DistributionMaster>("SELECT StockDistribution.invid, StockDistribution.date, StockDistribution.datetime, StockDistribution.FromStore, StockDistribution.ToStore, StockDistribution.note, StockDistribution.FromBranchId, StockDistribution.ToBranchId, StockDistribution.RegionId, Region.name AS regionname FROM StockDistribution INNER JOIN Region ON StockDistribution.RegionId = Region.id " + strquery).ToList();
            return View(list);
        }
        public ActionResult Create(DistributionMaster distribution)
        {
            distribution.date = DateTime.Now;
            distribution.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from StockDistribution").FirstOrDefault();
            var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID <> 1)").ToList();
            //var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck<>1)").ToList();
            var region = _context.Database.SqlQuery<Region>("Select * from region").ToList();
            var SaleInvVM = new SaleInvVM
            {
                Region_list = region,
                distribution = distribution,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);
        }
        [HttpPost]
        public ActionResult Save(string[] item_name, int[] id, decimal[] qty,decimal[] price,decimal[] total, string[] ToStore, string[] item_name3, int[] id3, decimal[] qty3, string[] packing3, DistributionMaster distribution)
        {
           
            
            distribution.invid = _context.Database.SqlQuery<decimal>("select ISNULL(Max(invid),0)+1 from StockDistribution").FirstOrDefault();
            
            if (item_name != null)
            {
                for (int i = 0; i < item_name.Count(); i++)
                {
                     _context.Database.ExecuteSqlCommand("INSERT INTO StockDistributionDetail(sr, pid, pname, qty, invid, FromBranchId, ToBranchId, FromStore, ToStore, date, price, total)  VALUES (" + i + "," + id[i] + ",'" + item_name[i] + "','" + qty[i] + "','" + distribution.invid + "','" + distribution.FromBranchId + "','" + distribution.ToBranchId + "','" + distribution.FromStore + "','" + ToStore[i] + "','" + DateTime.Today + "','" + price[i] + "','" + total[i] + "' )");
               }

                if (item_name3 != null) {

                    for (int i = 0; i < item_name3.Count(); i++)
                    {
                        _context.Database.ExecuteSqlCommand("INSERT INTO StockDistributionDetail1(sr,invid, pid, pname, qty, packing,date)  VALUES (" + i + "," + distribution.invid + "," + id3[i] + ",'" + item_name3[i] + "','" + qty3[i] + "','" + packing3[i] + "','" + DateTime.Today + "')");
                    }
                }

                _context.Database.ExecuteSqlCommand("INSERT INTO  StockDistribution(invid, date, datetime, FromStore, ToStore, note, FromBranchId, ToBranchId,RegionId) VALUES (" + distribution.invid + ",'" + distribution.date + "','" + DateTime.Now + "','" + distribution.FromStore + "','0','" + distribution.note + "','" + distribution.FromBranchId + "','" + distribution.ToBranchId + "','" + distribution.RegionId + "')  ");

            }
          


            return RedirectToAction("Index");
        }


        public ActionResult Edit(int? ID)
        {
            var pro_listsss = _context.Database.SqlQuery<Products>("SELECT ProductName, ProductID, UnitPrice, ReorderLevel, vattax, CategoryID, [desc], Active  FROM   Product WHERE (CategoryID <> 1)").ToList();

            //var pro_listsss = _context.Database.SqlQuery<Products>("select ProductName,ProductID,UnitPrice,ReorderLevel,vattax,CategoryID,[desc],Active from Product where CategoryID in (select CategoryID from Categories where RawProductCheck<>1)").ToList();
            var list = _context.Database.SqlQuery<DistributionMaster>("SELECT * from StockDistribution where invid="+ID).SingleOrDefault();
            var detaillist = _context.Database.SqlQuery<DistributionMasterDetail>("SELECT * from StockDistributionDetail where invid=" + ID).ToList();
            var detaillist1 = _context.Database.SqlQuery<DistributionMasterDetail1>("SELECT * from StockDistributionDetail1 where invid=" + ID).ToList();
            var region = _context.Database.SqlQuery<Region>("Select * from region").ToList();

            var SaleInvVM = new SaleInvVM
            {
                Region_list = region,
                distribution = list,
                distributionDetailList = detaillist,
                distributionDetail1list = detaillist1,
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
            var list = _context.Database.SqlQuery<DistributionMaster>("SELECT * from StockDistribution where invid="+ID).SingleOrDefault();
            var detaillist = _context.Database.SqlQuery<DistributionMasterDetail>("SELECT * from StockDistributionDetail where invid=" + ID).ToList();

            var SaleInvVM = new SaleInvVM
            {
                distribution = list,
                distributionDetailList = detaillist,
                pro_listsss = pro_listsss
            };
            return View(SaleInvVM);
           
        }

        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] qty, decimal[] price, decimal[] total, string[] ToStore, string[] item_name3, int[] id3, decimal[] qty3, string[] packing3, DistributionMaster distribution)
        {
          
           
            if (item_name != null)
            {
                _context.Database.ExecuteSqlCommand("UPDATE  StockDistribution  SET  FromStore ='"+ distribution.FromStore + "', ToStore ='0', note ='" + distribution.note + "', FromBranchId ='" + distribution.FromBranchId + "', ToBranchId ='" + distribution.ToBranchId + "' , RegionId ='" + distribution.RegionId + "'  where invid=" + distribution.invid);

                _context.Database.ExecuteSqlCommand("Delete From StockDistributionDetail where invid=" + distribution.invid);
                _context.Database.ExecuteSqlCommand("Delete From StockDistributionDetail1 where invid=" + distribution.invid);

                for (int i = 0; i < item_name.Count(); i++)
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO StockDistributionDetail(sr, pid, pname, qty, invid, FromBranchId, ToBranchId, FromStore, ToStore, date,price,total)  VALUES (" + i + "," + id[i] + ",'" + item_name[i] + "'," + qty[i] + "," + distribution.invid + ",'" + distribution.FromBranchId + "','" + distribution.ToBranchId + "','" + distribution.FromStore + "','" + ToStore[i] + "','" + DateTime.Today + "','"+price[i]+"','"+total[i]+"')");
                }

                if (item_name3 != null)
                {

                    for (int i = 0; i < item_name3.Count(); i++)
                    {
                        _context.Database.ExecuteSqlCommand("INSERT INTO StockDistributionDetail1(sr,invid, pid, pname, qty, packing,date)  VALUES (" + i + "," + distribution.invid + "," + id3[i] + ",'" + item_name3[i] + "','" + qty3[i] + "','" + packing3[i] + "','" + DateTime.Today + "')");
                    }
                }
            }


            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? ID)
        {
            _context.Database.ExecuteSqlCommand("Delete From StockDistribution where invid=" + ID);
            _context.Database.ExecuteSqlCommand("Delete From StockDistributionDetail where invid=" + ID);
            _context.Database.ExecuteSqlCommand("Delete From StockDistributionDetail1 where invid=" + ID);

            return RedirectToAction("Index");

        }
    }
}