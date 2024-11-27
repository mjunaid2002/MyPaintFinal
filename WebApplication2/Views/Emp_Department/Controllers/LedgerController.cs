using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.QueryViewModel;
using WebApplication2.Models;
using WebApplication2.ViewModels;
namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class LedgerController : Controller
    {
        private ApplicationDbContext _context;
        private List<LedgerQuery> trans_list;
        public LedgerController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Ledger
        public ActionResult Index(AccountTitle accountTitle)
        {
            var third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles").ToList();
            //DateTime startdate = _context.Database.SqlQuery<DateTime>("select StartDate from Sessions where year(StartDate)=YEAR(GETDATE()) and b_unit='" + Bunit + "'").FirstOrDefault();
            //ViewBag.StartDate = Convert.ToDateTime(startdate).ToString("yyyy-MM-dd");
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = third_level,
                accountTitle = accountTitle,
            };
            return View(Chart_of_account_Vm);
        }
        [HttpPost]
        public ActionResult LedgerSearch(AccountTitle accountTitle, string s_date, string e_date, TransactionDetail transactionDetail)
        {
            int dr = _context.Database.SqlQuery<int>("SELECT  isnull(SUM (Dr),0) as Dr FROM   OpeningBalance where AccountNo = " + accountTitle.AccountNo).FirstOrDefault();
            int cr = _context.Database.SqlQuery<int>("SELECT  isnull(SUM (Cr),0) as Cr FROM   OpeningBalance where AccountNo = " + accountTitle.AccountNo).FirstOrDefault();
            decimal op = 0;
            int headid = _context.Database.SqlQuery<int>("SELECT AccountHeadId from  AccountTitles where accountno=" + accountTitle.AccountNo + "").FirstOrDefault();
            int opening_bal = 0;
            if (headid == 1 || headid == 5)
            {
                op = _context.Database.SqlQuery<decimal>("SELECT   isnull(SUM (dr-cr),0) as Cr FROM   TransactionDetails  where TransDate < '" + s_date + "'  and AccountId = " + accountTitle.AccountNo + "").FirstOrDefault();
                opening_bal = dr - cr;
            }
            else if (headid == 2 || headid == 4)
            {
                op = _context.Database.SqlQuery<decimal>("SELECT   isnull(SUM (cr-dr),0) as Cr FROM   TransactionDetails  where TransDate < '" + s_date + "'  and AccountId = " + accountTitle.AccountNo + " ").FirstOrDefault();
                opening_bal = cr - dr;
            }
            if (accountTitle.AccountNo == 0)
            {
                // trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.ID, td.TransId, td.TransDes,td.TransDes as TransDetail, td.TransDate, td.AccountId, td.Dr, td.Cr, td.InvId, td.Vtype, td.b_unit, td.Rinvid, act.AccountNo, act.AccMain, act.AccountHeadId, act.SecondLevel, act.AccountTitleName,act.AccountType from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.TransDate between '" + s_date + "' AND '" + e_date + "' order by td.TransDate,td.TransId").ToList();
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.ID, td.TransId, td.TransDes,td.TransDes as TransDetail, td.TransDate, td.AccountId, td.Dr, td.Cr, td.InvId, td.Vtype, td.b_unit, act.AccountNo, act.AccMain, act.AccountHeadId, act.SecondLevel, act.AccountTitleName,act.AccountType from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.TransDate between '" + s_date + "' AND '" + e_date + "' order by td.TransDate,td.TransId").ToList();

            }
            else if (accountTitle.AccountNo != 0 && s_date != "" && e_date != "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.ID, td.TransId, td.TransDes, td.TransDate, td.AccountId, td.Dr, td.Cr, td.InvId, td.Vtype, at.AccountHeadId FROM TransactionDetails AS td INNER JOIN AccountTitles AS at ON td.AccountId = at.AccountNo WHERE td.AccountId = " + accountTitle.AccountNo + " AND td.TransDate between '" + s_date + "' AND '" + e_date + "' order by td.TransDate,td.TransId").ToList();

            }
            else if (s_date == "" && e_date == "")
            {
                trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT td.ID, td.TransId, td.TransDes,td.TransDes as TransDetail, td.TransDate, td.AccountId, td.Dr, td.Cr, td.InvId, td.Vtype, act.AccountNo, act.AccMain, act.AccountHeadId, act.SecondLevel, act.AccountTitleName,act.AccountType from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + accountTitle.AccountNo + " order by td.TransDate,td.TransId").ToList();

            }
            _context.Database.ExecuteSqlCommand("Delete From  TempTransDetails where userid=" + Session["UserID"].ToString());

            foreach (var item in trans_list) {

                _context.Database.ExecuteSqlCommand("INSERT  INTO TempTransDetails(userid,TransId, TransDes, TransDate, AccountId, Dr, Cr, InvId, Vtype, b_unit, Rinvid, AccountNo, AccMain, AccountHeadId, SecondLevel, AccountTitleName, AccountType)  " +
                       "VALUES('" + Session["UserID"].ToString() + "','" + @item.TransId + "','" + @item.TransDes + "','" + @item.TransDate + "','" + @item.AccountId + "','" + @item.Dr + "','" + @item.Cr + "','" + @item.InvId + "','" + @item.Vtype + "','" + @item.AccountType + "','" + @item.Rinvid + "','" + @item.AccountNo + "','" + @item.AccMain + "','" + @item.AccountHeadId + "','" + @item.SecondLevel + "','" + @item.AccountTitleName + "','" + @item.AccountType + "') ");


            }
            var des = "";
            foreach (var item in trans_list.Where(x => x.Vtype != "CRV" && x.Vtype != "CPV" && x.Vtype != "BPV" && x.Vtype != "BRV" && x.Vtype != "JV"))
            {
                if (@item.Vtype == "TSRINV" || @item.Vtype == "WTSINV" || @item.Vtype == "TSINV"  || @item.Vtype == "SRINV") {

                    des = _context.Database.SqlQuery<string>("SELECT STUFF(( SELECT ', ' + SUBSTRING(packing, 1, 2) + '-' + dbo.ExtractNumber(prname) + ' | ' + CAST(qty AS VARCHAR) FROM srsdetail WHERE OrderID = '"+item.InvId+"' FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS desp").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("UPDATE  TempTransDetails  SET  TransDes ='"+des+ "' where TransId ="+item.TransId);


                }

                else if (@item.Vtype == "SINVRAW") 
                {
                    des = _context.Database.SqlQuery<string>("SELECT STUFF(( SELECT ', ' + SUBSTRING(packing, 1, 2) + '-' + dbo.ExtractNumber(prname) + ' | ' + CAST(qty AS VARCHAR) FROM Order_detail WHERE OrderID = '" + item.InvId + "' FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS desp").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("UPDATE  TempTransDetails  SET  TransDes ='" + des + "' where TransId =" + item.TransId);

                }
                
                else if (@item.Vtype == "SINV") 
                {
                    des = _context.Database.SqlQuery<string>("SELECT STUFF(( SELECT ', ' + SUBSTRING(packing, 1, 2) + '-' + dbo.ExtractNumber(prname) + ' | ' + CAST(qty AS VARCHAR) FROM Orderdetail WHERE OrderID = '" + item.InvId + "' FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS desp").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("UPDATE  TempTransDetails  SET  TransDes ='" + des + "' where TransId =" + item.TransId);

                }

                else if (@item.Vtype == "PINV" || @item.Vtype == "PTINV") 
                {
                    des = _context.Database.SqlQuery<string>("SELECT STUFF(( SELECT ', ' + pname + ' | ' + CAST(qty AS VARCHAR) FROM purchasedetail WHERE invid = '" + item.InvId + "' FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS desp").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("UPDATE  TempTransDetails  SET  TransDes ='" + des + "' where TransId =" + item.TransId);


                }

                else if (@item.Vtype == "PRINV" || @item.Vtype == "TPRINV") 
                {
                    des = _context.Database.SqlQuery<string>("SELECT STUFF(( SELECT ', ' + pname + ' | ' + CAST(qty AS VARCHAR) FROM srpdetail WHERE invid = '" + item.InvId + "' FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS desp").FirstOrDefault();
                    _context.Database.ExecuteSqlCommand("UPDATE  TempTransDetails  SET  TransDes ='" + des + "' where TransId =" + item.TransId);


                }



            }
            //for (int i = 0; i < trans_list.Count; i++)
            //{

            //    if (trans_list[i].AccountType == "")
            //    {

            //    }

            //}

            trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT * From TempTransDetails where userid=" + Session["UserID"] ).ToList();


            accountTitle.cr = opening_bal + Convert.ToInt32(op);
            accountTitle.AccountTitleName = _context.Database.SqlQuery<string>("SELECT AccountTitleName FROM AccountTitles WHERE AccountNo = " + accountTitle.AccountNo + "").FirstOrDefault();
            accountTitle.AccountNo = _context.Database.SqlQuery<int>("SELECT AccountNo FROM AccountTitles WHERE AccountNo = " + accountTitle.AccountNo + "").FirstOrDefault();
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                third_level = _context.Database.SqlQuery<AccountTitle>("SELECT * FROM AccountTitles ").ToList(),
                accountTitle = accountTitle,
                transactionDetail = transactionDetail,
                trans_list = trans_list,
            };
            ViewBag.StartDate = Convert.ToDateTime(s_date).ToString("yyyy-MM-dd");
            ViewBag.EndDate = Convert.ToDateTime(e_date).ToString("yyyy-MM-dd");
            return View(Chart_of_account_Vm);
        }
        public ActionResult LedgerSearchSingle(int? ID, int status)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            int dr = _context.Database.SqlQuery<int>("SELECT  SUM (OpeningBalance.Dr) as Dr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id where OpeningBalance.AccountNo = " + ID + "  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit,Sessions.StartDate, Sessions.EndDate").FirstOrDefault();
            int cr = _context.Database.SqlQuery<int>("SELECT   SUM (OpeningBalance.Cr) as Cr FROM   OpeningBalance INNER JOIN dbo.AccountTitles ON OpeningBalance.AccountNo = dbo.AccountTitles.AccountNo INNER JOIN Sessions ON OpeningBalance.Session_id = Sessions.Id where  OpeningBalance.AccountNo = " + ID + "  Group by OpeningBalance.Session_id,OpeningBalance.AccountNo,OpeningBalance.b_unit,Sessions.StartDate, Sessions.EndDate").FirstOrDefault();

            int headid = _context.Database.SqlQuery<int>("SELECT AccountHeadId from  AccountTitles where accountno=" + ID + "").FirstOrDefault();
            int opening_bal = 0;
            if (headid == 1 || headid == 5)
            {
                opening_bal = dr - cr;
            }
            else if (headid == 2 || headid == 4)
            {
                opening_bal = cr - dr;
            }

            ViewBag.AccountTitleName = _context.Database.SqlQuery<string>("SELECT AccountTitleName FROM AccountTitles WHERE AccountNo = " + ID + "").FirstOrDefault();
            trans_list = _context.Database.SqlQuery<LedgerQuery>("SELECT (SELECT TOP(1) (SELECT AccountTitleName FROM AccountTitles WHERE (AccountNo <> 10000003) AND (AccountNo = TransactionDetails.AccountId)) AS Expr1 FROM TransactionDetails WHERE (TransId = td.TransId)) AS facc,td.* , act.* from TransactionDetails td , AccountTitles act WHERE td.AccountId = act.Accountno AND td.AccountId = " + ID + " and td.b_unit='" + Bunit + "' ").ToList();
            ViewBag.cr = opening_bal;
            ViewBag.status = status;
            var Chart_of_account_Vm = new ChartofaccountVm
            {
                trans_list = trans_list,
            };
            ViewBag.StartDate = Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy");
            ViewBag.EndDate = Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy");
            return View(Chart_of_account_Vm);
        }

    }
}