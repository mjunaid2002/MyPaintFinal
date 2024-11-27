using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Item_ledger
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Detail { get; set; }
        public int item_id { get; set; }
        public string b_unit { get; set; }
        public decimal sdetail { get; set; }
        public decimal pdetail { get; set; }
        public int srdetail { get; set; }
        public int prdetail { get; set; }
       
    }
}