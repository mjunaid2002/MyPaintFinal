using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.Models;
using WebApplication1.QueryViewModel;

namespace WebApplication1.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
         public Products products  { get; set; }
        public Categories categories { get; set; }
        public Attendance attendance { get; set; }
        public IEnumerable<Products> pro_list { get; set; }
        public IEnumerable<MianCategories> quality { get; set; }
        public IEnumerable<QualityCategories> QualityCategories { get; set; }
        public IEnumerable<Categories> Categories_list { get; set; }
        public IEnumerable<Product_Type> Product_Type_list { get; set; }
        public IEnumerable<MianCategories> listss { get; set; }
        public IEnumerable<ProductQuery> lp_lisr { get; set; }
        public IEnumerable<MeasuringUnit> MeasuringUnit { get; set; }
        public IEnumerable<stockquery> list { get; set; }
        public IEnumerable<Product> product_list { get; set; }
        public IEnumerable<Category> cat_list { get; set; }
        public IEnumerable<Brands> br_list { get; set; }
        public IEnumerable<Employee> emp_list { get; set; }
        public IEnumerable<ProductFinishedRegion> ProductFinishedRegion_list { get; set; }
        public ProductFinishedRegion ProductFinishedRegion { get; set; }
        public ProductPricingRegion ProductPricingRegion { get; set; }
        public IEnumerable<QueryViewModel.Region> Region_list { get; set; }
        public IEnumerable<QueryViewModel.Branch> branch_list { get; set; }
        public IEnumerable<ProductPricingRegion> ProductPricingRegion_list { get; set; }
    
    }
}