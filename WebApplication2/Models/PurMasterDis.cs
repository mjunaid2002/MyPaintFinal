using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class PurMasterDis
    {
        public int Id { get; set; }
        public int invid { get; set; }
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
       
    }
}
