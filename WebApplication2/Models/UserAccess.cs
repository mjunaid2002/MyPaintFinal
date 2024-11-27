using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class UserAccess
    {
        public int Id { get; set; }
        public decimal FormId { get; set; }
        public decimal SubFormId { get; set; }
        public decimal SuperFormId { get; set; }
        public decimal EmpId { get; set; }
    }
    
}
