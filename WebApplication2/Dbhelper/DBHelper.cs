using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication2.Dbhelper
{
    public class DBHelper
    {

        public static string ConnectionString()
        {
            return "server=66.165.248.146;database=hclpvtlt_pos; User Id=hclpvtlt_pos;Password=9%6Jswn8";
        }

        //delete from qtnm;
        //delete from qtndetail;
        //delete from purchasem;
        //delete from purchasedetail;
        //delete from srpm;
        //delete from srpdetail;
        //delete from ProductIngrMaster;
        //delete from ProductIngrDetail;
        //delete from ProductIngrDetail1;
        //delete from tbl_BatchTransfer;
        //delete from labm;
        //delete from labdetail;
        //delete from samplem;
        //delete from sampledetail;
        //delete from samplem2;
        //delete from sampledetail2;

        public static DataTable getDataTable(string query)
        {
            DataTable dt = new DataTable();
            if (query != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString()))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(dt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                   
                }
            }
            return dt;
        }
    }
}