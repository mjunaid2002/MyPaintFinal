using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class SaleDisController : Controller
    {
        private ApplicationDbContext _context;

        public SaleDisController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Purchase
        public ActionResult Index(SaleMasterDis SaleMasterDis)
        {
            var list = _context.Database.SqlQuery<SaleDis>("SELECT *,(Select name From Customers where AccountNo=SaleMasterDis.custid) as CustomerName FROM SaleMasterDis ORDER BY InvId DESC").ToList();
            return View(list);
        }

        public ActionResult SaleDisRep(int ID)
        {
            var list = _context.Database.SqlQuery<saledetaildisRep>("SELECT od.invid as ID, od.pid, od.ratecode, od.pname, od.retailprice, od.cp, od.ctn, od.retailprice, od.qty* od.cp AS total, (od.cp - od.disc* Products.Wat) * od.qty AS disctotal,od.retailprice* od.qty - od.retailprice* od.qty / 117 * 100 AS FedTax, od.disc, od.qty, od.retailprice* od.cp - (od.retailprice* od.qty - od.retailprice* od.qty / 117 * 100) AS GrossValue,od.disc* od.qty * Products.Wat AS tovalue, od.qty* od.cp - (od.retailprice* od.qty - od.retailprice* od.qty / 117 * 100) AS NVEF, 0 AS AddToRate,0 AS AddToValue, 0 AS specialdisc, Products.Wat* od.qty AS totalweight, Products.Barcode FROM         PurDetailDis AS od INNER JOIN Products ON od.pid = Products.ID WHERE     (od.invid = "+ID+") ORDER BY od.sr").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT CONVERT(varchar, date, 105) FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT Discount FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM PurMasterDis where InvID =" + ID + "").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var supplier_name = _context.Database.SqlQuery<string>("SELECT (Select name From Customers where AccountNo=SaleMasterDis.custid) as cusname  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var email = _context.Database.SqlQuery<string>("SELECT (Select email From Customers where AccountNo=SaleMasterDis.custid) as cusname  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var address = _context.Database.SqlQuery<string>("SELECT (Select address From Customers where AccountNo=SaleMasterDis.custid) as cusname  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var CNIC = _context.Database.SqlQuery<string>("SELECT (Select CNIC From Customers where AccountNo=SaleMasterDis.custid) as CNIC  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var NTN = _context.Database.SqlQuery<string>("SELECT (Select ntn From Customers where AccountNo=SaleMasterDis.custid) as ntn  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            var phone = _context.Database.SqlQuery<string>("SELECT (Select phone From Customers where AccountNo=SaleMasterDis.custid) as cusname  FROM SaleMasterDis where InvID =" + ID + "").FirstOrDefault();
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptsaledis.rpt"));

            rd.SetDataSource(list);
            rd.SetParameterValue("compname", CompanyName);
            rd.SetParameterValue("email", Email);
            rd.SetParameterValue("phone", Phone);
            rd.SetParameterValue("address", Address);
            rd.SetParameterValue("date", date);
            rd.SetParameterValue("invid", ID);
            rd.SetParameterValue("customername", supplier_name);
            rd.SetParameterValue("cargo", cargo);
            rd.SetParameterValue("dis", dis);
            rd.SetParameterValue("grandtotal", grandtotal);
            rd.SetParameterValue("email", Email);
            rd.SetParameterValue("CNIC", CNIC);
            rd.SetParameterValue("NTN", NTN);
            rd.SetParameterValue("cusphone", phone);
            rd.SetParameterValue("cusaddress", address);
            //rd.SetParameterValue("Image", Image);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptsaledis.pdf");
        }
        public ActionResult Create(SaleMasterDis SaleMasterDis, SaleDetailDis SaleDetailDis, TransactionDetail TransactionDetail)
        {
            SaleMasterDis.InvId = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasterDis").FirstOrDefault();
            var cus_list = _context.Customer.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list = _context.PurDetailDis.ToList();
            var sale_list = _context.Database.SqlQuery<SaleDis>("SELECT *,(Select name From Customers where AccountNo=SaleMasterDis.custid) as CustomerName FROM SaleMasterDis ORDER BY InvId DESC").ToList();
             var SaleDisVM = new SaleDisVM
            {

                SaleDetailDis = SaleDetailDis,
                SaleMasterDis = SaleMasterDis,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                pur_list = pur_list,
                sale_list = sale_list,
                pro_list = pro_list
            };
            return View(SaleDisVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(string[] item_name, decimal[] dis_amount, int[] id, decimal[] sp, decimal[] qty, decimal[] n_total, decimal[] disc, decimal[] t_p, decimal[] r_p, decimal[] ctn, decimal[] r_code, SaleMasterDis SaleMasterDis, SaleDetailDis SaleDetailDis, TransactionDetail TransactionDetail)
        {
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetailDis (InvId,prid,prname,sp,qty,total,Date,ctn,tradeprice,retailprice,RateCode,disc,dis_amount) VALUES (" + SaleMasterDis.InvId + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMasterDis.date + "'," + ctn[i] + "," + t_p[i] + "," + r_p[i] + "," + r_code[i] + "," + disc[i] + ","+ dis_amount [i]+ ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasterDis (InvId,empid,date,custid,cargoname,cargocharges,discount,total,PhoneNumber,RateCode,Address,GrandTotal,rec) VALUES (" + SaleMasterDis.InvId + "," + SaleMasterDis.empid + ",'" + SaleMasterDis.date + "'," + SaleMasterDis.custid + ",N'" + SaleMasterDis.cargoname + "'," + SaleMasterDis.cargocharges + "," + SaleMasterDis.discount + "," + SaleMasterDis.total + ",'" + SaleMasterDis.PhoneNumber + "'," + SaleMasterDis.RateCode + ",N'" + SaleMasterDis.Address + "'," + SaleMasterDis.GrandTotal + "," + SaleMasterDis.rec + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Customer'," + SaleMasterDis.custid + ",'" + SaleMasterDis.GrandTotal + "',0," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Sale',222222,0,'" + SaleMasterDis.GrandTotal + "'," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','CGS',333333,'" + SaleMasterDis.GrandTotal + "',0," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Stock',111111,0,'" + SaleMasterDis.GrandTotal + "'," + SaleMasterDis.InvId + ",'SINV')");
       
            return RedirectToAction("Create");
        }

        public ActionResult Edit(int? ID, SaleMasterDis SaleMasterDis, SaleDetailDis SaleDetailDis, TransactionDetail TransactionDetail)
        {
            SaleMasterDis.InvId = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from SaleMasterDis").FirstOrDefault();
            var cus_list = _context.Customer.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list = _context.PurDetailDis.ToList();
            var sale_list = _context.Database.SqlQuery<SaleDis>("SELECT *,(Select name From Customers where AccountNo=SaleMasterDis.custid) as CustomerName FROM SaleMasterDis ORDER BY InvId DESC").ToList();
            SaleMasterDis = _context.SaleMasterDis.SingleOrDefault(c => c.InvId == ID);
            var s_detail = _context.Database.SqlQuery<SaleDetailDis>("SELECT * from SaleDetailDis where InvID=" + ID + "").ToList();

            var SaleDisVM = new SaleDisVM
            {

                SaleDetailDis = SaleDetailDis,
                SaleMasterDis = SaleMasterDis,
                TransactionDetail = TransactionDetail,
                cus_list = cus_list,
                emp_list = emp_list,
                pur_list = pur_list,
                s_detail = s_detail,
                sale_list = sale_list,
                pro_list = pro_list
            };
            return View(SaleDisVM);
        }
        [HttpPost]
        public ActionResult Update(SaleDisVM SaleDisVM,string[] item_name, decimal[] dis_amount, int[] id, decimal[] sp, decimal[] qty, decimal[] n_total, decimal[] disc, decimal[] t_p, decimal[] r_p, decimal[] ctn, decimal[] r_code, SaleMasterDis SaleMasterDis, SaleDetailDis SaleDetailDis, TransactionDetail TransactionDetail)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + SaleDisVM.SaleMasterDis.InvId + " ");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetailDis where InvId =" + SaleDisVM.SaleMasterDis.InvId + " ");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasterDis where InvId =" + SaleDisVM.SaleMasterDis.InvId + " ");

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO SaleDetailDis (InvId,prid,prname,sp,qty,total,Date,ctn,tradeprice,retailprice,RateCode,disc,dis_amount) VALUES (" + SaleMasterDis.InvId + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + SaleMasterDis.date + "'," + ctn[i] + "," + t_p[i] + "," + r_p[i] + "," + r_code[i] + "," + disc[i] + "," + dis_amount[i] + ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO SaleMasterDis (InvId,empid,date,custid,cargoname,cargocharges,discount,total,PhoneNumber,RateCode,Address,GrandTotal,rec) VALUES (" + SaleMasterDis.InvId + "," + SaleMasterDis.empid + ",'" + SaleMasterDis.date + "'," + SaleMasterDis.custid + ",N'" + SaleMasterDis.cargoname + "'," + SaleMasterDis.cargocharges + "," + SaleMasterDis.discount + "," + SaleMasterDis.total + ",'" + SaleMasterDis.PhoneNumber + "'," + SaleMasterDis.RateCode + ",N'" + SaleMasterDis.Address + "'," + SaleMasterDis.GrandTotal + "," + SaleMasterDis.rec + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Customer'," + SaleMasterDis.custid + ",'" + SaleMasterDis.GrandTotal + "',0," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Sale',222222,0,'" + SaleMasterDis.GrandTotal + "'," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','CGS',333333,'" + SaleMasterDis.GrandTotal + "',0," + SaleMasterDis.InvId + ",'SINV')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES (" + TransactionDetail.TransId + ",'" + SaleMasterDis.date + "','Stock',111111,0,'" + SaleMasterDis.GrandTotal + "'," + SaleMasterDis.InvId + ",'SINV')");

            return RedirectToAction("Create");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From SaleDetailDis where InvId =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From SaleMasterDis where InvId =" + ID + "");
            return RedirectToAction("Index");
        }
    }
}

