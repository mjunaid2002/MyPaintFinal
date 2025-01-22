using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
namespace WebApplication1.ViewModels
{
    public class SaleInvVM
    {
        public IEnumerable<Scheme> Scheme_list { get; set; }
        public OpeningStockBranch OpeningStockBranch { get; set; }
        public IEnumerable<OpeningStockBranch> OpeningStockBranchList { get; set; }
        public IEnumerable<Region> Region_list { get; set; }
        public IEnumerable<Branch> Branch_list { get; set; }
        public IEnumerable<ProductionOrder> ProductionOrderlist { get; set; }
        public IEnumerable<FinishTransferM> FinishTransferlist { get; set; }
        public IEnumerable<FinishTransferDetail> FinishTransferDetailList { get; set; }
        public BranchTransferM BranchTransfer { get; set; }
        public IEnumerable<BranchTransferM> BranchTransferList { get; set; }
        public BranchTransferDetail BranchTransferDetail { get; set; }
        public IEnumerable<BranchTransferDetail> BranchTransferDetailList { get; set; }
        public ProductionOrder ProductionOrder { get; set; }
        public FinishTransferM FinishTransfer { get; set; }
        public FinishTransferDetail FinishTransferDetail { get; set; }
        public ProductionOrderDetail ProductionOrderDetail{ get; set; }
        public IEnumerable<ProductionOrderDetail> ProductionOrderDetaillist { get; set; }
        public DistributionMaster distribution { get; set; }
        public IEnumerable<DistributionMaster> distributionList { get; set; }
        public DistributionMasterDetail distributionDetail { get; set; }
        public IEnumerable<DistributionMasterDetail> distributionDetailList { get; set; }
        public IEnumerable<DistributionMasterDetail1> distributionDetail1list { get; set; }
        public IEnumerable<sampledetail> sampledetail { get; set; }
         public IEnumerable<BatchFillingBatches> BatchFillingBatches { get; set; }
         public IEnumerable<BatchFillingDetailQUery> batchfillingdetail { get; set; }
         public IEnumerable<SaleReturnDetailQuery> saleReturnQueryDetail { get; set; }
         public IEnumerable<OrderDetailQuery> OrderDetailQuery { get; set; }
         public IEnumerable<Categories> cat_list { get; set; }
         public IEnumerable<MianCategories> quality { get; set; }
         public IEnumerable<Categories> product { get; set; }
         public IEnumerable<Products> color1 { get; set; }
         public IEnumerable<Products> color { get; set; }
         public IEnumerable<PoDetail> poDetail { get; set; }
         public IEnumerable<Customers> Cus_list { get; set; }
         public IEnumerable<cargo> Cargo_list { get; set; }
         public IEnumerable<Products> pro_listsss { get; set; }
         public IEnumerable<Store> Store { get; set; }
         public IEnumerable<Salequery> list { get; set; }
         public IEnumerable<SaleDetailQuery> delivery_list { get; set; }
         public IEnumerable<QuotationQuery> q_detail { get; set; }
         public IEnumerable<stockquery> pro_list_s { get; set; }
         public IEnumerable<Gatepassdetail> g_detail { get; set; }
         public IEnumerable<Supplier> sup_list { get; set; }
         public IEnumerable<Customer> cus_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<Product> pro_list { get; set; }
        public IEnumerable<SaleDetailqueryss> s_detail { get; set; }
        public IEnumerable<Salequery> sale_list_woc { get; set; }
        public IEnumerable<ProductIngrDetailQuery> ProductIngrDetailQuery { get; set; }
        public IEnumerable<BatchTransferQuery> batchTransferQuerylist { get; set; }
        public CustomerDiscount customerDiscount { get; set; }
        public LabMasterQuery labMasterQuery { get; set; }
        public OrderMasterQuery orderMasterQuery { get; set; }
        public SaleReturnQuery saleReturnQuery { get; set; }
        public samplem samplem { get; set; }
        public PoMaster poMaster { get; set; }
        public Settings sett { get; set; }
        public BusinessUnit Settings { get; set; }
        public Customer customer { get; set; }
        public SaleMasterReturn saleMasterReturn { get; set; }
        public SaleDetail SaleDetail { get; set; }
        public GatepassMaster gatepassMaster { get; set; }
        public SaleMaster SaleMaster { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
        public ProductIngrMasterQuery productIngrMasterQuery { get; set; }
        public IEnumerable<ProductIngrMasterQuery> ProductIngrMasterQuerylist { get; set; }
        public BatchTransferQuery batchTransferQuery { get; set; }
        public BatchReceivingQuery batchReceivingQuery { get; set; }
        public BatchFillingMasterQUery batchFillingMasterQUery  { get; set; }
        public string InvNo  { get; set; }
        public decimal rawtotal  { get; set; }
        
    }
}