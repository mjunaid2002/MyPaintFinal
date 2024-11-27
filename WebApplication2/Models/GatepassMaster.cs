using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class GatepassMaster
    {
        public int ID { get; set; }
        public int InvID { get; set; }
        public decimal supid { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Date { get; set; }
        public int b_unit { get; set; }
        public int DsrId { get; set; }
        public int Store { get; set; }
        public string CargoName { get; set; }
    }
}
