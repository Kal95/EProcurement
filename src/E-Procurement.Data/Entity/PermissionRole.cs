using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
  public  class PermissionRole : BaseEntity.Entity
    {
        //public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
  
    }
}
