using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
namespace WebApplication1.ViewModels
{
    public class TouchSaleInvVM
    {
        public IEnumerable<Ac_head> Ac_head_list { get; set; }
        public IEnumerable<Category> cat_list { get; set; }
        public IEnumerable<Product> pro_list { get; set; }
         public IEnumerable<Salequery> sale_list_woc { get; set; }
            
    }
}