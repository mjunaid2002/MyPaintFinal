using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class UserLogin
    {
        public decimal Id { get; set; }
        public string UserName { get; set; }
        public decimal EmpId { get; set; }
        public decimal teamlead_id { get; set; }
        public decimal dep_id { get; set; }
        public string Password { get; set; }
        public string b_unit { get; set; }
        public decimal roleid { get; set; }
        public decimal StoreID { get; set; }
    }
    public class UserRole
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string b_unit { get; set; }
       
        
    }
    public class UserRoleView
    {
        public decimal id { get; set; }
        public decimal EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string role { get; set; }

        public string teamlead_id { get; set; }
        public string dep_id { get; set; }
    }
}
