using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;

namespace WebApplication1.ViewModels
{
    public class CustomerVM
    {
        public Customer Customer { get; set; }
        public IEnumerable<Provinces> prov_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<City> city_list { get; set; }
        public IEnumerable<Town> town_list { get; set; }
    }
}