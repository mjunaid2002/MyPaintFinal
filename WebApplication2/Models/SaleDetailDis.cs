using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class SaleDetailDis
    {
        public int Id { get; set; }
        public int prid { get; set; }
        public string prname { get; set; }
        public decimal sp { get; set; }
        public decimal qty { get; set; }
        public decimal total { get; set; }
        public int InvId { get; set; }
        public decimal RateCode { get; set; }
        public decimal ctn { get; set; }
        public decimal retailprice { get; set; }
        public decimal tradeprice { get; set; }
        public decimal disc { get; set; }
        public string Date { get; set; }
        public string dis_amount { get; set; }
    }
}
