using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ProductionOrderDetail
    {
        public int ID { get; set; }
        public int pid { get; set; }
        public string pname { get; set; }
        public decimal dubbi { get; set; }
        public decimal quarter { get; set; }
        public decimal gallon { get; set; }
        public decimal drum { get; set; }
        public decimal totalqty { get; set; }
        
        public decimal dubbi_c { get; set; }
        public decimal quarter_c { get; set; }
        public decimal gallon_c { get; set; }
        public decimal drum_c { get; set; }
        public decimal totalqty_c { get; set; }
        public decimal totalqty_r { get; set; }
        public decimal totalqty_m { get; set; }
    }
}