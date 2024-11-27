using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class TransactionDetail
    {
        public int ID { get; set; }
        public int Rinvid { get; set; }
        public int TransId { get; set; }
        public string TransDes { get; set; }
        public string TransDate { get; set; }
        public int AccountId { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public int InvId { get; set; }
        public string Vtype { get; set; }
    }
    public class TransactionDetailqry
    {
        public int ID { get; set; }
        public int Rinvid { get; set; }
        public int TransId { get; set; }
        public string TransDes { get; set; }
        public string TransDate { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public int InvId { get; set; }
        public string Vtype { get; set; }
    }
}
