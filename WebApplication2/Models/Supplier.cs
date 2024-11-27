using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Supplier
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Ntn { get; set; }
        public string BankDetail { get; set; }
        public string CompanyName { get; set; }
        public int AccountNo { get; set; }
        public string Telephone { get; set; }
        public string Gst { get; set; }
        public string Provinces { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string CNIC { get; set; }
        public decimal BusinessUnit { get; set; }
        public string Description { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal PromptPaymentDiscount { get; set; }
        public string PaymentConditions { get; set; }
        public string Daysofpayment { get; set; }
        public decimal CreditLimit { get; set; }

    }
}
