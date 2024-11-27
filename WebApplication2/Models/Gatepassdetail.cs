using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Gatepassdetail
    {
        public int ID { get; set; }
        public int InvId { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string date { get; set; }
        public string item_iden { get; set; }
        public int Qty { get; set; }
        public int status { get; set; }
        public decimal SP { get; set; }
        public decimal p_box { get; set; }
    }
}
