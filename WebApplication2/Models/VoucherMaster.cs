using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class VoucherMaster
    {
        public int ID { get; set; }
        public int invid { get; set; }
        public int? Account { get; set; }
        public int TID { get; set; }
        public string Date { get; set; }
        public decimal TDr { get; set; }
        public decimal TCr { get; set; }
        public string Remarks { get; set; }
        public string VType { get; set; }
        public string b_unit { get; set; }
        
    }
    public class VoucherMasterQuery
    {
        public int TransId { get; set; }
        public string TransDate { get; set; }
        public decimal Dr { get; set; }
    }
}
