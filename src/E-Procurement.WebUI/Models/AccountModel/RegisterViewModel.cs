using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.AccountModel
{
    public class RegisterViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Unit { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

//[Required]
        public string Username { get; set; }
        
        //public string Password { get; set; }

        //public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
