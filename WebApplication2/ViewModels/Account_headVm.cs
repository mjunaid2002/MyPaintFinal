using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication2.Models;

namespace WebApplication1.ViewModels
{
    public class Account_headVm
    {
        public IEnumerable<Ac_head> Ac_head_list { get; set; }
        public Ac_main Ac_main { get; set; }
        public Ac_second ac_Second { get; set; }
        public AccountTitle accountTitle { get; set; }
        public AccountTitle AccountTitle { get; set; }
    }
}