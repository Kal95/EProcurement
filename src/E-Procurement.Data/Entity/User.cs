using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace E_Procurement.Data.Entity
{
    /// <summary>
    /// The use Entity
    /// </summary>
    public class User: IdentityUser<int>
    {
        //todo add more user properties here
        

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Unit { get; set; }

        public virtual ICollection<IdentityUserRole<int>> Roles { get; set; } = new List<IdentityUserRole<int>>();

        ///// <summary>
        ///// Navigation property for the claims this user possesses.
        ///// </summary>
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; set; } = new List<IdentityUserClaim<int>>();
        [NotMapped]
        public string FullName
        {
            get
            {
                return this.LastName + " " + this.FirstName;
            }
        }

    }
}
