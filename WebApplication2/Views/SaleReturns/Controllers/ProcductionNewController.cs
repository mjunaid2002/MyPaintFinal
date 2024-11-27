using System;
using System.Linq;
using System.Web.Mvc;
using CRM.Models;
using WebApplication1.ViewModels;
using WebApplication1.QueryViewModel;
using WebApplication1.Models;
namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class ProcductionNewController : Controller
    {
        private ApplicationDbContext _context;
        public ProcductionNewController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: ProcductionNew
        public ActionResult Purchases()
        {
            //bool b_status = Convert.ToBoolean(Session["Bunit_status"]);
            //var query = "";
            //if (b_status == false)
            //{
            //    query = "AND b_unit = '" + Session["BusinessUnit"] + "' ";
            //}
            //var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT *,(Select name From Suppliers where AccountNo=PurMasters.SupplierId) as SupplierName ,(Select name From Employees where AccountNo=PurMasters.DsrId) as EmployeeName FROM PurMasters where InvType = 'PINVWOCTN' " + query + " ORDER BY InvID DESC").ToList();
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT * From Brands order by id desc").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult ProductionCreate(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            PurMaster.InvID = _context.Database.SqlQuery<int>("Select ISNULL(Max(inv_id),0)+1  from ProductionMaster_new").FirstOrDefault();
            
            PurMaster.Rtotal = _context.Database.SqlQuery<int>("select id FROm Brands where id = " + ID + "").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list_s = _context.Database.SqlQuery<stockquery>("SELECT ID,((OpeningStock + QTYIN) - QTYOUT) AS BAL, Name, CostPrice, PiecesBox FROM (SELECT Products.ID, Products.Name, Products.CostPrice, Products.Barcode, Products.PiecesBox, Products.OpeningStock, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM PurDetails WHERE(ItemID = Products.ID)) AS QTYIN, (SELECT ISNULL(SUM(Qty), 0) AS Expr1 FROM ProductionDetail WHERE(pid = Products.ID )) AS QTYOUT FROM Products WHERE type=1) AS derivedtbl_1").ToList();
            //  PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWOCTN");
            //  var p_detail = _context.Database.SqlQuery<PurDetail>("SELECT * from PurDetails where InvID IN (" + f_invids + ") and InvType = 'PINVWOCTN'").ToList();
            var p_detail = _context.Database.SqlQuery<Product>("SELECT * from Products  where BusinessUnit=" + Bunit + " order by name asc").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                Store = Store,
                batch_no = batch_no,
                pro_list_s = pro_list_s,
                pro_list1 = p_detail,
                // pro_list1 = pro_list11,
                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                //  p_detail = p_detail,
                // p_detail = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult ProductionSave(int[] item_id, string[] item_name, decimal[] sp,decimal[] sp_euro,decimal[] tot_ne_euro, int[] qty, decimal[] n_total, decimal[] sp_ne, int[] party, int[] qty_ne, decimal[] tot_ne, decimal[] tot_leng, string[] item_name_new, int[] p_id, PurMaster PurMaster)
        {
            for (int i = 0; i < item_id.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [ProductionDetail_new] (party,inv_id,date,b_no,sp,qty,item_name,pid,total,b_unit) VALUES (" + party[i] + "," + PurMaster.InvID + ",'" + DateTime.Now.ToShortDateString() + "'," + PurMaster.Rtotal + "," + sp[i] + "," + qty[i] + ",'" + item_name[i] + "'," + item_id[i] + "," + n_total[i] + ",'"+ Session["BusinessUnit"] + "')");
            }
            for (int j = 0; j < p_id.Count(); j++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [Production_finished_new] (tot_ne_euro,sp_euro,inv_id,date,b_no,sp,qty,item_name,p_id,total,tot_leng,b_unit) VALUES (" + tot_ne_euro[j] + "," + sp_euro [j]+ "," + PurMaster.InvID + ",'" + DateTime.Now.ToShortDateString() + "'," + PurMaster.Rtotal + "," + sp_ne[j] + "," + qty_ne[j] + ",'" + item_name_new[j] + "'," + p_id[j] + "," + tot_ne[j] + "," + tot_leng[j] + ",'" + Session["BusinessUnit"] + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO [ProductionMaster_new] (net_total_euro,freight_charges,store,inv_id,SupplierId,pur_leng,pro_leng,yelid,batch_no,s_date,e_date,NetAmount,Labour_cost,other_cost,total_cost,net_final,gorss_total,b_unit) VALUES (" + Request["net_total_euro"] + ",'" + Request["freight_charges"] +"'," + PurMaster.Store + "," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + Request["pur_leng"] + "," + Request["pro_leng"] + "," + Request["yelid"] + "," + PurMaster.Rtotal + ",'" + PurMaster.Date + "','" + PurMaster.Phone + "'," + Request["net_total"] + "," + PurMaster.CargoCharges + "," + PurMaster.DiscountAmount + "," + PurMaster.Total + ",0," + PurMaster.GrandTotal + ",'" + Session["BusinessUnit"] + "')");
            return RedirectToAction("Purchases");
        }
        public ActionResult Index()
        {
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Database.SqlQuery<PurchaseQuery>("SELECT (Select name From Brands where id=ProductionMaster_new.batch_no) as BatchNo,yelid,NetAmount,net_final as BTotal,inv_id as InvID,s_date as Date,e_date as Phone,batch_no as Rtotal, labour_cost as CargoCharges,other_cost as DiscountAmount,total_cost as Total,gorss_total as GrandTotal from [ProductionMaster_new] where b_unit ='" + Bunit + "'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                list = list,
            };
            return View(PurInvoiceVM);
        }
        public ActionResult Edit(int? ID, PurMaster PurMaster, PurDetail PurDetail, TransactionDetail TransactionDetail)
        {
            //var Ids = _context.Database.SqlQuery<PurMaster>("SELECT * from [ProductionMaster_new] where batch_no = " + ID + "").ToList();
            //string result = "";
            //foreach (var item in Ids)
            //{
            //    result += item.InvID + ",";
            //}
            //string f_invids = result.TrimEnd(',');

            // decimal qty = _context.Database.SqlQuery<decimal>("select SUM(qty*PiecesBox) as qty FROm PurDetails where InvID IN (" + f_invids + ") and  InvType = 'PINVWOCTN'").FirstOrDefault();
            //int p_box = _context.Database.SqlQuery<int>("select SUM(PiecesBox) as PiecesBox FROm PurDetails where InvID IN (" + f_invids + ") and  InvType = 'PINVWOCTN'").FirstOrDefault();
            PurMaster.InvID = _context.Database.SqlQuery<int>("select inv_id FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.NetAmount = _context.Database.SqlQuery<decimal>("select SUM(NetAmount) as NetAmount FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.CargoCharges = _context.Database.SqlQuery<decimal>("select SUM(Labour_cost) as NetAmount FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.DiscountAmount = _context.Database.SqlQuery<decimal>("select SUM(Other_cost) as DiscountAmount FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.Total = _context.Database.SqlQuery<decimal>("select SUM(total_cost) as Total FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.GrandTotal = _context.Database.SqlQuery<decimal>("select SUM(gorss_total) as GrandTotal FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.SupplierId = _context.Database.SqlQuery<int>("select SupplierId FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.Store = _context.Database.SqlQuery<int>("select Store FROm [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.NetFinal = _context.Database.SqlQuery<decimal>("select NetAmount From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.Purlength = _context.Database.SqlQuery<decimal>("select pur_leng From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.ProLength = _context.Database.SqlQuery<decimal>("select pro_leng From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.Yeild = _context.Database.SqlQuery<decimal>("select yelid From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.freight_charges = _context.Database.SqlQuery<decimal>("select freight_charges From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            ViewBag.net_total_euro = _context.Database.SqlQuery<decimal>("select net_total_euro From [ProductionMaster_new] where inv_id = " + ID + "").FirstOrDefault();
            PurMaster.Rtotal = _context.Database.SqlQuery<int>("select batch_no as Rtotal FROm [ProductionMaster_new] where inv_id = " + ID + " ").FirstOrDefault();
            PurMaster.Phone = _context.Database.SqlQuery<string>("select e_date as Phone  FROm [ProductionMaster_new] where inv_id = " + ID + " ").FirstOrDefault();
            PurMaster.Date = _context.Database.SqlQuery<string>("select s_date as Date FROm [ProductionMaster_new] where inv_id = " + ID + " ").FirstOrDefault();
            // ViewBag.PurchaseLength = qty;
            // PurMaster.InvID = _context.Database.SqlQuery<int>("select ISNULL(Max(InvID),0)+1 from PurMasters where InvID IN " + f_invids + " and  InvType = 'PINVWOCTN'").FirstOrDefault();
            var Bunit = Convert.ToString(Session["BusinessUnit"]);
            var Bunitdec = Convert.ToDecimal(Session["BusinessUnit"]);
            var Store = _context.Store.Where(z => z.b_unit == Bunit).ToList();
            var sup_list = _context.Supplier.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var emp_list = _context.Employee.Where(z => z.BusinessUnit == Bunitdec).ToList();
            var batch_no = _context.Brands.Where(z => z.b_unit == Bunit).ToList();
            var pro_list = _context.Database.SqlQuery<Product>("SELECT * from Products where type=3 and BusinessUnit='" + Bunit + "'").ToList();
            //  PurMaster = _context.PurMaster.SingleOrDefault(c => c.InvID == ID && c.InvType == "PINVWOCTN");
            var p_details = _context.Database.SqlQuery<PurDetailQuery>("SELECT party,(Select name From suppliers where accountno=ProductionDetail_new.party) as Suppname,products.PiecesBox,inv_id as InvId,sp,qty,item_name as ItemName,pid as ItemID,total as NetTotal from  ProductionDetail_new Inner JOIN products ON ProductionDetail_new.pid = products.ID where inv_id = " + ID + "").ToList();
            var pro_list_fin = _context.Database.SqlQuery<PurFinQuery>("SELECT tot_ne_euro,sp_euro,products.PiecesBox,products.PiecesBox,p_id,item_name,SP,qty,total,tot_leng from Production_finished_new Inner JOIN products ON Production_finished_new.p_id = products.ID where inv_id = " + ID + " and b_unit='"+ Bunit + "'").ToList();
            var PurInvoiceVM = new PurInvoiceVM
            {
                Store = Store,
                batch_no = batch_no,
                pro_list_fin = pro_list_fin,
                PurDetail = PurDetail,
                PurMaster = PurMaster,
                TransactionDetail = TransactionDetail,
                sup_list = sup_list,
                emp_list = emp_list,
                p_details = p_details,
                pro_list = pro_list
            };
            return View(PurInvoiceVM);
        }
        [HttpPost]
        public ActionResult ProductionUpdate(int[] item_id, string[] item_name, decimal[] sp,decimal[] sp_euro, decimal[] tot_ne_euro, int[] qty, decimal[] n_total, decimal[] sp_ne, int[] party, int[] qty_ne, decimal[] tot_ne, decimal[] tot_leng, string[] item_name_new, int[] p_id, PurMaster PurMaster)
        {
            _context.Database.ExecuteSqlCommand("Delete From [ProductionDetail_new] where inv_id=" + PurMaster.InvID + "");
            _context.Database.ExecuteSqlCommand("Delete From [Production_finished_new] where inv_id =" + PurMaster.InvID + "");
            _context.Database.ExecuteSqlCommand("Delete From [ProductionMaster_new] where inv_id =" + PurMaster.InvID + "");
            for (int i = 0; i < item_id.Count(); i++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [ProductionDetail_new] (party,inv_id,date,b_no,sp,qty,item_name,pid,total,b_unit) VALUES (" + party[i] + "," + PurMaster.InvID + ",'" + DateTime.Now.ToShortDateString() + "'," + PurMaster.Rtotal + "," + sp[i] + "," + qty[i] + ",'" + item_name[i] + "'," + item_id[i] + "," + n_total[i] + ",'" + Session["BusinessUnit"] + "')");
            }
            for (int j = 0; j < p_id.Count(); j++)
            {
                _context.Database.ExecuteSqlCommand("INSERT INTO [Production_finished_new] (tot_ne_euro,sp_euro,inv_id,date,b_no,sp,qty,item_name,p_id,total,tot_leng,b_unit) VALUES (" + tot_ne_euro[j] + "," + sp_euro[j] + "," + PurMaster.InvID + ",'" + DateTime.Now.ToShortDateString() + "'," + PurMaster.Rtotal + "," + sp_ne[j] + "," + qty_ne[j] + ",'" + item_name_new[j] + "'," + p_id[j] + "," + tot_ne[j] + "," + tot_leng[j] + ",'" + Session["BusinessUnit"] + "')");
            }
            _context.Database.ExecuteSqlCommand("INSERT INTO [ProductionMaster_new] (net_total_euro,freight_charges,store,inv_id,SupplierId,pur_leng,pro_leng,yelid,batch_no,s_date,e_date,NetAmount,Labour_cost,other_cost,total_cost,net_final,gorss_total,b_unit) VALUES (" + Request["net_total_euro"] + ",'" + Request["freight_charges"] + "'," + PurMaster.Store + "," + PurMaster.InvID + "," + PurMaster.SupplierId + "," + Request["pur_leng"] + "," + Request["pro_leng"] + "," + Request["yelid"] + "," + PurMaster.Rtotal + ",'" + PurMaster.Date + "','" + PurMaster.Phone + "'," + Request["net_total"] + "," + PurMaster.CargoCharges + "," + PurMaster.DiscountAmount + "," + PurMaster.Total + ",0," + PurMaster.GrandTotal + ",'" + Session["BusinessUnit"] + "')");
            return RedirectToAction("Index");
        }
        [HttpGet]
        public JsonResult GetStockTable(string value)
        {
            var allow_list = _context.Database.SqlQuery<stockquery>("SELECT ItemID, item_name, QTYIN, Name, AccountNo, QTYOUT, CostPrice, PiecesBox FROM(SELECT ItemID, ItemName AS item_name, SUM(Qty) AS QTYIN, Name, AccountNo, (SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductionDetail_new WHERE (pid = derivedtbl_1.ItemID) AND (party = derivedtbl_1.AccountNo)) AS QTYOUT, (SELECT ISNULL(CostPrice, 0) AS Expr1 FROM dbo.Products WHERE (ID = derivedtbl_1.ItemID)) AS CostPrice, (SELECT PiecesBox FROM dbo.Products AS Products_2 WHERE (ID = derivedtbl_1.ItemID)) AS PiecesBox FROM (SELECT GatepassDetail.ItemID, GatepassDetail.ItemName, GatepassDetail.Qty, dbo.Suppliers.Name, dbo.Suppliers.AccountNo FROM GatepassDetail INNER JOIN GatepassMaster ON GatepassDetail.InvId = GatepassMaster.invid INNER JOIN dbo.Suppliers ON GatepassMaster.supid = dbo.Suppliers.AccountNo INNER JOIN dbo.Products AS Products_1 ON GatepassDetail.ItemID = Products_1.ID WHERE (Products_1.type = 1) OR (Products_1.type = 3)) AS derivedtbl_1 WHERE (AccountNo = "+value+") GROUP BY ItemID, ItemName, Name, AccountNo) AS derivedtbl_2 WHERE (QTYIN - QTYOUT > 0)").ToList();
            return Json(allow_list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Action(string value,string parti)
        {
            var getgrosspackage = _context.Database.SqlQuery<stockquery>("SELECT ItemID, item_name, QTYIN - QTYOUT As Bal,QTYIN, Name, AccountNo, QTYOUT, CostPrice, PiecesBox FROM(SELECT ItemID, ItemName AS item_name, SUM(Qty) AS QTYIN, Name, AccountNo,(SELECT ISNULL(SUM(qty), 0) AS Expr1 FROM ProductionDetail_new WHERE (pid = derivedtbl_1.ItemID) AND (party = derivedtbl_1.AccountNo)) AS QTYOUT, (SELECT ISNULL(CostPrice, 0) AS Expr1 FROM dbo.Products WHERE (ID = derivedtbl_1.ItemID)) AS CostPrice, (SELECT PiecesBox FROM dbo.Products AS Products_2 WHERE (ID = derivedtbl_1.ItemID)) AS PiecesBox FROM (SELECT GatepassDetail.ItemID, GatepassDetail.ItemName, GatepassDetail.Qty, dbo.Suppliers.Name, dbo.Suppliers.AccountNo FROM GatepassDetail INNER JOIN GatepassMaster ON GatepassDetail.InvId = GatepassMaster.invid INNER JOIN dbo.Suppliers ON GatepassMaster.supid = dbo.Suppliers.AccountNo INNER JOIN dbo.Products AS Products_1 ON GatepassDetail.ItemID = Products_1.ID WHERE (Products_1.type = 1) OR (Products_1.type = 3)) AS derivedtbl_1 where ItemID = "+value+" and AccountNo= "+parti+" GROUP BY ItemID, ItemName, Name, AccountNo) AS derivedtbl_2 WHERE (QTYIN - QTYOUT > 0)").ToList();
            return Json(getgrosspackage, JsonRequestBehavior.AllowGet);
        }
    }
}