using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;
namespace WebApplication1.ViewModels
{
    public class SaleDisVM
    {
        public IEnumerable<Customer> cus_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<Product> pro_list { get; set; }
        public IEnumerable<SaleDetailDis> s_detail { get; set; }
        public IEnumerable<SaleDis> sale_list { get; set; }
        public IEnumerable<PurDetailDis> pur_list { get; set; }
        public SaleMasterDis SaleMasterDis { get; set; }
        public SaleDetailDis SaleDetailDis { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
    }
}