using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class BusinessUnit
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is empty")]
        public string Name { get; set; }
        public int Lang { get; set; }
        public string Image { get; set; }
        public string expiraydate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Gst { get; set; }
        public string Ntn { get; set; }
    }
}