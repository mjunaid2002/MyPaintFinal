using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class ProductionOrder
    {
       
        public int Id { get; set; }
        public DateTime curdate { get; set; }
        public DateTime Promiseddate { get; set; }
        public decimal customerid { get; set; }
        public decimal regionid { get; set; }
        
    }
    public class ProductionOrderView
    {
       
        public int Id { get; set; }
        public DateTime curdate { get; set; }
        public DateTime Promiseddate { get; set; }
        public string customername { get; set; }
        public string regionname { get; set; }
        
    }
}