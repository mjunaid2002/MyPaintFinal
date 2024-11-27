using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
using System;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class PurchaseWtaxController : Controller
    {
        private ApplicationDbContext _context;
        public PurchaseWtaxController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: PurchaseWtax
        public ActionResult Index()
        {
            bool b_status = Convert.ToBoolean(Session["Bunit_status"]);
            var query = "";
            if (b_status == false)
            {
                query = "AND b_unit = '" + Session["BusinessUnit"] + "' ";
            }
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWTAX' " + query + " ORDER BY InvID DESC").ToList();
            var img_list = _context.Database.SqlQuery<VoucherImagesquery>("SELECT * from VoucherImages where VType = 'PINVWTAX'").ToList();

            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
                img_list = img_list,

            };
            return View(PurInvoiceVM);
        }
        public ActionResult Create(PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvType = 'PINVWTAX'").FirstOrDefault();
            var sup_list = _context.Supplier.ToList();
            var emp_list = _context.Employee.ToList();
            var pro_list = _context.Product.ToList();
            var pur_list_woc = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWTAX' ORDER BY InvID DESC").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {

                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                pur_list_woc = pur_list_woc,
                pro_list = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost, ActionName("Create")]
        public ActionResult Save(HttpPostedFileBase[] file, string[] tax_per, string[] tax_amount, string[] n_totalwt, int[] p_box, int[] b_no, string[] exp_date, string[] item_name, int[] id, string[] sp, string[] qty, string[] n_total, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
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
                _context.Database.ExecuteSqlCommand("INSERT INTO VoucherImages (Images,Tid,Vtype) VALUES ('" + img + "'," + PurMaster.InvID + ",'PINVWTAX')");
            }
            for (int i = 0; i < item_name.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO PurDetails (b_unit,InvId,ItemID,ItemName,SP,Qty,NetTotal,Date,Ctn,PiecesBox,BatchNumber,Expriy,InvType,tax_item,tax_amount,NetTotal_wtax) VALUES ('" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + id[i] + ",N'" + item_name[i] + "'," + sp[i] + "," + qty[i] + "," + n_total[i] + ",'" + PurMaster.Date + "',0," + p_box[i] + "," + b_no[i] + ",'" + exp_date[i] + "','PINVWTAX'," + tax_per[i] + "," + tax_amount[i] + "," + n_totalwt[i]+")");
            }

            //Sale Master Data
            _context.Database.ExecuteSqlCommand("INSERT INTO PurMasters (b_unit,InvID,SupplierId,DsrId,Address,Phone,Date,CargoName,CargoCharges,NetAmount,DiscountAmount,GrandTotal,Total,Rtotal,BTotal,InvType,Total_Wtax,NetTotal_Wtax,tax_amount) VALUES ('" + Session["BusinessUnit"] + "'," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + PurMaster.DsrId + ",N'" + PurMaster.Address + "','" + PurMaster.Phone + "','" + PurMaster.Date + "',N'" + PurMaster.CargoName + "'," + PurMaster.CargoCharges + "," + PurMaster.NetAmount + "," + PurMaster.DiscountAmount + "," + PurMaster.GrandTotal + "," + PurMaster.Total + "," + PurMaster.Rtotal + "," + PurMaster.BTotal + ",'PINVWTAX'," + PurMaster.Total_Wtax + "," + PurMaster.NetTotal_Wtax + "," + PurMaster.tax_amount + ")");

            TransactionDetail.TransId = _context.Database.SqlQuery<int>("select ISNULL(Max(TransId),0)+1 from TransactionDetails").FirstOrDefault();

            //Transaction Data
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Supplier'," + PurMaster.SupplierId + ",0,'" + PurMaster.GrandTotal + "'," + PurMaster.InvID + ",'PINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Stock',111111,'" + PurMaster.GrandTotal + "',0," + PurMaster.InvID + ",'PINVWTAX')");
            _context.Database.ExecuteSqlCommand("INSERT INTO TransactionDetails (b_unit,TransId,TransDate,TransDes,AccountId,Dr,Cr,InvId,Vtype) VALUES ('" + Session["BusinessUnit"] + "'," + TransactionDetail.TransId + ",'" + PurMaster.Date + "','Tax Payable',131313,'" + PurMaster.tax_amount + "',0," + PurMaster.InvID + ",'PINVWTAX')");
            return RedirectToAction("Create");
        }
    }
}