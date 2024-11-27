using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CRM.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is empty")]
        public string Name { get; set; }

        public string Image { get; set; }
        public string b_unit { get; set; }
       }
 
}
