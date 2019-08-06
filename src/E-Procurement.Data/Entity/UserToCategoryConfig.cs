using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class UserToCategoryConfig: BaseEntity.Entity
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
