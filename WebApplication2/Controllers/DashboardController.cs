using System;
using System.Linq;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class DashboardController : Controller
    {
        private ApplicationDbContext _context;
        public DashboardController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult IndexTemp()
        {
            //Total Companies
            Session["t_comp"] = _context.Database.SqlQuery<int>("Select COunt(*) from BusinessUnits ").FirstOrDefault();

            //Active Companies

            Session["active_comp"] = _context.Database.SqlQuery<int>("select  COunt(*) from BusinessUnits where expiraydate > '2021-08-03'").FirstOrDefault();


            //Inactive COmpanies
            Session["inactive_comp"] = _context.Database.SqlQuery<int>("select  COunt(*) from BusinessUnits where expiraydate < '2021-08-03'").FirstOrDefault();


            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
            public ActionResult Index1(BusinessUnit BusinessUnits)
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);


            //Total Companies
            Session["t_comp"] = _context.Database.SqlQuery<int>("Select COunt(*) from BusinessUnits ").FirstOrDefault();

            //Active Companies

            Session["active_comp"] = _context.Database.SqlQuery<int>("select  COunt(*) from BusinessUnits where expiraydate > '2021-08-03'").FirstOrDefault();


            //Inactive COmpanies
            Session["inactive_comp"] = _context.Database.SqlQuery<int>("select  COunt(*) from BusinessUnits where expiraydate < '2021-08-03'").FirstOrDefault();



            DateTime dt = DateTime.Now;
            Session["mon_sale"] = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasters where  month(date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_sale"] = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasters where  day(date) = '" + dt.Day + "' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_pur"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(GRandTotal),0) as sum From [dbo].[PurMasters] where  month(date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_pur"] = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From [dbo].[PurMasters]  where day(date) = '" + dt.Day + "' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_pro"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(Gorss_total),0) as sum From [ProductionMaster_new] where  month(s_date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_pro"] = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(Gorss_total),0) as sum From [ProductionMaster_new]  where day(s_date) = '" + dt.Day + "' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_igp"] = _context.Database.SqlQuery<int>("Select   Count(*) From [GatepassMaster] where  month(date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_igp"] = _context.Database.SqlQuery<int>("Select  Count(*) From [GatepassMaster]  where day(date) = '" + dt.Day + "' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_ogp"] = _context.Database.SqlQuery<int>("Select   Count(*) From [dbo].[DeliveryNoteMaster] where  month(date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_ogp"] = _context.Database.SqlQuery<int>("Select  Count(*) From [dbo].[DeliveryNoteMaster]  where day(date) = '" + dt.Day + "' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_rec"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(Cr),0) as sum From TransactionDetails where  month(transdate) = '" + dt.Month + "' and TransDes='customer' and b_unit='" + Bunit + "' ").FirstOrDefault();
            Session["dal_rec"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(Cr),0) as sum From TransactionDetails where  day(transdate) = '" + dt.Day + "' and TransDes='customer' and b_unit='" + Bunit + "'").FirstOrDefault();

            Session["mon_pay"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(dr),0) as sum From TransactionDetails where  month(transdate) = '" + dt.Month + "' and Accountid in (select accountno from suppliers)  and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["dal_payment"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(dr),0) as sum From TransactionDetails where  day(transdate) = '" + dt.Day + "' and Accountid in (select accountno from suppliers)  and b_unit='" + Bunit + "'").FirstOrDefault();


            //Customer Balances
            var cus_bal = _context.Database.SqlQuery<dashboardquery>("SELECT Top(6) Name, isnull(total,0) as total,isnull(op,0) as op FROM(SELECT dbo.Customers.Name, SUM(dbo.TransactionDetails.Dr - dbo.TransactionDetails.Cr) AS total,(SELECT SUM(dr -cr) FROM OpeningBalance where OpeningBalance.accountno=dbo.Customers.AccountNo) as op FROM dbo.Customers INNER JOIN dbo.TransactionDetails ON dbo.Customers.AccountNo = dbo.TransactionDetails.AccountId where TransactionDetails.b_unit='" + Bunit + "' GROUP BY dbo.Customers.Name, dbo.Customers.AccountNo order by total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();
            //Supplier Balance
            var sup_bal = _context.Database.SqlQuery<dashboardquery>("SELECT Top(6) Name, isnull(total,0) as total,isnull(op,0) as op FROM(SELECT dbo.Suppliers.Name, SUM(dbo.TransactionDetails.cr - dbo.TransactionDetails.dr) AS total, (SELECT SUM(cr -dr) FROM OpeningBalance where OpeningBalance.accountno=dbo.Suppliers.AccountNo) as op FROM dbo.Suppliers INNER JOIN dbo.TransactionDetails ON dbo.Suppliers.AccountNo = dbo.TransactionDetails.AccountId where TransactionDetails.b_unit='" + Bunit + "' GROUP BY dbo.Suppliers.Name, dbo.Suppliers.AccountNo order by total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();
            //Customer aging
            var cus_aging = _context.Database.SqlQuery<dashboardquery>("SELECT Top(6) Name, total,days FROM(Select(Select TOP(1) name From customers where Accountno=salemasters.CustomerId and Customers.BusinessUnit='" + Bunit + "') as Name, (Select SUM(dr-cr) From TransactionDetails where AccountId=salemasters.CustomerId) as total ,DATEDIFF(day, GETDATE(), date) days from salemasters where b_unit='" + Bunit + "'  order by days desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();

            //Revenue
            decimal t_sale_currentmonth = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasters where  month(date) = '" + dt.Month + "' and b_unit='" + Bunit + "'").FirstOrDefault();
            decimal t_salereturn_currentmonth = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasterReturns where  month(date) = '" + dt.Month + "'  and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["curr_mon_rev"] = t_sale_currentmonth - t_salereturn_currentmonth;

            int prev_mon = dt.Month - 1;
            decimal t_sale_prevmonth = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasters where  month(date) = " + prev_mon + " and b_unit='" + Bunit + "'").FirstOrDefault();
            decimal t_salereturn_prevmonth = _context.Database.SqlQuery<decimal>("Select  Isnull(SUM(GRandTotal),0) as sum From SaleMasterReturns where  month(date) = " + prev_mon + " and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["prev_mon_rev"] = t_sale_prevmonth - t_salereturn_prevmonth;


            //Expense
            Session["t_exp_currmonth"] = _context.Database.SqlQuery<decimal>("Select Top(1)(Select Isnull(SUM(dr),0) as sum From TransactionDetails where Accountid=accounttitles.Accountno and month(transdate) = '" + dt.Month + "'  and b_unit='" + Bunit + "') as sum from accounttitles where Accountheadid=5 and b_unit='" + Bunit + "'  ").FirstOrDefault();
            Session["t_exp_prevmonth"] = _context.Database.SqlQuery<decimal>("Select Top(1)(Select Isnull(SUM(dr),0) as sum From TransactionDetails where Accountid=accounttitles.Accountno and month(transdate) = " + prev_mon + "  and b_unit='" + Bunit + "') as sum from accounttitles where Accountheadid=5 and b_unit='" + Bunit + "' ").FirstOrDefault();

            //NetProfit
            decimal net_curr = Convert.ToDecimal(Session["curr_mon_rev"]) - Convert.ToDecimal(Session["t_exp_currmonth"]);
            decimal prev_curr = Convert.ToDecimal(Session["prev_mon_rev"]) - Convert.ToDecimal(Session["t_exp_prevmonth"]);
            Session["net_currmonth"] = net_curr;
            Session["net_prevmonth"] = prev_curr;

            //Capital
            Session["t_cap_currmonth"] = _context.Database.SqlQuery<decimal>("Select Top(1) (Select Isnull(SUM(cr),0) as sum From TransactionDetails where Accountid=accounttitles.Accountno and month(transdate) = '" + dt.Month + "') as sum from accounttitles where Accountheadid=3 and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["t_cap_prevmonth"] = _context.Database.SqlQuery<decimal>("Select Top(1) (Select Isnull(SUM(cr),0) as sum From TransactionDetails where Accountid=accounttitles.Accountno and month(transdate) = " + prev_mon + ") as sum from accounttitles where Accountheadid=3 and b_unit='" + Bunit + "'").FirstOrDefault();

            //Recovery
            Session["t_rec_currmonth"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(Cr),0) as sum From TransactionDetails where  month(transdate) = '" + dt.Month + "' and TransDes='customer' and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["t_rec_prevmonth"] = _context.Database.SqlQuery<decimal>("Select   Isnull(SUM(Cr),0) as sum From TransactionDetails where  month(transdate) = " + prev_mon + " and TransDes='customer' and b_unit='" + Bunit + "'").FirstOrDefault();

            //Receivalbe
            Session["t_recv_currmonth"] = _context.Database.SqlQuery<decimal>("Select Isnull( SUM(dr-cr) +(SELECT SUM(dr - cr) FROM OpeningBalance where OpeningBalance.accountno in(select accountno from customers)), 0) as sum From TransactionDetails where month(transdate) = " + dt.Month + " and Accountid in (select accountno from customers) and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["t_recv_prevmonth"] = _context.Database.SqlQuery<decimal>("Select Isnull( SUM(dr-cr) +(SELECT SUM(dr - cr) FROM OpeningBalance where OpeningBalance.accountno in(select accountno from customers)), 0) as sum From TransactionDetails where month(transdate) = " + prev_mon + " and Accountid in (select accountno from customers) and b_unit='" + Bunit + "'").FirstOrDefault();

            //Payable
            Session["t_pay_currmonth"] = _context.Database.SqlQuery<decimal>("Select Isnull( SUM(cr-dr) +(SELECT SUM(cr - dr) FROM OpeningBalance where OpeningBalance.accountno in (select accountno from suppliers)), 0) as sum From TransactionDetails where month(transdate) = " + dt.Month + " and Accountid in (select accountno from suppliers)  and b_unit='" + Bunit + "'").FirstOrDefault();
            Session["t_pay_prevmonth"] = _context.Database.SqlQuery<decimal>("Select Isnull( SUM(cr-dr) +(SELECT SUM(cr - dr) FROM OpeningBalance where OpeningBalance.accountno in (select accountno from suppliers)), 0) as sum From TransactionDetails where month(transdate) = " + prev_mon + " and Accountid in (select accountno from suppliers)  and b_unit='" + Bunit + "'").FirstOrDefault();

            //Cash/Bank Balance
            decimal bal_cashbankcurr = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = '" + dt.Month + "' and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE  b_unit='" + Bunit + "' and (SecondLevel IN ('1000002')) ))").FirstOrDefault();
            int opp_cashbankcurr = _context.Database.SqlQuery<int>("select  Isnull(SUm(dr-cr),0) from AccountTitles where SecondLevel in ('1000002') ").FirstOrDefault();
            Session["cashbankcurr"] = bal_cashbankcurr + opp_cashbankcurr;
            decimal bal_cashbankprev = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = " + prev_mon + " and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE b_unit='" + Bunit + "' and (SecondLevel IN ( '1000002'))))").FirstOrDefault();
            Session["cashbankprev"] = bal_cashbankprev + opp_cashbankcurr;


            decimal bal_cashcurr = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = '" + dt.Month + "' and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE  b_unit='" + Bunit + "' and (SecondLevel IN ('1000001')) ))").FirstOrDefault();
            int opp_cashcurr = _context.Database.SqlQuery<int>("select  isnull(SUm(dr-cr),0) from AccountTitles where SecondLevel in ('1000001') ").FirstOrDefault();
            Session["cashkcurr"] = bal_cashcurr + opp_cashcurr;
            decimal bal_cashprev = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = " + prev_mon + " and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE b_unit='" + Bunit + "' and (SecondLevel IN ('1000001'))))").FirstOrDefault();
            Session["cashprev"] = bal_cashprev + opp_cashbankcurr;


            decimal bal_stockcurr = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = '" + dt.Month + "' and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE  b_unit='" + Bunit + "' and (SecondLevel IN ('1000003')) ))").FirstOrDefault();
            int opp_stockcurr = _context.Database.SqlQuery<int>("select  isnull(SUm(dr-cr),0) from AccountTitles where SecondLevel in ('1000003') ").FirstOrDefault();
            Session["stockcurr"] = bal_stockcurr + opp_stockcurr;
            decimal bal_stockprev = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = " + prev_mon + " and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE b_unit='" + Bunit + "' and (SecondLevel IN ('1000003'))))").FirstOrDefault();
            Session["stockprev"] = bal_stockprev + opp_cashbankcurr;



            decimal bal_cuscurr = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = '" + dt.Month + "' and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE  b_unit='" + Bunit + "' and (SecondLevel IN ('1000004')) ))").FirstOrDefault();
            int opp_cuscurr = _context.Database.SqlQuery<int>("select  isnull(SUm(dr-cr),0) from AccountTitles where SecondLevel in ('1000004') ").FirstOrDefault();
            Session["cuscurr"] = bal_cuscurr + opp_cuscurr;
            decimal bal_cusprev = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(Dr) - SUM(Cr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = " + prev_mon + " and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE b_unit='" + Bunit + "' and (SecondLevel IN ('1000004'))))").FirstOrDefault();
            Session["cusprev"] = bal_cusprev + opp_cashbankcurr;


            decimal bal_supcurr = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(cr) - SUM(dr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = '" + dt.Month + "' and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE  b_unit='" + Bunit + "' and (SecondLevel IN ('2000001')) ))").FirstOrDefault();
            int opp_sucurr = _context.Database.SqlQuery<int>("select  isnull(SUm(cr-dr),0) from AccountTitles where SecondLevel in ('2000001') ").FirstOrDefault();
            Session["sucurr"] = bal_supcurr + opp_sucurr;
            decimal bal_suprev = _context.Database.SqlQuery<decimal>("SELECT isnull(SUM(cr) - SUM(dr),0) AS Expr1 FROM dbo.TransactionDetails WHERE   month(transdate) = " + prev_mon + " and (AccountId IN (SELECT AccountNo FROM dbo.AccountTitles WHERE b_unit='" + Bunit + "' and (SecondLevel IN ('2000001'))))").FirstOrDefault();
            Session["suprev"] = bal_suprev + opp_cashbankcurr;

            //var sale_list = _context.Database.SqlQuery<BusinessUnit>("SELECT * FROM BusinessUnits where id = "+Session["BusinessUnit"] +"").ToList();
            var sale_list = _context.Database.SqlQuery<dashboardquery>("SELECT TOP(6) Date,SUM(GrandTotal) as total FROM SaleMasters where b_unit='" + Bunit + "'  Group by date Order by Date desc ").ToList();
            var pur_dash = _context.Database.SqlQuery<dashboardquery>("SELECT TOP(6) Date,SUM(GrandTotal) as total FROM PurMasters where  b_unit='" + Bunit + "'  Group by date Order by Date desc").ToList();

            var sale_list_woc = _context.Database.SqlQuery<Salequery>("SELECT TOP(10) DATEDIFF(day, GETDATE(), date) days,*,(Select TOP(1) name From Customers where AccountNo=SaleMasters.CustomerId and Customers.BusinessUnit='" + Bunit + "') as CustomerName ,(Select name From Employees where AccountNo=SaleMasters.DsrId) as EmployeeName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var salereturn_list_woc = _context.Database.SqlQuery<Salequery>("SELECT TOP(10) DATEDIFF(day, GETDATE(), date) days,*,(Select TOP(1) name From Customers where AccountNo=[SaleMasterReturns].CustomerId and Customers.BusinessUnit='" + Bunit + "') as CustomerName ,(Select name From Employees where AccountNo=[SaleMasterReturns].DsrId) as EmployeeName FROM [SaleMasterReturns] where InvType = 'SINVWOCTN' and  b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var top_customers_sale = _context.Database.SqlQuery<Salequery>("Select TOP(10) GrandTotal,CustomerName from( SELECT SUM(GrandTotal) as GrandTotal,(Select TOP(1) name From Customers where AccountNo=SaleMasters.CustomerId and Customers.BusinessUnit='" + Bunit + "') as CustomerName FROM SaleMasters where InvType = 'SINVWOCTN' and b_unit='" + Bunit + "' Group by CustomerId order by GrandTotal desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();

            var bank_cash = _context.Database.SqlQuery<Salequery>("Select TOP(10) Total,CustomerName from( select AccountTitleName as CustomerName,Isnull((Select SUm(dr-cr) from TransactionDetails where Accountid=AccountTitles.accountno and b_unit='" + Bunit + "'),0)as Total from AccountTitles where b_unit='" + Bunit + "' and SecondLevel in('1000001','1000002') group by accmain,accountno,AccountTitleName order by Total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();

            var cus_re = _context.Database.SqlQuery<Salequery>("Select 'Recovery' as CustomerName, Isnull(Sum(Total),0) as Total from( select Isnull((Select SUm(cr) from TransactionDetails where Accountid=AccountTitles.accountno and b_unit='" + Bunit + "'),0)as Total from AccountTitles where b_unit='" + Bunit + "' and SecondLevel in('1000004') group by accmain,accountno,AccountTitleName order by Total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();
            var cus_short = _context.Database.SqlQuery<Salequery>("Select 'ShortFall' as CustomerName, Isnull(Sum(Total),0) as Total from( select Isnull((Select SUm(dr-cr) from TransactionDetails where Accountid=AccountTitles.accountno and b_unit='" + Bunit + "'),0)as Total from AccountTitles where b_unit='" + Bunit + "' and SecondLevel in('1000004') group by accmain,accountno,AccountTitleName order by Total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();

            var supp_re = _context.Database.SqlQuery<Salequery>("Select 'Recovery' as CustomerName, Isnull(Sum(Total),0) as Total from( select Isnull((Select SUm(dr) from TransactionDetails where Accountid=AccountTitles.accountno and b_unit='" + Bunit + "'),0)as Total from AccountTitles where b_unit='" + Bunit + "' and SecondLevel in('2000001') group by accmain,accountno,AccountTitleName order by Total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();
            var supp_short = _context.Database.SqlQuery<Salequery>("Select 'ShortFall' as CustomerName, Isnull(Sum(Total),0) as Total from( select Isnull((Select SUm(cr-dr) from TransactionDetails where Accountid=AccountTitles.accountno and b_unit='" + Bunit + "'),0)as Total from AccountTitles where b_unit='" + Bunit + "' and SecondLevel in('2000001') group by accmain,accountno,AccountTitleName order by Total desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();


            var pur_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT TOP(10) DATEDIFF(day, GETDATE(), date) days,*,(Select TOP(1) name From Suppliers where AccountNo=PurMasters.SupplierId and Suppliers.BusinessUnit='" + Bunit + "') as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var purreturn_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT TOP(10) DATEDIFF(day, GETDATE(), date) days,*,(Select TOP(1) name From Suppliers where AccountNo=[PurMasterReturns].supplierid and Suppliers.BusinessUnit='" + Bunit + "') as SupplierName ,(Select name From Employees where AccountNo=[PurMasterReturns].DsrId) as EmployeeName FROM [PurMasterReturns] where InvType = 'PINVWOCTN' and b_unit='" + Bunit + "' ORDER BY InvID DESC").ToList();
            var top_supp_pur = _context.Database.SqlQuery<PurchaseQuery>("Select TOP(10) GrandTotal,SupplierName from( SELECT SUM(GrandTotal) as GrandTotal,(Select TOP(1) name From Suppliers where AccountNo=PurMasters.Supplierid and Suppliers.BusinessUnit='" + Bunit + "') as SupplierName FROM PurMasters where InvType = 'PINVWOCTN' and b_unit='" + Bunit + "' Group by Supplierid order by GrandTotal desc OFFSET 0 ROWS) AS derivedtbl_1").ToList();


            var payables = _context.Database.SqlQuery<PurchaseQuery>("select TOP(6) (Select TOP(1) name From Suppliers where AccountNo=TransactionDetails.AccountId and Suppliers.BusinessUnit='" + Bunit + "') as SupplierName,SUM(Cr)-SUM(Dr) as Bal,isnull((SELECT SUM(cr - dr) FROM OpeningBalance where OpeningBalance.accountno = TransactionDetails.AccountId), 0) as op, AccountId from TransactionDetails where Accountid in (select accountno from suppliers) and b_unit='" + Bunit + "' group by AccountId").ToList();
            var rec = _context.Database.SqlQuery<PurchaseQuery>("select TOP(6) (Select TOP(1) name From Customers where AccountNo=TransactionDetails.AccountId and Customers.BusinessUnit='" + Bunit + "') as SupplierName,SUM(Dr)-SUM(Cr) as Bal,isnull((SELECT SUM(Dr - Cr) FROM OpeningBalance where OpeningBalance.accountno = TransactionDetails.AccountId), 0) as op, AccountId from TransactionDetails where Accountid in (select accountno from customers) and b_unit='" + Bunit + "' group by AccountId").ToList();
            //var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT   (Select name From Brands where id=ProductionMaster_new.batch_no) as BatchNo,yelid,NetAmount,net_final as BTotal,inv_id as InvID,s_date as Date,e_date as Phone,batch_no as Rtotal, labour_cost as CargoCharges,other_cost as DiscountAmount,total_cost as Total,gorss_total as GrandTotal from [ProductionMaster_new] where  b_unit='" + Bunit + "'").ToList();
            var list_new = _context.Database.SqlQuery<PurchaseQuery>("Select Isnull(SUm(Dr)-Sum(Cr),0) As Bal,Transdate,'Expense' as Type From TransactionDetails where Accountid Between '5000001' and '5999999' group by AccountId,Transdate UNION ALL Select SUm(Dr)-Sum(Cr) As Bal,Transdate,'Sale' as Type From TransactionDetails where Accountid Between '4000001' and '4999999' and b_unit='" + Bunit + "' group by AccountId,Transdate").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                cus_re = cus_re,
                cus_short = cus_short,
                supp_re = supp_re,
                bank_cash = bank_cash,
                supp_short = supp_short,
                top_supp_pur = top_supp_pur,
                purreturn_list_woc = purreturn_list_woc,
                top_customers_sale = top_customers_sale,
                cus_aging = cus_aging,
                sup_bal = sup_bal,
                cus_bal = cus_bal,
                list_new = list_new,
                //list = list,
                rec = rec,
                payables = payables,
                pur_list_woc = pur_list_woc,
                sale_list_woc = sale_list_woc,
                salereturn_list_woc = salereturn_list_woc,
                pur_dash = pur_dash,
                sale_list = sale_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult GraphTest(BusinessUnit BusinessUnits)
        {
            var sale_list = _context.Database.SqlQuery<dashboardquery>("SELECT Date,SUM(GrandTotal) as total FROM SaleMasters  Group by date Order by Date desc ").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                sale_list = sale_list,
            };
            return View(PurInvoiceVM);
        }
    }
}