using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication2.Controllers
{
    [SessionTimeout]
    public class SettingsController : Controller
    {
        private ApplicationDbContext _context;

        public SettingsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Settings
        public ActionResult Index()
        {
            string bUNit = Convert.ToString(Session["BusinessUnit"]);
            var list = _context.Settings.SingleOrDefault(c => c.Bunit == bUNit);
            return View(list);
        }
        [HttpPost, ActionName("Index")]
        public ActionResult Save(int lang, Settings Settings, HttpPostedFileBase file, bool b_unit, bool CNameStatus, bool EmailStatus, bool PhoneStatus, bool TelephoneStatus, bool GstStatus, bool NtnStatus, bool SaleNoteStatus, bool LogoStatus)
        {
            string bUNit = Convert.ToString(Session["BusinessUnit"]);
            var login = _context.Settings.SingleOrDefault(c => c.Bunit == bUNit);
            if (login == null)
            {
                Random r = new Random();
                int num = r.Next();
                string ImageName = "";
                string physicalPath;
                if (file != null)
                {
                    ImageName = System.IO.Path.GetFileName(file.FileName);
                    string img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file.SaveAs(physicalPath);
                    _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Settings](inv_status,[ID] ,[CompanyName] ,[Email] ,[Phone] ,[Telephone] ,[Address] ,[MImage] ,[Gst] ,[Ntn] ,[SaleNote] ,[CNameStatus] ,[GstStatus] ,[NtnStatus] ,[SaleNoteStatus] ,[LogoStatus] ,[EmailStatus] ,[PhoneStatus] ,[TelephoneStatus] ,[Lang] ,[Bunit_status] ,[Str].[Bunit]) VALUES ('" + Request["fort"] + "','" + Settings.CompanyName + "','" + Settings.Email + "','" + Settings.Phone + "','" + Settings.Telephone + "','" + Settings.Address + "','" + img + "','" + Settings.Gst + "','" + Settings.Ntn + "','" + Settings.SaleNote + "','" + Settings + "','" + Settings.CNameStatus + "','" + Settings.GstStatus + "','" + Settings.NtnStatus + "','" + Settings.SaleNoteStatus + "','" + Settings + "','" + Settings.LogoStatus + "','" + Settings.EmailStatus + "','" + Settings.PhoneStatus + "','" + Settings.TelephoneStatus + "'," + Settings.Lang + "," + Settings.Bunit_status + ",'" + Settings.Str + "','" + Session["BusinessUnit"] + "')");
                    //  _context.Database.ExecuteSqlCommand("UPDATE Settings set Str ='" + Settings.Str + "',Bunit_status ='" + b_unit + "',lang="+lang+",CompanyName = '" + Settings.CompanyName + "',Email = '" + Settings.Email + "',Phone = '" + Settings.Phone + "',Telephone = '" + Settings.Telephone + "',Address = '" + Settings.Address + "',MImage = '" + img + "',Gst = '" + Settings.Gst + "',Ntn = '" + Settings.Ntn + "',SaleNote = '" + Settings.SaleNote + "',CNameStatus = '" + CNameStatus + "',GstStatus = '" + GstStatus + "',NtnStatus = '" + NtnStatus + "',SaleNoteStatus = '" + SaleNoteStatus + "',LogoStatus = '" + LogoStatus + "',TelephoneStatus = '" + TelephoneStatus + "',EmailStatus = '" + EmailStatus + "',PhoneStatus = '" + PhoneStatus + "'");
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Settings](inv_status,[ID] ,[CompanyName] ,[Email] ,[Phone] ,[Telephone] ,[Address] ,[MImage] ,[Gst] ,[Ntn] ,[SaleNote] ,[CNameStatus] ,[GstStatus] ,[NtnStatus] ,[SaleNoteStatus] ,[LogoStatus] ,[EmailStatus] ,[PhoneStatus] ,[TelephoneStatus] ,[Lang] ,[Bunit_status] ,[Str].[Bunit]) VALUES ('" + Request["fort"] + "','" + Settings.CompanyName + "','" + Settings.Email + "','" + Settings.Phone + "','" + Settings.Telephone + "','" + Settings.Address + "','','" + Settings.Gst + "','" + Settings.Ntn + "','" + Settings.SaleNote + "','" + Settings + "','" + Settings.CNameStatus + "','" + Settings.GstStatus + "','" + Settings.NtnStatus + "','" + Settings.SaleNoteStatus + "','" + Settings + "','" + Settings.LogoStatus + "','" + Settings.EmailStatus + "','" + Settings.PhoneStatus + "','" + Settings.TelephoneStatus + "'," + Settings.Lang + "," + Settings.Bunit_status + ",'" + Settings.Str + "','" + Session["BusinessUnit"] + "')");
                }
            }
            else
            {
                Random r = new Random();
                int num = r.Next();
                string ImageName = "";
                string physicalPath;
                if (file != null)
                {
                    ImageName = System.IO.Path.GetFileName(file.FileName);
                    string img = num + ImageName;
                    physicalPath = Server.MapPath("~/Uploads/" + img);
                    file.SaveAs(physicalPath);
                    // _context.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Settings]([ID] ,[CompanyName] ,[Email] ,[Phone] ,[Telephone] ,[Address] ,[MImage] ,[Gst] ,[Ntn] ,[SaleNote] ,[CNameStatus] ,[GstStatus] ,[NtnStatus] ,[SaleNoteStatus] ,[LogoStatus] ,[EmailStatus] ,[PhoneStatus] ,[TelephoneStatus] ,[Lang] ,[Bunit_status] ,[Str].[Bunit]) VALUES ('" + Settings.CompanyName + "','" + Settings.Email + "','" + Settings.Phone + "','" + Settings.Telephone + "','" + Settings.Address + "','" + img + "','" + Settings.Gst + "','" + Settings.Ntn + "','" + Settings.SaleNote + "','" + Settings + "','" + Settings.CNameStatus + "','" + Settings.GstStatus + "','" + Settings.NtnStatus + "','" + Settings.SaleNoteStatus + "','" + Settings + "','" + Settings.LogoStatus + "','" + Settings.EmailStatus + "','" + Settings.PhoneStatus + "','" + Settings.TelephoneStatus + "'," + Settings.Lang + "," + Settings.Bunit_status + ",'" + Settings.Str + "','" + Session["BusinessUnit"] + "')");
                    _context.Database.ExecuteSqlCommand("UPDATE Settings set inv_status ='" + Request["fort"] + "',Str ='" + Settings.Str + "',Bunit_status ='" + b_unit + "',lang=" + lang + ",CompanyName = '" + Settings.CompanyName + "',Email = '" + Settings.Email + "',Phone = '" + Settings.Phone + "',Telephone = '" + Settings.Telephone + "',Address = '" + Settings.Address + "',MImage = '" + img + "',Gst = '" + Settings.Gst + "',Ntn = '" + Settings.Ntn + "',SaleNote = '" + Settings.SaleNote + "',CNameStatus = '" + CNameStatus + "',GstStatus = '" + GstStatus + "',NtnStatus = '" + NtnStatus + "',SaleNoteStatus = '" + SaleNoteStatus + "',LogoStatus = '" + LogoStatus + "',TelephoneStatus = '" + TelephoneStatus + "',EmailStatus = '" + EmailStatus + "',PhoneStatus = '" + PhoneStatus + "'");
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("UPDATE Settings set inv_status ='" + Request["fort"] + "',Str ='" + Settings.Str + "',Bunit_status ='" + b_unit + "',lang=" + lang + ",CompanyName = '" + Settings.CompanyName + "',Email = '" + Settings.Email + "',Phone = '" + Settings.Phone + "',Telephone = '" + Settings.Telephone + "',Address = '" + Settings.Address + "',MImage = '',Gst = '" + Settings.Gst + "',Ntn = '" + Settings.Ntn + "',SaleNote = '" + Settings.SaleNote + "',CNameStatus = '" + CNameStatus + "',GstStatus = '" + GstStatus + "',NtnStatus = '" + NtnStatus + "',SaleNoteStatus = '" + SaleNoteStatus + "',LogoStatus = '" + LogoStatus + "',TelephoneStatus = '" + TelephoneStatus + "',EmailStatus = '" + EmailStatus + "',PhoneStatus = '" + PhoneStatus + "'");
                }
            }
            Session["CompName"] = Settings.CompanyName;

            return RedirectToAction("Index");

        }
    }
}