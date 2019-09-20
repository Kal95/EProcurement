using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.Repository.ApprovalRepo
{
    public class ApprovalTypeModel
    {
        [Required]
        public string ApprovalTypeName { get; set; }
        public int ApprovalTypeId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy{ get; set; }
        public string UpdatedBy { get; set; }

    }

}
