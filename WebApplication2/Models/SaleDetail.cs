using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class SaleDetail
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal  SP { get; set; }
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
}
