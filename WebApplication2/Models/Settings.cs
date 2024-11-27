using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Models
{
    public class Settings
    {
        public int inv_status { get; set; }
        public int ID { get; set; }
        public int Lang { get; set; }
        public string CompanyName { get; set; }
        public string Str { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string MImage { get; set; }
        public decimal Gst { get; set; }
        public string Ntn { get; set; }
        public string Bunit { get; set; }
        public string SaleNote { get; set; }
        public bool Bunit_status { get; set; }
        public bool CNameStatus { get; set; }
        public bool GstStatus { get; set; }
        public bool NtnStatus { get; set; }
        public bool SaleNoteStatus { get; set; }
        public bool LogoStatus { get; set; }
        public bool EmailStatus { get; set; }
        public bool PhoneStatus { get; set; }
        public bool TelephoneStatus { get; set; }
       
    }
}
