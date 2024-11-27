using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class tbl_BatchRequest
    {
        public int Id { get; set; }
         public int orderid { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public string batchno { get; set; }
        public string department { get; set; }
    }
}