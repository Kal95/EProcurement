using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.PermissionModel
{
    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class RolePermissionViewModel
    {

        public int RoleId { get; set; }
        public bool SelectedPermission { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
}
