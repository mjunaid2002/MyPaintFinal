using CRM.Models;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebApplication1.Models;
using WebApplication2.Dbhelper;

namespace WebApplication2.Report
{
    public partial class CityReport : System.Web.UI.Page
    {
        private ApplicationDbContext _context;
        public CityReport()
        {
            _context = new ApplicationDbContext();
        }
        private List<Customer> emp;
        protected void Page_Load(object sender, EventArgs e)
        {

            //DataTable dt_item = new DataTable();
            //List<string> datelist = new List<string>();

            //dt_item = DBHelper.getDataTable("Select * from  customers ");
            //if (dt_item.Rows.Count > 0)
            //{
            //    for (int iii = 0; iii < dt_item.Rows.Count; iii++)
            //    {
            //        datelist.Add(dt_item.Rows[iii][0].ToString());
            //    }
            //}
            string id = Request.QueryString["id"];
            var list = _context.Database.SqlQuery<Customer>("SELECT * FROM Customers where id="+id+" ").ToList();
            //    emp = new List<Customer>()
            //{
            //    new Customer()
            //    { Phone="raki.kalluri@gmail.com" },
            //    new Customer()
            //    { Phone="raki.kalluri@gmail.com" },
            //    new Customer()
            //    { Phone="raki.kaldsadsadadsadsluri@gmail.com" },
            //    new Customer()
            //    { Phone="raki.kalluri@gmail.com" },
            //    new Customer()
            //    { Phone="raki.kalluri@gmail.com" }

            //};
            CityReportViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            CustomerReport crystalReport = new CustomerReport();
            crystalReport.SetDataSource(list);
            CityReportViewer.ReportSource = crystalReport;
            CityReportViewer.RefreshReport();
            //ParameterFields paramFields = new ParameterFields();
            //ParameterField pfItemYr = new ParameterField();
            //pfItemYr.ParameterFieldName = "CompanyName";
            //ParameterDiscreteValue dcItemYr = new ParameterDiscreteValue();
            //dcItemYr.Value = CompanyName;
            //pfItemYr.CurrentValues.Add(dcItemYr);
            //paramFields.Add(pfItemYr);
            //CityReportViewer.ParameterFieldInfo = paramFields;
        }
    }
}