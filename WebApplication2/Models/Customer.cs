using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Customer
    {
        public decimal customerid { get; set; }
      //  public int Count { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Ntn { get; set; }
        public string BankDetail { get; set; }
        public string CompanyName { get; set; }
        public decimal AccountNo { get; set; }
        public string Telephone { get; set; }
        public string Gst { get; set; }
        public string Provinces { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string CNIC { get; set; }
        public string BusinessUnit { get; set; }
        public string Description { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal PromptPaymentDiscount { get; set; }
        public string PaymentConditions { get; set; }
        public string Daysofpayment { get; set; }
        public decimal CreditLimit { get; set; }
        public int Dsr { get; set; }
        public int longi { get; set; }
        public int lati { get; set; }
        public bool comp_check { get; set; }
        public bool cus_check { get; set; }

    }
}
