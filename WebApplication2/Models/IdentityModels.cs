using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CRM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication1.Models;
using WebApplication2.Models;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
       
        public DbSet<tbl_BatchRequest> tbl_BatchRequest { get; set; }
        public DbSet<SaleMasterReturn> SaleMasterReturn { get; set; }
        public DbSet<PurMasterReturn> PurMasterReturn { get; set; }
        public DbSet<Cargo> Cargo { get; set; }
        public DbSet<OpeningBalance> OpeningBalance { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<UserRole> tbl_UserRole { get; set; }
        public DbSet<MeasuringUnit> tbl_MeasuringUnit { get; set; }
        public DbSet<Ac_head> Ac_head { get; set; }
        public DbSet<Ac_main> Ac_main { get; set; }
        public DbSet<Ac_second> Ac_second { get; set; }
        public DbSet<AccountTitle> AccountTitle { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<BusinessUnit> BusinessUnits { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Emp_Department> Emp_Department { get; set; }
        public DbSet<FormsMenus> FormsMenus { get; set; }
        public DbSet<FormsSubMenus> FormsSubMenus { get; set; }
        public DbSet<FormsSuperMenus> FormsSuperMenus { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Provinces> Provinces { get; set; }
        public DbSet<PurDetail> PurDetail { get; set; }
        public DbSet<PurDetailDis> PurDetailDis { get; set; }
        public DbSet<PurMaster> PurMaster { get; set; }
        public DbSet<PurMasterDis> PurMasterDis { get; set; }
        public DbSet<SaleDetailDis> SaleDetailDis { get; set; }
        public DbSet<SaleDetail> SaleDetail { get; set; }
        public DbSet<GatepassMaster> GatepassMaster { get; set; }
        public DbSet<SaleMaster> SaleMaster { get; set; }
        public DbSet<SaleMasterDis> SaleMasterDis { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Town> Town { get; set; }
        public DbSet<TransactionDetail> TransactionDetail { get; set; }
        public DbSet<UserAccess> UserAccess { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
       
        public DbSet<Voucher> Voucher { get; set; }
        public DbSet<VoucherMaster> VoucherMaster { get; set; }
        public DbSet<ProductionOrder> ProductionOrder { get; set; }
        public DbSet<ProductionOrderDetail> ProductionOrderDetail { get; set; }
        public object GeneralQuery { get; internal set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}