using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string CatID { get; set; }
        public string BrandID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
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
    public class ProductCapShow
    {
        public decimal ProductID { get; set; }
        public int ProductID1 { get; set; }
        public string ProductName { get; set; }
        public decimal CapDubbi { get; set; }
        public decimal CapQuarter { get; set; }
        public decimal CapGallon { get; set; }
        public decimal CapDrum { get; set; }
        public decimal qty { get; set; }
        
        
        public decimal dubbi_o { get; set; }
        public decimal quarter_o { get; set; }
        public decimal gallon_o { get; set; }
        public decimal drum_o { get; set; }
        public decimal qty_o { get; set; }
        
        public decimal dubbi_c { get; set; }
        public decimal quarter_c { get; set; }
        public decimal gallon_c { get; set; }
        public decimal drum_c { get; set; }
        public decimal qty_c { get; set; }
        public decimal qty_r { get; set; }
        public decimal qty_m { get; set; }
        
        public decimal dubbi { get; set; }
        public decimal quarter { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
        public decimal Qty1 { get; set; }
        

    }
}
