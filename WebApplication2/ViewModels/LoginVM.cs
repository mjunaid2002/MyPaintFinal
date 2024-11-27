using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;

namespace WebApplication1.ViewModels
{
    public class LoginVM
    {
        public Employee Employee { get; set; }
        public BusinessUnit businessUnit { get; set; }
        public UserAccess UserAccess { get; set; }
        public UserLogin UserLogin { get; set; }
        public IEnumerable<UserRole> role_list { get; set; }
        public IEnumerable<Settings> Set_list { get; set; }
        public IEnumerable<BusinessUnit> bus_unit { get; set; }
      }
}