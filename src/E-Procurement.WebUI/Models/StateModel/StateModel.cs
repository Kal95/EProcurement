using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.StateModel
{
    public class StateModel
    {
        [Required]
        public string StateName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }

    }

}
