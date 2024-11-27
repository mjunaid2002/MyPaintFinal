using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CRM.Models
{
    public class Session
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is empty")]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       }
}
