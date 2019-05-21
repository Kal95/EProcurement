using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace E_Procurement.Data.Entity
{
    /// <summary>
    /// The use Entity
    /// </summary>
    public class User: IdentityUser<int>
    {
        //todo add more user properties here
        public virtual ICollection<IdentityUserRole<int>> Roles { get; set; } = new List<IdentityUserRole<int>>();

        ///// <summary>
        ///// Navigation property for the claims this user possesses.
        ///// </summary>
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; set; } = new List<IdentityUserClaim<int>>();

    }
}
