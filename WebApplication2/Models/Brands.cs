using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Brands
    {
        [Required]
        [Display(Name ="ID")]
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is empty")]
        public string Name { get; set; }
        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public string b_unit { get; set; }
         
    }
}
