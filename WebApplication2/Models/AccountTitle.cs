using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class AccountTitle
    {
        public int Id { get; set; }
        public int AccountNo { get; set; }
        public int AccMain { get; set; }
        public int AccountHeadId { get; set; }
        public int SecondLevel { get; set; }
         public string AccountTitleName { get; set; }
         public string b_unit { get; set; }
        public string AccountType { get; set; }
        public int cr { get; set; }
        public int dr { get; set; }
    }
}
