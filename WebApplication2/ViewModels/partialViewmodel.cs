using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication2.ViewModels
{
    public class partialViewmodel
    {
        public IEnumerable<Product> pro_list { get; set; }
    }
}