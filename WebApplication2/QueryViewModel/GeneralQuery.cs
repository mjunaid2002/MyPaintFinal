using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.QueryViewModel
{
    public class LabMasterQuery
    {
        public decimal OrderId { get; set; }
        public decimal pid { get; set; }
        public bool ready { get; set; }
        public decimal categid { get; set; }
        public decimal maincateg { get; set; }
        public string ColorName { get; set; }
        public string req_status { get; set; }
        public decimal Drum { get; set; }
        public decimal Gallon { get; set; }
        public decimal Quarter { get; set; }
        public decimal Dubbi { get; set; }
        public decimal Stand_total_per { get; set; }
        public decimal Stand_total_cost { get; set; }
        public decimal Stand_total_weight { get; set; }
        public decimal ltrcoast { get; set; }
        public decimal viscosity { get; set; }
        public decimal whc { get; set; }
        public decimal kgcoast { get; set; }
        public string Quality { get; set; }
        //public decimal RegionId { get; set; }
        public string BatchNo { get; set; }
        public string ProductName { get; set; }
        public string ColorNo { get; set; }
        public string note { get; set; }
        public string ph { get; set; }
        public DateTime Date { get; set; }
        public DateTime datetime { get; set; }

    }
    public class DistributionMaster
    {
        public decimal invid { get; set; }
        public DateTime date { get; set; }
        public DateTime datetime { get; set; }
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public string note { get; set; }
        public decimal FromBranchId { get; set; }
        public decimal ToBranchId { get; set; }
        public decimal RegionId { get; set; }
        public string regionname { get; set; }
    }

    public class DistributionMasterDetail
    {
        public decimal sr { get; set; }
        public DateTime date { get; set; }
        public decimal pid { get; set; }
        public string pname { get; set; }
        public decimal qty { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public decimal invid { get; set; }
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public decimal FromBranchId { get; set; }
        public decimal ToBranchId { get; set; }
    }

    public class DistributionMasterDetail1
    {
        public decimal sr { get; set; }
        public decimal invid { get; set; }
        public DateTime date { get; set; }
        public decimal pid { get; set; }
        public string pname { get; set; }
        public decimal qty { get; set; }
        public string packing { get; set; }



    }

    public class FinishTransferM
    {
        public decimal invid { get; set; }
        public DateTime date { get; set; }

        public int Fromregion { get; set; }
        public int ToRegion { get; set; }
        public string note { get; set; }
        public string Fromname { get; set; }
        public string Toname { get; set; }




    }

    public class FinishTransferDetail
    {
        public decimal sr { get; set; }
        public decimal invid { get; set; }
        public DateTime date { get; set; }
        public decimal pid { get; set; }
        public string pname { get; set; }
        public string size { get; set; }
        public decimal qty { get; set; }



    }
    public class SaleReturnDetailQuery
    {
        public decimal disc_amount { get; set; }
        public decimal sr { get; set; }
        public decimal prid { get; set; }
        public decimal qty { get; set; }
        public decimal ltrkg { get; set; }
        public decimal sp { get; set; }
        public decimal total { get; set; }
        public decimal gst { get; set; }
        public decimal totalgst { get; set; }
        public decimal peritmdisc { get; set; }
        public decimal dsicval { get; set; }
        public decimal totalafterdisc { get; set; }
        public decimal wht { get; set; }
        public decimal ntotal { get; set; }
        public decimal CapDubbi { get; set; }
        public decimal CapQuarter { get; set; }
        public decimal CapGallon { get; set; }
        public decimal CapDrum { get; set; }
        public decimal OrderID { get; set; }
        public string prname { get; set; }
        public string Status { get; set; }
        public string packing { get; set; }
        public string catagoryname { get; set; }
    }
    public class SaleReturnQuery
    {
        public decimal OrderID { get; set; }
        public DateTime date { get; set; }
        public decimal cargoid { get; set; }
        public decimal cargo { get; set; }
        public decimal custid { get; set; }
        public string empname { get; set; }
        public decimal discount { get; set; }
        public decimal tax { get; set; }
        public decimal cargocharges { get; set; }
        public decimal total { get; set; }
        public decimal bal { get; set; }
        public decimal receivedamt { get; set; }
        public decimal afterdisc { get; set; }
        public decimal inctax { get; set; }
        public decimal gst { get; set; }
        public decimal wht { get; set; }
        public decimal ntotal { get; set; }
        public string title { get; set; }
        public string invno { get; set; }
        public decimal RegionId { get; set; }
        public string custname { get; set; }
        public string req_status { get; set; }
        public string note { get; set; }

    }
    public class OrderDetailQuery
    {
        public string packing { get; set; }
        public decimal disc_val { get; set; }
        public decimal disc_amount { get; set; }
        public decimal total_after { get; set; }
        public decimal sr { get; set; }
        public DateTime date { get; set; }
        public decimal prid { get; set; }
        public decimal sp { get; set; }
        public decimal ctn { get; set; }
        public decimal dsicval { get; set; }
        public decimal tax { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public decimal OrderID { get; set; }
        public decimal barcode { get; set; }
        public decimal pr { get; set; }
        public string prsn { get; set; }
        public string prname { get; set; }
        public string brandname { get; set; }
        public string catname { get; set; }

    }
    public class OrderMasterQuery
    {
        public decimal ntotal { get; set; }
        public decimal OrderID { get; set; }
        public decimal RegionId { get; set; }
        public DateTime date { get; set; }
        public decimal cargoid { get; set; }
        public decimal custid { get; set; }
        public string empname { get; set; }
        public string req_status { get; set; }
        public decimal discount { get; set; }
        public decimal tax { get; set; }
        public decimal net_total { get; set; }
        public decimal cargocharges { get; set; }
        public decimal total { get; set; }
        public decimal bal { get; set; }
        public decimal receivedamt { get; set; }
        public decimal token { get; set; }
        public string custname { get; set; }
        public string note { get; set; }

    }
    public class ProductPricing
    {
        public decimal CategoryID { get; set; }
        public decimal sp { get; set; }
        public decimal splower { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
        public string CategoryName { get; set; }
        public string empname { get; set; }
        public decimal discount { get; set; }
        public decimal tax { get; set; }
        public decimal net_total { get; set; }
        public decimal cargocharges { get; set; }
        public decimal total { get; set; }
        public decimal bal { get; set; }
        public decimal receivedamt { get; set; }
        public decimal token { get; set; }
        public string custname { get; set; }
        public string note { get; set; }
        public decimal RegionID { get; set; }
        public string RegionName { get; set; }


    }


    public class ProductFinishedRegion
    {
        public decimal sr { get; set; }
        public decimal pid { get; set; }

        public decimal regionid { get; set; }
        public string regionname { get; set; }
        public string ProductName { get; set; }

        public decimal dubi_o { get; set; }
        public decimal quarter_o { get; set; }
        public decimal gallon_o { get; set; }
        public decimal drum_o { get; set; }

        public decimal dubi_w { get; set; }
        public decimal quarter_w { get; set; }
        public decimal gallon_w { get; set; }
        public decimal drum_w { get; set; }

        public decimal dubi_min { get; set; }
        public decimal quarter_min { get; set; }
        public decimal gallon_min { get; set; }
        public decimal drum_min { get; set; }

        public decimal dubi_max { get; set; }
        public decimal quarter_max { get; set; }
        public decimal gallon_max { get; set; }
        public decimal drum_max { get; set; }

    }


    public class BatchFillingDetailQUery
    {
        public decimal Id { get; set; }
        public decimal sr { get; set; }
        public decimal ItemId { get; set; }
        public decimal qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public DateTime date { get; set; }
        public string ItemName { get; set; }
        public string type { get; set; }

    }
    public class BatchFillingBatches
    {
        public decimal Id { get; set; }
        public decimal sr { get; set; }
        public decimal filled { get; set; }
        public decimal waste { get; set; }
        public decimal cost { get; set; }
        public DateTime date { get; set; }
        public string batchn { get; set; }
    }
    public class BatchFillingMasterQUery
    {
        public decimal Id { get; set; }
        public string username { get; set; }
        public int userid { get; set; }
        public decimal RegionId { get; set; }
        public string status { get; set; }
        public decimal totalfillweight { get; set; }
        public decimal yeild { get; set; }
        public decimal batchespercost { get; set; }
        public decimal packingcost { get; set; }
        public decimal batchcost { get; set; }
        public decimal totalcost { get; set; }
        public string superviedby { get; set; }
        public string fillby2 { get; set; }
        public string filledby1 { get; set; }
        public decimal ColorID { get; set; }
        public decimal FilledDubbiQTY { get; set; }
        public decimal FilledDubbiWeight { get; set; }
        public decimal SampleDubbiQTY { get; set; }
        public decimal SampleDubbiWeight { get; set; }
        public decimal FilledQuarterQTY { get; set; }
        public decimal FilledQuarterWeight { get; set; }
        public decimal SampleQuarterQTY { get; set; }
        public decimal SampleQuarterWeight { get; set; }
        public decimal FilledGallonQTY { get; set; }
        public decimal FilledGallonWeight { get; set; }
        public decimal SampleGallonQTY { get; set; }
        public decimal SampleGallonWeight { get; set; }
        public decimal FilledDrumQTY { get; set; }
        public decimal FilledDrumWeight { get; set; }
        public decimal SampleDrumQTY { get; set; }
        public decimal SampleDrumWeight { get; set; }
        public DateTime Fillingdate { get; set; }
        public string ColorName { get; set; }
        public string supervisedby { get; set; }
        public string batchn { get; set; }
        public string filledby { get; set; }
        public decimal Dubi { get; set; }
        public decimal Quarter { get; set; }
        public decimal Gallon { get; set; }
        public decimal Drum { get; set; }
        public decimal Dubi_tot { get; set; }
        public decimal Quarter_tot { get; set; }
        public decimal Gallon_tot { get; set; }
        public decimal Drum_tot { get; set; }
        public decimal total_packing_dubbi { get; set; }
        public decimal total_packing_quarter { get; set; }
        public decimal total_packing_gallon { get; set; }
        public decimal total_packing_drum { get; set; }
    }
    public class BatchReceivingQuery
    {
        public decimal cost { get; set; }
        public decimal Id { get; set; }
        public decimal RegionId { get; set; }
        public decimal dubbi { get; set; }
        public decimal quarter { get; set; }
        public decimal Gallon { get; set; }
        public decimal Drum { get; set; }
        public decimal pid { get; set; }
        public decimal weight { get; set; }
        public decimal totalweight { get; set; }
        public decimal ItemId { get; set; }
        public DateTime date { get; set; }
        public DateTime daterec { get; set; }
        public string batch { get; set; }
        public string color { get; set; }
        public string status { get; set; }
    }
    public class BatchTransferQuery
    {
        public decimal Id { get; set; }
        public decimal pid { get; set; }
        public decimal weight { get; set; }
        public decimal totalweight { get; set; }
        public decimal ItemId { get; set; }
        public decimal RegionId { get; set; }
        public DateTime date { get; set; }
        public DateTime daterec { get; set; }
        public string batch { get; set; }
        public string color { get; set; }
        public string status { get; set; }
    }
    public class ProductIngrDetailQuery
    {
        public decimal Id { get; set; }
        public decimal ItemId { get; set; }
        public decimal weight { get; set; }
        public decimal percentage { get; set; }
        public decimal Rate { get; set; }
        public decimal Cost { get; set; }
        public decimal status { get; set; }
        public decimal OrderId { get; set; }
        public string ItemName { get; set; }
        public string name { get; set; }
        public int sr { get; set; }
        public DateTime Date { get; set; }
        public string Date1 { get; set; }
        public decimal ext1 { get; set; }
        public decimal ext2 { get; set; }
        public decimal ext3 { get; set; }
        public string ext4 { get; set; }
        public string ext5 { get; set; }
        public string ext6 { get; set; }
    }
    public class ProductIngrMasterQuery
    {
        public bool ready { get; set; }
        public decimal Total_cost { get; set; }
        public decimal estimated { get; set; }
        public decimal Stand_total_weight { get; set; }
        public decimal Stand_total_cost { get; set; }
        public decimal Stand_total_per { get; set; }
        public decimal Add_total_weight { get; set; }
        public decimal Add_total_cost { get; set; }
        public decimal Add_total_per { get; set; }
        public decimal OrderId { get; set; }
        public decimal Drum { get; set; }
        public decimal Gallon { get; set; }
        public decimal Quarter { get; set; }
        public decimal Dubbi { get; set; }
        public decimal kgcoast { get; set; }
        public decimal ltrcoast { get; set; }
        public decimal viscosity { get; set; }
        public decimal whc { get; set; }
        public decimal totalweight { get; set; }
        public decimal yield { get; set; }
        public decimal FilledDrum { get; set; }
        public decimal FilledGallon { get; set; }
        public decimal FilledQuarter { get; set; }
        public decimal FilledDubbi { get; set; }
        public decimal totalweightreceived { get; set; }
        public decimal SampleQTY { get; set; }
        public decimal SampleWeight { get; set; }
        public decimal transfered { get; set; }
        public decimal Prdwaste { get; set; }
        public decimal Pkgwaste { get; set; }
        public decimal pid { get; set; }
        public decimal categid { get; set; }
        public decimal maincateg { get; set; }
        //public decimal Ready { get; set; }
        public string ready2 { get; set; }
        public string machineoper1 { get; set; }
        public string note { get; set; }
        public string machineoper2 { get; set; }
        public string testby { get; set; }
        public string transferedby { get; set; }
        public string ColorNo { get; set; }
        public string Quality { get; set; }
        public string ColorName { get; set; }
        public string BatchNo { get; set; }
        public decimal RegionId { get; set; }
        public string ProductName { get; set; }
        public string RegionName { get; set; }
        public string ph { get; set; }
        public string fillingdate { get; set; }
        public DateTime fillingdate1 { get; set; }
        public string SampleType { get; set; }
        public string username { get; set; }
        public int userid { get; set; }
        public DateTime Date { get; set; }
        public DateTime datetime { get; set; }
    }


    public class SaleReportQuery
    {
        public decimal OrderID { get; set; }
        public decimal invid { get; set; }
        public decimal qty { get; set; }
        public decimal sp { get; set; }
        public decimal wht { get; set; }
        public decimal dsicval { get; set; }
        public decimal total_after { get; set; }
        public decimal ext1 { get; set; }
        public decimal ext2 { get; set; }
        public decimal ext3 { get; set; }
        public string custname { get; set; }
        public string prname { get; set; }
        public string packing { get; set; }
        public string ext4 { get; set; }
        public string ext5 { get; set; }
        public string ext6 { get; set; }
        public string maincataname { get; set; }
        public DateTime date { get; set; }

    }
    public class PurchaseReportQuery
    {
        public decimal invid { get; set; }
        public DateTime date { get; set; }
        public string supinv { get; set; }
        public string supname { get; set; }
        public string note { get; set; }
        public string builty { get; set; }
        public string cntrsupplier { get; set; }
        public string status { get; set; }
        public string ext1 { get; set; }
        public string ext2 { get; set; }
        public string pname { get; set; }
        public decimal supid { get; set; }
        public decimal cargid { get; set; }
        public decimal cargocharges { get; set; }
        public decimal othercharges { get; set; }
        public decimal discount { get; set; }
        public decimal paid { get; set; }
        public DateTime datetime { get; set; }
        public decimal cp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public decimal tax { get; set; }
        public decimal taxtotal { get; set; }
        public decimal ext3 { get; set; }
        public decimal ext4 { get; set; }
    }

    public class RawMaterialReportQuery
    {
        public decimal production { get; set; }
        public decimal transfer { get; set; }
        public decimal balance { get; set; }
        public decimal CategoryID { get; set; }
        public string ProductName { get; set; }
        public string BatchNo { get; set; }
        public string status { get; set; }
        public string CategoryName { get; set; }
        public string ColorName { get; set; }
        public string pname { get; set; }
        public decimal batch_filling { get; set; }
        public decimal ProductID { get; set; }
        public decimal totalweight { get; set; }
        public decimal kgcoast { get; set; }
        public decimal OrderId { get; set; }
        public decimal vattax { get; set; }
        public decimal cost { get; set; }
        public decimal AdjQTY { get; set; }
        public decimal QTYin { get; set; }
        public decimal QTYLabin { get; set; }
        public decimal QTYrtnIn { get; set; }
        public decimal qtyoutprd { get; set; }
        public decimal qtyout { get; set; }
        public decimal qtyoutlab { get; set; }
        public decimal QTYSaleOut { get; set; }
        public DateTime Date { get; set; }
        public decimal qtyoutwast { get; set; }
        public decimal Qtybal { get; set; }
        public decimal ext1 { get; set; }
        public decimal ext2 { get; set; }
        public decimal ext3 { get; set; }
        public string ext5 { get; set; }
        public string ext6 { get; set; }
        public string ext7 { get; set; }
        public string QTYStatus { get; set; }
        public decimal POQty { get; set; }
        public decimal MaxLevelRaw { get; set; }
        public string ProductType { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal ReorderLevel { get; set; }
        public string SupplierName { get; set; }
        public string QualityCategoriesitem { get; set; }
    }
    public class tbl_RawMaterialReportQuery
    {
        public decimal sr { get; set; }
        public decimal ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal opening { get; set; }
        public decimal PurchaseQTY { get; set; }
        public decimal LabQTY { get; set; }
        public decimal SalesStockReturn { get; set; }
        public decimal BatchQty { get; set; }
        public decimal LabBatchQty { get; set; }
        public decimal SaleQTY { get; set; }
        public decimal PurchaseRtn { get; set; }
        public decimal damage { get; set; }
        public decimal cost { get; set; }
    }
    public class tbl_Product
    {
        public decimal ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal vattax { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CustomerDiscount
    {
        public decimal id { get; set; }
        public decimal customerid { get; set; }
        public decimal CategoryID { get; set; }
        public decimal disc { get; set; }
        public string customername { get; set; }
        public string CategoryName { get; set; }
    }
    public class samplem
    {
        public decimal invid { get; set; }
        public int regionid { get; set; }
        public decimal total { get; set; }
        public string note { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        [NotMapped]
        public string regionname { get; set; }

    }
    public class PoMaster
    {
        public decimal invid { get; set; }
        public DateTime date { get; set; }
        public DateTime datetime { get; set; }
        public decimal gtotal { get; set; }
        public decimal discount { get; set; }
        public decimal total { get; set; }
        public decimal tax { get; set; }
        public decimal tax_amount { get; set; }
        public decimal supid { get; set; }
        public string supname { get; set; }
        public string status { get; set; }
        public string req_status { get; set; }
        public string builty { get; set; }
        public string note { get; set; }
    }
    public class sampledetail
    {
        public decimal sr { get; set; }
        public string packing { get; set; }
        public string pname { get; set; }
        public decimal pid { get; set; }
        public decimal cp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public decimal invid { get; set; }
        public DateTime date { get; set; }

    }

    public class PoDetail
    {
        public decimal partywht { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
        public decimal sp { get; set; }
        public decimal splower { get; set; }
        public decimal disc { get; set; }
        public decimal op { get; set; }
        public decimal qty2 { get; set; }
        public decimal qtyreturn { get; set; }
        public decimal qtybatchfilling { get; set; }
        public decimal qtylab { get; set; }
        public decimal qty1 { get; set; }
        public decimal sr { get; set; }
        public string pname { get; set; }
        public decimal pid { get; set; }
        public decimal cp { get; set; }
        public decimal cp2 { get; set; }
        public decimal qty { get; set; }
        public decimal inctax { get; set; }
        public decimal total { get; set; }
        public decimal tax { get; set; }
        public decimal invid { get; set; }
        public string tax_per { get; set; }
        public string psrn { get; set; }


    }
    public class Customers
    {
        public decimal customerid { get; set; }
        public decimal partywht { get; set; }
        public decimal Cityid { get; set; }
        public decimal accno { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NTN { get; set; }
        public string Phone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }


    }
    public class Cityquery
    {
        public decimal id { get; set; }
        public string name { get; set; }
        public string Address { get; set; }

    }
    public class MianCategories
    {
        public decimal MainCategoryID { get; set; }
        public string MainCategoryName { get; set; }
    }
    public class QualityCategories
    {
        public decimal ID { get; set; }
        public string CategoryName { get; set; }
    }
    public class Categories
    {
        public decimal CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string RawProductCheck { get; set; }
        public string IsPacking { get; set; }
        public string Description { get; set; }
        public decimal MainCategoryID { get; set; }
    }
    public class Product_Type
    {
        public decimal Id { get; set; }
        public string ProductType { get; set; }
    }
    public class cargo
    {
        public decimal id { get; set; }
        public string name { get; set; }
        public string nameurdu { get; set; }
        public string address { get; set; }
        public string addressurdu { get; set; }
        public string cityid { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
    }
    public class Products
    {
        public decimal rem { get; set; }
        public decimal qty1 { get; set; }
        public decimal qty { get; set; }
        public decimal cp { get; set; }
        public decimal OpeningDubbi { get; set; }
        public decimal OpeningQuarter { get; set; }
        public decimal OpeningGallon { get; set; }
        public decimal OpeningDrum { get; set; }
        public decimal ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal MaxLevelRaw { get; set; }
        public string ProductType { get; set; }
        public string CategoryName { get; set; }
        public decimal vattax { get; set; }
        public decimal BrandID { get; set; }
        public decimal CategoryID { get; set; }
        public string quality { get; set; }
        public string BatchNo { get; set; }
        public string ProductName { get; set; }
        public string desc { get; set; }
        public string Active { get; set; }
        public decimal barcode { get; set; }
        public decimal sp { get; set; }
        public decimal itmdisc { get; set; }
        public decimal splower { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
        public decimal CapDubbi { get; set; }
        public decimal CapQuarter { get; set; }
        public decimal CapGallon { get; set; }
        public decimal iduser { get; set; }
        public decimal CapDrum { get; set; }
        public decimal ReorderDubbi { get; set; }
        public decimal ReorderQuarter { get; set; }
        public decimal ReorderGallon { get; set; }
        public decimal ReorderDrum { get; set; }
        public decimal MaxDubbi { get; set; }
        public decimal MaxQuarter { get; set; }
        public decimal MaxGallon { get; set; }
        public decimal MaxDrum { get; set; }
        public decimal totalweight { get; set; }
    }
    public class Region
    {
        public decimal id { get; set; }
        public string name { get; set; }
    }
    public class ProductPricingRegion
    {
        public decimal Id { get; set; }
        public decimal CategoryID { get; set; }
        public decimal RegionID { get; set; }
        public decimal dubi { get; set; }
        public decimal quarter { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
    }
    public class Supplierquery
    {
        public decimal ten { get; set; }
        public int op { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Ntn { get; set; }
        public string BankDetail { get; set; }
        public string CompanyName { get; set; }
        public int AccountNo { get; set; }
        public string Telephone { get; set; }
        public string Gst { get; set; }
        public string Provinces { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string CNIC { get; set; }
        public decimal BusinessUnit { get; set; }
        public string Description { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal PromptPaymentDiscount { get; set; }
        public string PaymentConditions { get; set; }
        public string Daysofpayment { get; set; }
        public decimal CreditLimit { get; set; }

    }

    public class CustomerQUery
    {
        public decimal ID { get; set; }
        //  public int Count { get; set; }
        public decimal ten { get; set; }
        public int op { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Ntn { get; set; }
        public string BankDetail { get; set; }
        public string CompanyName { get; set; }
        public decimal AccountNo { get; set; }
        public string Telephone { get; set; }
        public string Gst { get; set; }
        public string Provinces { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string CNIC { get; set; }
        public string BusinessUnit { get; set; }
        public string Description { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal PromptPaymentDiscount { get; set; }
        public string PaymentConditions { get; set; }
        public string Daysofpayment { get; set; }
        public decimal CreditLimit { get; set; }
        public int Dsr { get; set; }
        public int longi { get; set; }
        public int lati { get; set; }
        public bool comp_check { get; set; }
        public bool cus_check { get; set; }

    }

    public class SaleDetailQuery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal SP1 { get; set; }
        public decimal SP { get; set; }
        public int Ctn { get; set; }
        public decimal PiecesBox { get; set; }
        public decimal Qty { get; set; }
        public decimal NetTotal { get; set; }
        public decimal tax_item { get; set; }
        public decimal tax_amount { get; set; }
        public decimal NetTotal_wtax { get; set; }
        public string Date { get; set; }
        public string InvType { get; set; }
        public string d_no { get; set; }
        public string t_drum { get; set; }
        public string e_drum { get; set; }
        public string idnit { get; set; }
        public string packing { get; set; }

    }
    public class QuotationQuery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public int Ctn { get; set; }
        public string Munit { get; set; }
        public string b_unit { get; set; }
        public string ItemName { get; set; }
        public string Date { get; set; }
        public decimal SP { get; set; }
        public decimal Qty { get; set; }
        public decimal NetTotal { get; set; }
        public int PiecesBox { get; set; }
    }
    public class PurDetailQuery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int party { get; set; }
        public int ItemID { get; set; }
        public string Suppname { get; set; }
        public string ItemName { get; set; }
        public decimal SP { get; set; }
        public int Qty { get; set; }
        public decimal NetTotal { get; set; }
        public decimal PiecesBox { get; set; }
    }
    public class PurFinQuery
    {
        public int qty { get; set; }
        public int p_id { get; set; }
        public string item_name { get; set; }
        public decimal sp { get; set; }
        public decimal sp_euro { get; set; }
        public decimal tot_ne_euro { get; set; }
        public decimal total { get; set; }
        public decimal PiecesBox { get; set; }
        public decimal tot_leng { get; set; }
    }
    public class dashboardquery
    {
        public decimal total { get; set; }
        public int op { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public int days { get; set; }
    }
    public class ItemLedgerquert
    {
        public int ItemID { get; set; }
        public decimal Qty { get; set; }
        public string Date { get; set; }
    }
    public class ItemLedgerquertyyyy
    {
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public string Date { get; set; }
    }
    public class Salequery
    {
        public int days { get; set; }
        public int ID { get; set; }
        public int InvID { get; set; }
        public int CustomerId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string InvType { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal tax_amount { get; set; }
    }
    public class GatepassdetailQUery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string date { get; set; }
        public string item_iden { get; set; }
        public string Munit { get; set; }
        public int Qty { get; set; }
        public int status { get; set; }
        public decimal SP { get; set; }
        public decimal p_box { get; set; }
    }
    public class PurchaseQuery
    {
        public int op { get; set; }
        public int days { get; set; }
        public int ID { get; set; }
        public int InvID { get; set; }
        public int SupplierId { get; set; }
        public int DsrId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Transdate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Bal { get; set; }
        public decimal yelid { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public int Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public string InvType { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal tax_amount { get; set; }
        public string BatchNo { get; set; }
    }
    public class GatepassQuery
    {
        public int ID { get; set; }
        public int InvID { get; set; }
        public decimal supid { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string SupplierName { get; set; }
        public string Date { get; set; }
        public int b_unit { get; set; }
        public int status { get; set; }
    }
    public class PurchaseDis
    {
        public decimal Id { get; set; }
        public decimal invid { get; set; }
        public decimal itemid { get; set; }
        public string date { get; set; }
        public decimal supid { get; set; }
        public string cargoname { get; set; }
        public decimal cargocharges { get; set; }
        public decimal othercharges { get; set; }
        public decimal discount { get; set; }
        public decimal total { get; set; }
        public decimal GrandTotal { get; set; }
        // public decimal bal { get; set; }
        public decimal PhoneNo { get; set; }
        public string RateCode { get; set; }
        // public string Decimal { get; set; }
        public string Address { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }


    }
    public class SaleDis
    {

        public int ID { get; set; }
        public int InvId { get; set; }
        public decimal empid { get; set; }
        public string cargoname { get; set; }
        public int custid { get; set; }
        public string date { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public decimal discount { get; set; }
        public decimal cargocharges { get; set; }
        public decimal total { get; set; }
        // public decimal bal { get; set; }
        public decimal rec { get; set; }
        public decimal RateCode { get; set; }
        public decimal GrandTotal { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public int prid { get; set; }
        public string prname { get; set; }
        public decimal sp { get; set; }
        public decimal qty { get; set; }
        public decimal ctn { get; set; }
        public decimal retailprice { get; set; }
        public decimal tradeprice { get; set; }
        public decimal disc { get; set; }
        public string pname { get; set; }
        public decimal cp { get; set; }
        public decimal disctotal { get; set; }
        public decimal FedTax { get; set; }
        public decimal GrossValue { get; set; }
        public decimal tovalue { get; set; }
        public decimal NVEF { get; set; }
        public int specialdisc { get; set; }
        public decimal totalweight { get; set; }
        public string Barcode { get; set; }

    }
    public class SaleMasterRep
    {

        public int ID { get; set; }
        public int InvID { get; set; }
        public int CustomerId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string InvType { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public int InvId { get; set; }
        public decimal empid { get; set; }
        public string cargoname { get; set; }
        public int custid { get; set; }
        public string date { get; set; }
        public string PhoneNumber { get; set; }
        public decimal discount { get; set; }
        public decimal cargocharges { get; set; }
        public decimal total { get; set; }
        // public decimal bal { get; set; }
        public decimal rec { get; set; }
        public decimal RateCode { get; set; }

    }
    public class SaleDetailRep
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int SP { get; set; }
        public int Ctn { get; set; }
        public int PiecesBox { get; set; }
        public int Qty { get; set; }
        public int NetTotal { get; set; }
        public string Date { get; set; }
        public int prid { get; set; }
        public string prname { get; set; }
        public decimal sp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public decimal RateCode { get; set; }
        public decimal ctn { get; set; }
        public decimal retailprice { get; set; }
        public decimal tradeprice { get; set; }
        public decimal disc { get; set; }
        public string pname { get; set; }
        public decimal cp { get; set; }
        public decimal disctotal { get; set; }
        public decimal FedTax { get; set; }
        public decimal GrossValue { get; set; }
        public decimal tovalue { get; set; }
        public decimal NVEF { get; set; }
        public int specialdisc { get; set; }
        public decimal totalweight { get; set; }
        public string Barcode { get; set; }

    }
    public class PurchaseMasterRep
    {
        public int ID { get; set; }
        public int InvID { get; set; }
        public int SupplierId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public string InvType { get; set; }
        public int invid { get; set; }
        public string date { get; set; }
        public decimal supid { get; set; }
        public string cargoname { get; set; }
        public decimal cargocharges { get; set; }
        public decimal othercharges { get; set; }
        public decimal discount { get; set; }
        public decimal total { get; set; }
        // public decimal bal { get; set; }
        public decimal PhoneNo { get; set; }
        public string RateCode { get; set; }
        // public string Decimal { get; set; }

    }
    public class PurchaseDetailRep
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal SP { get; set; }
        public int Ctn { get; set; }
        public int PiecesBox { get; set; }
        public int Qty { get; set; }
        public decimal NetTotal { get; set; }
        public string Date { get; set; }
        public decimal sr { get; set; }
        public int pid { get; set; }
        public decimal p_box { get; set; }
        public string pname { get; set; }
        public decimal cp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public int invid { get; set; }
        public decimal ctn { get; set; }
        public decimal tradeprice { get; set; }
        public decimal retailprice { get; set; }
        public decimal RateCode { get; set; }
        public decimal disc { get; set; }


    }
    public class PurchaseDetailDisRep
    {
        public int Id { get; set; }
        public decimal sr { get; set; }
        public int pid { get; set; }
        public decimal p_box { get; set; }
        public string pname { get; set; }
        public decimal cp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public int invid { get; set; }
        public decimal ctn { get; set; }
        public decimal tradeprice { get; set; }
        public decimal retailprice { get; set; }
        public decimal RateCode { get; set; }
        public decimal disc { get; set; }
        public string Date { get; set; }
        public decimal dis_amount { get; set; }
    }
    public class AcMain
    {
        public int ID { get; set; }
        public int count { get; set; }
        public int account_no { get; set; }
        public int a_head { get; set; }
        public string Headname { get; set; }
        public string a_title { get; set; }
        public string secondlevel { get; set; }
        public string FirstLevelAccount { get; set; }
        public string ThirdLevel { get; set; }
        public string ac_h_name { get; set; }
        public string AccountTitleName { get; set; }
        public int a_main_id { get; set; }
        public int AccountNo { get; set; }

    }
    public class SaleDetailqueryss
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string Munit { get; set; }
        public string ItemName { get; set; }
        public decimal SP { get; set; }
        public int Ctn { get; set; }
        public int PiecesBox { get; set; }
        public decimal Qty { get; set; }
        public decimal NetTotal { get; set; }
        public decimal tax_item { get; set; }
        public decimal tax_amount { get; set; }
        public decimal NetTotal_wtax { get; set; }
        public string Date { get; set; }
        public string InvType { get; set; }
        public string d_no { get; set; }
        public string t_drum { get; set; }
        public string e_drum { get; set; }
        public string idnit { get; set; }
        public string packing { get; set; }

    }
    public class PurDetailqueryss
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string Munit { get; set; }
        public string ItemName { get; set; }
        public decimal SP { get; set; }
        public int Ctn { get; set; }
        public decimal PiecesBox { get; set; }
        public decimal Qty { get; set; }
        public int g_passid { get; set; }
        public decimal NetTotal { get; set; }
        public string Date { get; set; }
        public string BatchNumber { get; set; }
        public string Expriy { get; set; }
        public string InvType { get; set; }
        public decimal tax_item { get; set; }
        public decimal tax_amount { get; set; }
        public decimal NetTotal_wtax { get; set; }
    }
    public class Gatepassdetailquery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Munit { get; set; }
        public string date { get; set; }
        public string item_iden { get; set; }
        public int Qty { get; set; }
        public int status { get; set; }
        public decimal SP { get; set; }
        public decimal p_box { get; set; }
    }
    public class OpeningBalanceQuery
    {
        public int Id { get; set; }
        public int AccountNo { get; set; }
        public int Cr { get; set; }
        public int Dr { get; set; }
        public string AccountTitleName { get; set; }
        public DateTime date { get; set; }
        public string narration { get; set; }
        public string Name { get; set; }

    }
    public class ProductQuery
    {
        public int ID { get; set; }
        public string CatID { get; set; }
        public string BrandID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
        public decimal bal { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePrice_euro { get; set; }
        public decimal LeastPrice { get; set; }
        public decimal PiecesBox { get; set; }
        public int OpeningStock { get; set; }
        public decimal ReOrder { get; set; }
        public string StockDate { get; set; }
        public string Barcode { get; set; }
        public string Wat { get; set; }
        public string type { get; set; }
        public string Brand { get; set; }
        public string MUnit { get; set; }
        public decimal ShelfNumber { get; set; }
        public decimal BusinessUnit { get; set; }
    }
    public class stockquery
    {
        public string item_name { get; set; }
        public int OpeningStock { get; set; }
        public decimal QTYIN { get; set; }
        public decimal QTYOUT { get; set; }
        public decimal openingnopur { get; set; }
        public decimal openingno { get; set; }

    }
    public class saledetaildisRep
    {
        public int ID { get; set; }
        public string pname { get; set; }
        public decimal retailprice { get; set; }
        //   public decimal invid { get; set; }
        public decimal cp { get; set; }
        public decimal ctn { get; set; }
        public decimal total { get; set; }
        public decimal disctotal { get; set; }
        public decimal FedTax { get; set; }
        public decimal disc { get; set; }
        public decimal qty { get; set; }
        public decimal GrossValue { get; set; }
        public decimal tovalue { get; set; }
        public decimal NVEF { get; set; }
        public int specialdisc { get; set; }
        public decimal totalweight { get; set; }
        public string Barcode { get; set; }
        public decimal ratecode { get; set; }
        public int pid { get; set; }
    }
    public class AttendanceQuery
    {
        public decimal Id { get; set; }
        public decimal EmpId { get; set; }
        public DateTime checkin_datetime { get; set; }
        public DateTime checkout_datetime { get; set; }
        public decimal Status { get; set; }
        public string EmpName { get; set; }
        public TimeSpan STIN { get; set; }
        public TimeSpan STOUT { get; set; }
        public DateTime Attdate { get; set; }
    }
    public class SaleItemDetailsQuery
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int SP { get; set; }
        public int SP1 { get; set; }
        public int Ctn { get; set; }
        public int PiecesBox1 { get; set; }
        public int PiecesBox { get; set; }
        public int Qty { get; set; }
        public decimal NetTotal { get; set; }
        public string Date { get; set; }
    }
    public class VoucherQuery
    {
        public int Id { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public string AccountName { get; set; }
        public string Narr { get; set; }
        public string Vtype { get; set; }

    }

    public class TrailQuery
    {
        public int AccountNo { get; set; }
        public int AccountHeadId { get; set; }
        public string AccountTitleName { get; set; }
        public int dr { get; set; }
        public int cr { get; set; }
        public decimal ccr { get; set; }
        public decimal cdr { get; set; }
        public decimal odr { get; set; }
        public decimal ocr { get; set; }
        public decimal closing_bal { get; set; }
        public decimal saledetaildistotal { get; set; }
        public decimal trans_balance { get; set; }
        public decimal saledetailtotal { get; set; }
        public string ItemName { get; set; }
        public string prname { get; set; }
    }
    public class VoucherImagesquery
    {
        public int Id { get; set; }
        public string Images { get; set; }
        public int Tid { get; set; }
        public string VType { get; set; }

    }
    public class FinishedGoodsQuery
    {
        public string ProductName { get; set; }
        public decimal OpeningDubbi { get; set; }
        public decimal received { get; set; }
        public decimal Filled { get; set; }
        public decimal OpeningQuarter { get; set; }
        public decimal OpeningGallon { get; set; }
        public decimal OpeningDrum { get; set; }
        public decimal DubbiIN { get; set; }
        public decimal QuarterIN { get; set; }
        public decimal GallonIN { get; set; }
        public decimal DrumIN { get; set; }
        public decimal DubbiIN1 { get; set; }
        public decimal Quarter1 { get; set; }
        public decimal DrumIN1 { get; set; }
        public decimal GallonIN1 { get; set; }
        public decimal DubbiOUT1 { get; set; }
        public decimal QuarterOUT1 { get; set; }
        public decimal DrumOUT1 { get; set; }
        public decimal GallonOUT1 { get; set; }
        public decimal DubbiOUT2 { get; set; }
        public decimal QuarterOUT2 { get; set; }
        public decimal DrumOUT2 { get; set; }
        public decimal GallonOUT2 { get; set; }
        public decimal DubbiOUT3 { get; set; }
        public decimal QuarterOUT3 { get; set; }
        public decimal DrumOUT3 { get; set; }
        public decimal GallonOUT3 { get; set; }
    }
    public class FinishedGoodsQuery2
    {
        public decimal ProductID { get; set; }
        public string ProductName { get; set; }

        public decimal StkOpenDubbi { get; set; }
        public decimal StkOpenQuarter { get; set; }
        public decimal StkOpenGallon { get; set; }
        public decimal StkOpenDrum { get; set; }

        public decimal StkInDubbi { get; set; }
        public decimal StkInQuarter { get; set; }
        public decimal StkInGallon { get; set; }
        public decimal StkInDrum { get; set; }

        public decimal StkOutDubbi { get; set; }
        public decimal StkOutQuarter { get; set; }
        public decimal StkOutGallon { get; set; }
        public decimal StkOutDrum { get; set; }

        public decimal StkDubbi { get; set; }
        public decimal StkQuarter { get; set; }
        public decimal StkGallon { get; set; }
        public decimal StkDrum { get; set; }

        public decimal ReorderDubbi { get; set; }
        public decimal ReorderQuarter { get; set; }
        public decimal ReorderGallon { get; set; }
        public decimal ReorderDrum { get; set; }

        public decimal MaxDubbi { get; set; }
        public decimal MaxQuarter { get; set; }
        public decimal MaxGallon { get; set; }
        public decimal MaxDrum { get; set; }

        public decimal PODubbi { get; set; }
        public decimal POQuarter { get; set; }
        public decimal POGallon { get; set; }
        public decimal PODrum { get; set; }

        public string Category { get; set; }
        public string Product { get; set; }
        public string Color { get; set; }
    }

    public class stockvalue
    {
        public decimal ColorID { get; set; }
        public string ColorName { get; set; }

        public decimal dubbi { get; set; }
        public decimal Quarter { get; set; }
        public decimal Gallon { get; set; }
        public decimal Drum { get; set; }
    }

    public class ItemLedger_Raw
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal StockIn { get; set; }
        public decimal StockOut { get; set; }
    }
    public class ItemLedger_finished
    {
        public int SizeID { get; set; }
        public int ID { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal DabbiIN { get; set; }
        public decimal QuarterIN { get; set; }
        public decimal GallonIN { get; set; }
        public decimal DrumIN { get; set; }
        public decimal DabbiOUT { get; set; }
        public decimal QuarterOUT { get; set; }
        public decimal GallonOUT { get; set; }
        public decimal DrumOUT { get; set; }
        public decimal DabbiBAL { get; set; }
        public decimal QuarterBAL { get; set; }
        public decimal GallonBAL { get; set; }
        public decimal DrumBAL { get; set; }

    }
    public class LedgerQuery
    {
        public int Id { get; set; }
        public int AccountNo { get; set; }
        public int AccMain { get; set; }
        public int AccountHeadId { get; set; }
        public int Rinvid { get; set; }
        public int SecondLevel { get; set; }
        public string AccountTitleName { get; set; }
        public string AccountType { get; set; }
        //public int cr { get; set; }
        //public int dr { get; set; }
        public int TransId { get; set; }
        public string facc { get; set; }
        public string TransDes { get; set; }
        public string TransDetail { get; set; }
        public string TransDate { get; set; }
        public int AccountId { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public int InvId { get; set; }
        public string Vtype { get; set; }
    }

    public class RawReportCalculationQuery
    {
        public decimal Sr { get; set; }
        public decimal ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal opening { get; set; }
        public decimal PurchaseQTY { get; set; }
        public decimal LabQTY { get; set; }
        public decimal SalesStockReturn { get; set; }
        public decimal BatchQty { get; set; }
        public decimal LabBatchQty { get; set; }
        public decimal SaleQTY { get; set; }
        public decimal PurchaseRtn { get; set; }
        public decimal damage { get; set; }
        public decimal cost { get; set; }
    }

    public class UserLogHistory
    {
        public decimal Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime datetime { get; set; }
        public string ActivityForm { get; set; }
        public string ActionTaken { get; set; }
        public string CurrentUser { get; set; }
        public decimal BillNo { get; set; }
        public string detail { get; set; }
    }
}