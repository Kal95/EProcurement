using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Procurement.Data.Entity
{
   public class Permission : BaseEntity.Entity
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Code { get; set; }
    }
}
