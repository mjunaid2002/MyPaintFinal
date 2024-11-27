using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class Cargo
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is empty")]
        public string Name { get; set; }
         public string Phone { get; set; }
         public string Address { get; set; }
         public string b_unit { get; set; }
        
    }
    public class MeasuringUnit
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is empty")]
        public string Name { get; set; }
        public string b_unit { get; set; }
      
    }
}