using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.CountryModel
{
    public class CountryModel
    {
        [Required]
        public string CountryName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
