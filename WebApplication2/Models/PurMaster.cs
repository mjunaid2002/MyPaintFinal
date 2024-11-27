using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class PurMaster
    {
        public int ID { get; set; }
        public int pinv_id { get; set; }
        public int InvID { get; set; }
        public int SupplierId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string b_unit { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public int Rtotal { get; set; }
        public int Store { get; set; }
        public decimal BTotal { get; set; }
        public string InvType { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal NetTotal_Wtax { get; set; }
        public decimal tax_amount { get; set; }
    }
    public class PurMasterReturn
    {
        public int ID { get; set; }
        public int pinv_id { get; set; }
        public int InvID { get; set; }
        public int SupplierId { get; set; }
        public int DsrId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string CargoName { get; set; }
        public decimal CargoCharges { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Total { get; set; }
        public int Rtotal { get; set; }
        public int Store { get; set; }
        public decimal BTotal { get; set; }
        public string InvType { get; set; }
        public decimal Total_Wtax { get; set; }
        public decimal NetTotal_Wtax { get; set; }
        public decimal tax_amount { get; set; }
    }
}
