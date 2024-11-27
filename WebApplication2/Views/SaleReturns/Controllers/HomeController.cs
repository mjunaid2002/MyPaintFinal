using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.ViewModels;
using CRM.Models;
using WebApplication1.Models;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace WebApplication1.Controllers
{
    [SessionTimeout]
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["UserID"] = null;
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
         [HttpPost, ActionName("Login")]
        //[ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            int count = _context.Database.SqlQuery<int>("Select count(id) From  tbl_emp_auth where username='" + Request["u_name"] + "' and password ='" + Request["pass"] + "'").FirstOrDefault();
            //var login = _context.UserLogin.SingleOrDefault(c => c.UserName == UserLogin.UserName && c.Password == UserLogin.Password);
            if (count != 0)
            {
                Session["UserID"] =  _context.Database.SqlQuery<decimal>("Select empid From  tbl_emp_auth where username='" + Request["u_name"] + "' and password ='" + Request["pass"] + "'").FirstOrDefault();
                Session["tax_per"] =  _context.Database.SqlQuery<decimal>("Select strnperc From  tbl_setting").FirstOrDefault();
               // Session["tax_per"] = 17; 
                return RedirectToAction("Index", "Dashboard");

            }
            else
            {
                return RedirectToAction("Login", "Home", new { ac = "fail" });
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Forget(BusinessUnit businessUnit)
        {
            var LoginVM = new LoginVM
            {
                businessUnit = businessUnit,
            };
            return View(LoginVM);
        }
        [HttpPost]
        public ActionResult ForgetSubmit(BusinessUnit businessUnit)
        {
            Random r = new Random();
            int num = r.Next();
            var obj = _context.BusinessUnits.Where(a => a.Email.Equals(businessUnit.Email)).FirstOrDefault();
            if (obj != null)
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("protechiz.net@gmail.com");
                msg.To.Add(businessUnit.Email);
                msg.Subject = "Forget Password";
                msg.Body = "Kindly find your new password:'" + num + "'";
                //msg.Priority = MailPriority.High;
                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("protechiz.net@gmail.com", "@ZST#123@");
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                }

                _context.Database.ExecuteSqlCommand("update UserLogins set Password='" + num + "' where UserName='" + obj.Name + "'");
                ViewBag.Message = "Great ! We Have sent you four new password. Kindly check your registred email ";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }
    }
}