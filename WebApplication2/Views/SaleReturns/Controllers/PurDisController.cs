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
    public class PurDisController : Controller
    {
        private ApplicationDbContext _context;

        public PurDisController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Purchase
        public ActionResult Index()
        {
            var list = _context.Database.SqlQuery<PurchaseDis>("SELECT *,(Select name From Suppliers where AccountNo=PurMasterDis.supid) as SupplierName FROM PurMasterDis ORDER BY InvID DESC").ToList();
            return View(list);
        }
        public ActionResult PurDisRep(int ID)
        {
            var list = _context.Database.SqlQuery<PurDetailDis>("Select * from PurDetailDis where InvId = " + ID + "").ToList();
            var date = _context.Database.SqlQuery<String>("SELECT Date FROM PurMasterDis where InvID =" + ID + "").FirstOrDefault();
            var cargo = _context.Database.SqlQuery<decimal>("SELECT CargoCharges FROM PurMasterDis where InvID =" + ID + "").FirstOrDefault();
            var dis = _context.Database.SqlQuery<decimal>("SELECT Discount FROM PurMasterDis where InvID =" + ID + "").FirstOrDefault();
            var grandtotal = _context.Database.SqlQuery<decimal>("SELECT GrandTotal FROM PurMasterDis where InvID =" + ID + "").FirstOrDefault();
            var CompanyName = _context.Database.SqlQuery<String>("SELECT CompanyName FROM Settings ").FirstOrDefault();
            var Email = _context.Database.SqlQuery<String>("SELECT Email FROM Settings ").FirstOrDefault();
            var Phone = _context.Database.SqlQuery<String>("SELECT Phone FROM Settings ").FirstOrDefault();
            var Image = _context.Database.SqlQuery<String>("SELECT MImage FROM Settings ").FirstOrDefault();
            var Address = _context.Database.SqlQuery<String>("SELECT Address FROM Settings ").FirstOrDefault();
            var supplier_name = _context.Database.SqlQuery<string>("SELECT (Select name From Suppliers where AccountNo=PurMasterDis.supid) as supname  FROM PurMasterDis where InvID ="+ID+"").FirstOrDefault();
            var email = _context.Database.SqlQuery<string>("SELECT (Select email From Suppliers where AccountNo=PurMasterDis.supid) as supname  FROM PurMasterDis where InvID ="+ID+"").FirstOrDefault();
            var address = _context.Database.SqlQuery<string>("SELECT (Select address From Suppliers where AccountNo=PurMasterDis.supid) as supname  FROM PurMasterDis where InvID ="+ID+"").FirstOrDefault();
            var phone = _context.Database.SqlQuery<string>("SELECT (Select phone From Suppliers where AccountNo=PurMasterDis.supid) as supname  FROM PurMasterDis where InvID ="+ID+"").FirstOrDefault();
               ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Report"), "rptpurdis.rpt"));

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
            rd.SetParameterValue("cusemail", email);
            rd.SetParameterValue("cusphone", phone);
            rd.SetParameterValue("cusaddress", address);
            //rd.SetParameterValue("Image", Image);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptpurdis.pdf");
        }

        public ActionResult Create(PurMasterDis PurMasterDis, PurDetailDis PurDetailDis, TransactionDetail TransactionDetail)
        {
            PurMasterDis.invid = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasterDis").FirstOrDefault();
            var sup_list = _context.Supplier.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseDis>("SELECT *,(Select name From Suppliers where AccountNo=PurMasterDis.supid) as SupplierName FROM PurMasterDis ORDER BY InvID DESC").ToList();
            var PurDisVM = new PurDisVM
            {

                PurDetailDis = PurDetailDis,
                PurMasterDis = PurMasterDis,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                pur_list_woc = pur_list_woc,
                pro_list = pro_list
            };
            return View(PurDisVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, string[] item_name, int[] id, decimal[] dis_amount,decimal[] sp, decimal[] qty, decimal[] n_total, decimal[] disc, decimal[] t_p, decimal[] r_p, decimal[] ctn, decimal[] p_box, PurMasterDis PurMasterDis, PurDetailDis PurDetailDis, TransactionDetail TransactionDetail)
        {
            Random r = new Random();
            int num = r.Next();
            string ImageName = "";
            string physicalPath;
            string img;

            for (int i = 0; i < file.Count(); i++)
            {

                if (file[i] == null)
                {
                    img = "demo.jpg";
                }
                else
                {
                    ImageName = System.IO.Path.GetFileName(file[i].FileName);
                    img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file[i].SaveAs(physicalPath);
                }
                _context.Database.ExecuteSqlCommand("INSERT INTO hclpvtlt_hclnew.VoucherImages (Images,Tid,Vtype) VALUES ('" + img + "'," + PurMasterDis.invid + ",'PINVDIS')");
            }
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetailDis (invid,pid,pname,cp,qty,total,Date,ctn,tradeprice,retailprice,RateCode,disc,p_box,sr,dis_amount) VALUES (" + PurMasterDis.invid + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMasterDis.date + "'," + ctn[i] + "," + t_p[i] + "," + r_p[i] + "," + PurMasterDis.RateCode + "," + disc[i] + "," + p_box[i] + ",0,"+ dis_amount [i]+ ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasterDis (invid,date,supid,cargoname,cargocharges,othercharges,discount,total,PhoneNo,RateCode,Address,GrandTotal) VALUES (" + PurMasterDis.invid + ",'" + PurMasterDis.date + "'," + PurMasterDis.supid + ",N'" + PurMasterDis.cargoname + "'," + PurMasterDis.cargocharges + "," + PurMasterDis.othercharges + "," + PurMasterDis.discount + "," + PurMasterDis.total + ",'" + PurMasterDis.PhoneNo + "'," + PurMasterDis.RateCode + ",N'" + PurMasterDis.Address + "'," + PurMasterDis.GrandTotal + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,Vtype,InvId) VALUES (" + TransactionDetail.TransId + ",'" + PurMasterDis.date + "','Supplier'," + PurMasterDis.supid + ",0,'" + PurMasterDis.GrandTotal + "','PINV'," + PurMasterDis.invid + ")");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,Vtype,InvId) VALUES (" + TransactionDetail.TransId + ",'" + PurMasterDis.date + "','Stock',111111,'" + PurMasterDis.GrandTotal + "',0,'PINV'," + PurMasterDis.invid + ")");

            return RedirectToAction("Create");
        }
        public ActionResult Edit(int? ID, PurMasterDis PurMasterDis, PurDetailDis PurDetailDis, TransactionDetail TransactionDetail)
        {
            PurMasterDis.invid = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasterDis").FirstOrDefault();
            var sup_list = _context.Supplier.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseDis>("SELECT *,(Select name From Suppliers where AccountNo=PurMasterDis.supid) as SupplierName FROM PurMasterDis ORDER BY InvID DESC").ToList();
            PurMasterDis = _context.PurMasterDis.SingleOrDefault(c => c.invid == ID);
            var p_detail = _context.Database.SqlQuery<PurDetailDis>("SELECT * from PurDetailDis where InvID=" + ID + "").ToList();

            var PurDisVM = new PurDisVM
            {

                PurDetailDis = PurDetailDis,
                PurMasterDis = PurMasterDis,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                pur_list_woc = pur_list_woc,
                p_detail= p_detail,

                pro_list = pro_list
            };
            return View(PurDisVM);
        }
        [HttpPost]
        public ActionResult Update(string[] item_name, int[] id, decimal[] dis_amount, decimal[] sp, decimal[] qty, decimal[] n_total, decimal[] disc, decimal[] t_p, decimal[] r_p, decimal[] ctn, decimal[] p_box, PurMasterDis PurMasterDis, PurDetailDis PurDetailDis, TransactionDetail TransactionDetail, PurDisVM PurDisVM)
        {
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + PurDisVM.PurMasterDis.invid + " ");
            _context.Database.ExecuteSqlCommand("Delete From PurDetailDis where InvId =" + PurDisVM.PurMasterDis.invid + " ");
            _context.Database.ExecuteSqlCommand("Delete From PurMasterDis where InvId =" + PurDisVM.PurMasterDis.invid + " ");

            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetailDis (invid,pid,pname,cp,qty,total,Date,ctn,tradeprice,retailprice,RateCode,disc,p_box,sr,dis_amount) VALUES (" + PurMasterDis.invid + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMasterDis.date + "'," + ctn[i] + "," + t_p[i] + "," + r_p[i] + "," + PurMasterDis.RateCode + "," + disc[i] + "," + p_box[i] + ",0," + dis_amount[i] + ")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasterDis (invid,date,supid,cargoname,cargocharges,othercharges,discount,total,PhoneNo,RateCode,Address,GrandTotal) VALUES (" + PurMasterDis.invid + ",'" + PurMasterDis.date + "'," + PurMasterDis.supid + ",N'" + PurMasterDis.cargoname + "'," + PurMasterDis.cargocharges + "," + PurMasterDis.othercharges + "," + PurMasterDis.discount + "," + PurMasterDis.total + ",'" + PurMasterDis.PhoneNo + "'," + PurMasterDis.RateCode + ",N'" + PurMasterDis.Address + "'," + PurMasterDis.GrandTotal + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,Vtype,InvId) VALUES (" + TransactionDetail.TransId + ",'" + PurMasterDis.date + "','Supplier'," + PurMasterDis.supid + ",0,'" + PurMasterDis.GrandTotal + "','PINV'," + PurMasterDis.invid + ")");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (TransId,TransDate,TransDes,AccountId,Dr,Cr,Vtype,InvId) VALUES (" + TransactionDetail.TransId + ",'" + PurMasterDis.date + "','Stock',111111,'" + PurMasterDis.GrandTotal + "',0,'PINV'," + PurMasterDis.invid + ")");

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _context.Database.ExecuteSqlCommand("Delete From TransactionDetails where InvId =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From PurDetailDis where InvId =" + ID + "");
            _context.Database.ExecuteSqlCommand("Delete From PurMasterDis where InvId =" + ID + "");
            return RedirectToAction("Index");
        }
    }
}