using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class SaleMasterDis
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
        public decimal total    { get; set; }
       // public decimal bal { get; set; }
        public decimal rec { get; set; }
        public decimal RateCode { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
