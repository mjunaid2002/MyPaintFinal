using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.ViewModels
{
    public class UserMan
    {
        public IEnumerable<UserRole> role_list { get; set; }
        public IEnumerable<BusinessUnit> b_unit { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<Employee> teamlead_list { get; set; }
        public IEnumerable<Emp_Department> dep_list { get; set; }
        public IEnumerable<UserLogHistory> userLogHistories { get; set; }
        public IEnumerable<Store> store_list { get; set; }
        public IEnumerable<UserAccess> UserAccess { get; set; }
        public IEnumerable<UserRoleView> Userlogin_list { get; set; }
        public UserLogin UserLogin { get; set; }
        public UserRole Roles { get; set; }
    }
}