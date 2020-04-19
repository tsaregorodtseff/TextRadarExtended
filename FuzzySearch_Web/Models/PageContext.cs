using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FuzzySearchDemo.Models
{
    public class PageContext : DbContext
    {
        public DbSet<Page> Pages { get; set; }
    }

    //public class PageDbInitialaser : DropCreateDatabaseAlways<PageContext>
    //{
    //    protected override void Seed(PageContext context)
    //    {

    //        context.Pages.Add(new Page { Text = "Война и мир" });

    //        base.Seed(context);
    //    }
    //}
}