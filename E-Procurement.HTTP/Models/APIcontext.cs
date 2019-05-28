using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace E_Procurement.HTTP.Models
{
    public class APIcontext :DbContext
    {
        public APIcontext()
        {
        }

        public APIcontext(DbContextOptions<APIcontext> options)
                : base(options)
            {
            }

            public DbSet<Item> Items { get; set; }
            public DbSet<ItemCategory> ItemsCategory { get; set; }

    }
}
