using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Attendance
    {
        public decimal Id { get; set; }
        public decimal EmpId { get; set; }
        public DateTime checkin_datetime { get; set; }
        public DateTime checkout_datetime { get; set; }
        public decimal Status { get; set; }
        public string EmpName { get; set; }
            
    }
}
