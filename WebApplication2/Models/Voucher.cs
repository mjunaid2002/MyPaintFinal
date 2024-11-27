using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public int TID { get; set; }
        public int Account_Id { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public string Date { get; set; }
        public string Narr { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string Vtype { get; set; }
        public int Bank_Account { get; set; }
        public string Cl_date { get; set; }
        public string ChequeNo { get; set; }
        public string Cheque_status { get; set; }

    }
}
