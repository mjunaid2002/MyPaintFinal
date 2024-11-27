using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Data;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ImportController : Controller
    {
        private ApplicationDbContext _context;
        public ImportController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: BusinessUnit
        public ActionResult Product()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportProducts(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;
                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();
                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Products";
                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("CatID", "CatID");
                        sqlBulkCopy.ColumnMappings.Add("BrandID", "BrandID");
                        sqlBulkCopy.ColumnMappings.Add("Image", "Image");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("CostPrice", "CostPrice");
                        sqlBulkCopy.ColumnMappings.Add("SalePrice", "SalePrice");
                        sqlBulkCopy.ColumnMappings.Add("LeastPrice", "LeastPrice");
                        sqlBulkCopy.ColumnMappings.Add("PiecesBox", "PiecesBox");
                        sqlBulkCopy.ColumnMappings.Add("OpeningStock", "OpeningStock");
                        sqlBulkCopy.ColumnMappings.Add("ReOrder", "ReOrder");
                        sqlBulkCopy.ColumnMappings.Add("StockDate", "StockDate");
                        sqlBulkCopy.ColumnMappings.Add("Barcode", "Barcode");
                        sqlBulkCopy.ColumnMappings.Add("Wat", "Wat");
                        sqlBulkCopy.ColumnMappings.Add("Brand", "Brand");
                        sqlBulkCopy.ColumnMappings.Add("MUnit", "MUnit");
                        sqlBulkCopy.ColumnMappings.Add("ShelfNumber", "ShelfNumber");
                        sqlBulkCopy.ColumnMappings.Add("BusinessUnit", "BusinessUnit");
                        sqlBulkCopy.ColumnMappings.Add("type", "type");
                        sqlBulkCopy.ColumnMappings.Add("SalePrice_euro", "SalePrice_euro");
                        //sqlBulkCopy.ColumnMappings.Add("Active", "Active");
                        //sqlBulkCopy.ColumnMappings.Add("payrollcalendar", "payrollcalendar");
                        //sqlBulkCopy.ColumnMappings.Add("Start Date", "CreationDateTime");
                        //sqlBulkCopy.ColumnMappings.Add("End Date", "LastUpdateDateTime");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                        ViewBag.Result = "Successfully Imported !";
                    }
                }
            }
            return RedirectToAction("Product", "Import", new { ac = "Succ" });
        }

        public ActionResult Supplier()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportSupplier(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;
                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();
                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Suppliers";
                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("ID", "ID");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                        sqlBulkCopy.ColumnMappings.Add("Phone", "Phone");
                        sqlBulkCopy.ColumnMappings.Add("Address", "Address");
                        sqlBulkCopy.ColumnMappings.Add("Image", "Image");
                        sqlBulkCopy.ColumnMappings.Add("Ntn", "Ntn");
                        sqlBulkCopy.ColumnMappings.Add("BankDetail", "BankDetail");
                        sqlBulkCopy.ColumnMappings.Add("CompanyName", "CompanyName");
                        sqlBulkCopy.ColumnMappings.Add("AccountNo", "AccountNo");
                        sqlBulkCopy.ColumnMappings.Add("Telephone", "Telephone");
                        sqlBulkCopy.ColumnMappings.Add("Gst", "Gst");
                        sqlBulkCopy.ColumnMappings.Add("Provinces", "Provinces");
                        sqlBulkCopy.ColumnMappings.Add("City", "City");
                        sqlBulkCopy.ColumnMappings.Add("Town", "Town");
                        sqlBulkCopy.ColumnMappings.Add("BusinessUnit", "BusinessUnit");
                        sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                        sqlBulkCopy.ColumnMappings.Add("SpecialDiscount", "SpecialDiscount");
                        sqlBulkCopy.ColumnMappings.Add("PromptPaymentDiscount", "PromptPaymentDiscount");
                        sqlBulkCopy.ColumnMappings.Add("PaymentConditions", "PaymentConditions");
                        sqlBulkCopy.ColumnMappings.Add("Daysofpayment", "Daysofpayment");
                        sqlBulkCopy.ColumnMappings.Add("CreditLimit", "CreditLimit");
                        sqlBulkCopy.ColumnMappings.Add("CNIC", "CNIC");

                        //sqlBulkCopy.ColumnMappings.Add("payrollcalendar", "payrollcalendar");
                        //sqlBulkCopy.ColumnMappings.Add("Start Date", "CreationDateTime");
                        //sqlBulkCopy.ColumnMappings.Add("End Date", "LastUpdateDateTime");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                        ViewBag.Result = "Successfully Imported !";
                    }
                }
            }
            return RedirectToAction("Supplier", "Import", new { ac = "Succ" });
        }


    }
}