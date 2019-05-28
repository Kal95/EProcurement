﻿using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQDetails : BaseEntity.Entity
    {
        public int RFQId { get; set; }
        public int VendorId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int QuotedQuantity { get; set; }
        public int AgreedQuantity { get; set; }
        public decimal QuotedAmount { get; set; }
        public decimal AgreedAmount { get; set; }

    }
}