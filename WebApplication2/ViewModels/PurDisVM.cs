using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
namespace WebApplication1.ViewModels
{
    public class PurDisVM
    {
        public IEnumerable<Supplier> sup_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<Product> pro_list { get; set; }
        public IEnumerable<PurDetailDis> p_detail { get; set; }
        public IEnumerable<PurMasterDis> pur_list { get; set; }
        public IEnumerable<PurchaseDis> pur_list_woc { get; set; }
        public PurDetailDis PurDetailDis { get; set; }
        public PurMasterDis PurMasterDis { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
    }
}