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
using WebApplication2.ViewModels;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ItemLedgerController : Controller
    {
        private ApplicationDbContext _context;
        private List<Item_ledger> pro_listsss;

        public ItemLedgerController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: ItemLedger
        public ActionResult Index()
        {
            var item_list = _context.Database.SqlQuery<Product>("SELECT * FROM Products where BusinessUnit='" + Session["BusinessUnit"] + "' ").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                item_list = item_list
            };
            return View(Chart_of_account_Vm);
        }
        [HttpPost]
        public ActionResult LedgerSearch(string e_date, decimal item_id, string s_date)
        {
            //var query = "";
            //if (b_status == false)
            //{
            //    query = "AND b_unit = '" + Session["BusinessUnit"] + "' ";
            //}
            _context.Database.ExecuteSqlCommand("truncate table Item_legder");

            //var select_sale = _context.Database.SqlQuery<Item_ledger>("Select ISNULL(SUM(qty),0) as QTYOut from SaleDetails where ItemID=" + item_id + " and date='" + e_date + "'").ToList(); ;
            //var select_pur = _context.Database.SqlQuery<Item_ledger>("Select ISNULL(SUM(qty),0) as QTYIn from PurDetails where ItemID=" + item_id + " and date='" + e_date + "'").ToList(); ;
            //var balalnce = _context.Database.SqlQuery<Item_ledger>("Select ISNULL(SUM(NetTotal),0) as balance from PurDetails where ItemID=" + item_id + " and date < '" + e_date + "'").ToList(); ;

            decimal opsdetail = _context.Database.SqlQuery<decimal>("Select ISNULL(SUM(qty),0) as qty from SaleDetails where Date < '" + s_date + "' and ItemId="+ item_id + " and b_unit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            decimal oppdetail = _context.Database.SqlQuery<decimal>("Select ISNULL(SUM(qty),0) as qty from PurDetails where Date < '" + s_date + "' and ItemId="+ item_id + " and b_unit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            int opsrdetail = _context.Database.SqlQuery<int>("Select ISNULL(SUM(qty),0) as qty from SaleDetailReturns where Date < '" + s_date + "' and ItemId="+ item_id + " and b_unit='" + Session["BusinessUnit"] + "'").FirstOrDefault();
            int opprdetail = _context.Database.SqlQuery<int>("Select ISNULL(SUM(qty),0) as qty from PurDetailReturns where Date < '" + s_date + "' and ItemId="+ item_id + " and b_unit='" + Session["BusinessUnit"] + "'").FirstOrDefault();

            decimal op = ((oppdetail + opsrdetail) - opsdetail - opprdetail);


            var select_sale = _context.Database.SqlQuery<ItemLedgerquert>("Select ISNULL(sum(Qty),0) as qty,Date,ItemID from SaleDetails  where Date between '" + s_date + "' and '" + e_date + "' and ItemId=" + item_id + " and b_unit='" + Session["BusinessUnit"] + "'  group by Date,ItemID").ToList();
            foreach (var emplst in select_sale)
            {
                _context.Database.ExecuteSqlCommand("Insert INTO Item_legder (Date,Detail,sdetail,pdetail,srdetail,prdetail,item_id,b_unit) values('" + emplst.Date + "','Sales Invoice',"+emplst.Qty+",0.00,0,0," + item_id + ",'" + Session["BusinessUnit"] + "') ");
            }
            var select_pur = _context.Database.SqlQuery<ItemLedgerquert>("Select ISNULL(sum(Qty),0) as qty,Date,ItemID from PurDetails  where Date between '" + s_date + "' and '" + e_date + "' and ItemId=" + item_id + " and b_unit='" + Session["BusinessUnit"] + "'  group by Date,ItemID").ToList();
            foreach (var emplst in select_pur)
            {
                _context.Database.ExecuteSqlCommand("Insert INTO Item_legder (Date,Detail,sdetail,pdetail,srdetail,prdetail,item_id,b_unit) values('" + emplst.Date + "','Purchase Invoice',0.00," + emplst.Qty + ",0,0," + item_id + ",'" + Session["BusinessUnit"] + "') ");
            }
            var select_salereturn = _context.Database.SqlQuery<ItemLedgerquertyyyy>("Select ISNULL(sum(Qty),0) as qty,Date,ItemID from SaleDetailReturns  where Date between '" + s_date + "' and '" + e_date + "' and ItemId=" + item_id + " and b_unit='" + Session["BusinessUnit"] + "'  group by Date,ItemID").ToList();
            foreach (var emplst in select_salereturn)
            {
                _context.Database.ExecuteSqlCommand("Insert INTO Item_legder (Date,Detail,s`    detail,pdetail,srdetail,prdetail,item_id,b_unit) values('" + emplst.Date + "','Sales Return Invoice',0.00,0.00," + emplst.Qty + ",0," + item_id + ",'" + Session["BusinessUnit"] + "') ");
            }
            var select_purreturn = _context.Database.SqlQuery<ItemLedgerquertyyyy>("Select ISNULL(sum(Qty),0) as qty,Date,ItemID from PurDetailReturns  where Date between '" + s_date + "' and '" + e_date + "' and ItemId=" + item_id + " and b_unit='" + Session["BusinessUnit"] + "'  group by Date,ItemID").ToList();
            foreach (var emplst in select_purreturn)
            {
                _context.Database.ExecuteSqlCommand("Insert INTO Item_legder (Date,Detail,sdetail,pdetail,srdetail,prdetail,item_id,b_unit) values('" + emplst.Date + "','Purchase Return Invoice',0.00,0.00,0," + emplst.Qty + "," + item_id + ",'" + Session["BusinessUnit"] + "') ");
            }


            pro_listsss = _context.Database.SqlQuery<Item_ledger>("select * from Item_legder where b_unit = '" + Session["BusinessUnit"] + "'").ToList();
            var item_list = _context.Database.SqlQuery<Product>("SELECT * FROM Products where BusinessUnit='" + Session["BusinessUnit"] + "' ").ToList();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                pro_listsss = pro_listsss,
                item_list = item_list,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("dd-MM-yyyy");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("dd-MM-yyyy");
            ViewBag.Opening = op;
            return View(Chart_of_account_Vm);

        }
    }
}