using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class SaleMaster
    {  
        public int Store { get; set; }
        public int ID { get; set; }
        public int InvID { get; set; }
        public int sInvID { get; set; }
        public int CustomerId { get; set; }
        public int DsrId { get; set; }
        public string b_unit { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string InvType { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal NetTotal_Wtax { get; set; }
        public decimal tax_amount { get; set; }
        public decimal US { get; set; }
        public decimal Euro { get; set; }
        public decimal Austrialan { get; set; }
        public decimal Canadian { get; set; }
    }
    public class SaleMasterReturn
    {
        public int Store { get; set; }
        public int ID { get; set; }
        public int InvID { get; set; }
        public int sInvID { get; set; }
        public int CustomerId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public string InvType { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Rtotal { get; set; }
        public decimal BTotal { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal NetTotal_Wtax { get; set; }
        public decimal tax_amount { get; set; }
        public decimal US { get; set; }
        public decimal Euro { get; set; }
        public decimal Austrialan { get; set; }
        public decimal Canadian { get; set; }
    }
}
