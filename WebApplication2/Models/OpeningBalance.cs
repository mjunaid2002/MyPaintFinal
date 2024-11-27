using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CRM.Models
{
    public class OpeningBalance
    {
        public int Id { get; set; }
        public int AccountNo { get; set; }
        public int Session_id { get; set; }
        public int Cr { get; set; }
        public int Dr { get; set; }
        public int b_unit { get; set; }
        
        public DateTime date { get; set; }
        public string narration { get; set; }

       }
}
