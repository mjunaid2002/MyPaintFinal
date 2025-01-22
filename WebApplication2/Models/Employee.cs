using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.QueryViewModel;

namespace CRM.Models
{
    public class Employee
    {
        public int ID { get; set; }
         public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public int AccountNo { get; set; }
        public decimal BusinessUnit { get; set; }
        public decimal mon_sal { get; set; }
        public decimal dal_sal { get; set; }
        public DateTime joiningdate { get; set; }
        public decimal dai_insentive { get; set; }
        public decimal cnic { get; set; }
        public decimal mon_insentive { get; set; }
        public decimal dep_id { get; set; }
        public decimal emp_id { get; set; }
        public int branch_id { get; set; }
        [NotMapped]
        public List<Emp_Department> departmentlist { get; set; }
        [NotMapped]
        public List<Employee> Employeelist { get; set; }
        [NotMapped]
        public List<Branch> Branchlist { get; set; }
        
        [NotMapped]
        public List<int> Branchid_list { get; set; }

    }

    public class Emp_Department
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
      

    }
}
