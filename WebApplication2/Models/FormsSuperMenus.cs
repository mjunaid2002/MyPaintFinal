using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class FormsSuperMenus
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public decimal submenuid { get; set; }
        public decimal menuid { get; set; } 
    }
}
