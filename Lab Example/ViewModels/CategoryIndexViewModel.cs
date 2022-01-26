using Lab_Example.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lab_Example.ViewModels
{
    public class CategoryIndexViewModel
    {

        //public IQueryable<Product> Products { get; set; }
        public IPagedList<Category> Categories { get; set; }

    }
}
