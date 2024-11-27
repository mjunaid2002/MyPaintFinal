using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
namespace WebApplication1.ViewModels
{
    public class PurInvoiceVM
    {
         public PurMasterReturn purMasterreturn { get; set; }
         public BusinessUnit Settings { get; set; }
         public GatepassMaster gatepassdetail { get; set; }
        public IEnumerable<Salequery> lists { get; set; }
        public IEnumerable<VoucherMaster> vou_list { get; set; }
        public IEnumerable<VoucherMasterQuery> vou_lists { get; set; }
        public IEnumerable<dashboardquery> cus_aging { get; set; }
        public IEnumerable<dashboardquery> sup_bal { get; set; }
        public IEnumerable<dashboardquery> cus_bal { get; set; }
        public IEnumerable<Store> Store { get; set; }
        public IEnumerable<PurchaseQuery> top_supp_pur { get; set; }
        public IEnumerable<PurchaseQuery> purreturn_list_woc { get; set; }
        public IEnumerable<PurchaseQuery> list_new { get; set; }
        public IEnumerable<PurchaseQuery> rec { get; set; }
        public IEnumerable<PurchaseQuery> payables { get; set; }
        public IEnumerable<Gatepassdetailquery> gp_detail { get; set; }
        public IEnumerable<Salequery> supp_short { get; set; }
        public IEnumerable<Salequery> supp_re { get; set; }
        public IEnumerable<Salequery> cus_short { get; set; }
        public IEnumerable<Salequery> cus_re { get; set; }
        public IEnumerable<Salequery> bank_cash { get; set; }
        public IEnumerable<Salequery> top_customers_sale { get; set; }
        public IEnumerable<Salequery> salereturn_list_woc { get; set; }
        public IEnumerable<Salequery> sale_list_woc { get; set; }
        public IEnumerable<Brands> batch_no { get; set; }
        public IEnumerable<stockquery> pro_list_s { get; set; }
        public IEnumerable<PurFinQuery> pro_list_fin { get; set; }
        public IEnumerable<GatepassdetailQUery> pro_lists { get; set; }
        public IEnumerable<GatepassQuery> gate_list { get; set; }
        public IEnumerable<dashboardquery> sale_list { get; set; }
        public IEnumerable<dashboardquery> pur_dash { get; set; }
        public IEnumerable<VoucherImagesquery> img_list { get; set; }
        public IEnumerable<PurchaseQuery> list { get; set; }
        public IEnumerable<Supplier> sup_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<Product> pro_list1 { get; set; }
        public IEnumerable<Product> pro_list { get; set; }
        public IEnumerable<PurchaseDis> POpurchasedetail { get; set; }
        public IEnumerable<RawMaterialReportQuery> PORawMaterialReportQuery { get; set; }
        //public IEnumerable<PurMaster> pur_list { get; set; }
        public IEnumerable<PurDetailqueryss> p_detail { get; set; }
        public IEnumerable<PurDetailQuery> p_details { get; set; }
        public IEnumerable<PurchaseQuery> pur_list_woc { get; set; }
        public Supplier supplier { get; set; }
        public PurDetail PurDetail { get; set; }
        public Settings sett { get; set; }
        public GatepassMaster gatepassMaster { get; set; }
        public PurMaster PurMaster { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
    }
}