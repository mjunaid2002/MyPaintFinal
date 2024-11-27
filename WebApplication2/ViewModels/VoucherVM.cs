using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.ViewModels
{
    public class VoucherVM
    {
        public IEnumerable<VoucherQuery> vou_det1 { get; set; }
        public IEnumerable<TransactionDetail> vou_det2 { get; set; }
        public IEnumerable<AccountTitle> Acc_List_cash { get; set; }
        public IEnumerable<AccountTitle> Acc_List { get; set; }
        public VoucherMaster Vouchers { get; set; }
        public BusinessUnit Settings { get; set; }
        public VoucherMaster VoucherMaster { get; set; }
        public Voucher Voucher { get; set; }
        public Attendance attendance { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<VoucherMaster> vou_list { get; set; }
        public IEnumerable<AttendanceQuery> atte_list { get; set; }
        public IEnumerable<Voucher> vou_det { get; set; }
        public IEnumerable<Customer> cus_list { get; set; }
        public IEnumerable<AccountTitle> ac_list { get; set; }
        public TransactionDetail TransactionDetail { get; set; }
        public Int32 from_acc { get; set; }
        public int acc { get; set; }
    }
}