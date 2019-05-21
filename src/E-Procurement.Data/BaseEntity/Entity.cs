using System;

namespace E_Procurement.Data.BaseEntity
{
    /// <summary>
    /// Base class for all entity
    /// </summary>
    public class Entity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }= DateTime.Now;
        public DateTime? LastDateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
