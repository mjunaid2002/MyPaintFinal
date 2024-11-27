using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class ChartofaccountVm
    {
        public IEnumerable<Ac_head> Ac_head { get; set; }
        public IEnumerable<Session> sess { get; set; }
        public IEnumerable<Item_ledger> pro_listsss { get; set; }
        public IEnumerable<Product> item_list { get; set; }
        public IEnumerable<Ac_head> Ac_head_list { get; set; }
        public IEnumerable<TrailQuery> exp_list { get; set; }
        public IEnumerable<TrailQuery> total_cgs { get; set; }
        public IEnumerable<TrailQuery> item_ist2 { get; set; }
        public IEnumerable<TrailQuery> item_ist { get; set; }
        public IEnumerable<TrailQuery> ass { get; set; }
        public IEnumerable<TrailQuery> lib { get; set; }
        public IEnumerable<TrailQuery> cap { get; set; }
        public IEnumerable<TrailQuery> trans { get; set; }
        public IEnumerable<SaleItemDetailsQuery> s_tems { get; set; }
        public IEnumerable<Ac_head> ac_head { get; set; }
        public IEnumerable<Ac_main> pro_list { get; set; }
        public IEnumerable<Ac_second> sec_list { get; set; }
        public IEnumerable<AccountTitle> third_level { get; set; }
        public IEnumerable<LedgerQuery> trans_list { get; set; }
        public OpeningBalance openingBalance { get; set; }
        public AccountTitle accountTitle { get; set; }
        public TransactionDetail transactionDetail { get; set; }
     


    }
}