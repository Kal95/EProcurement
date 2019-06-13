using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class EmailSentLog : BaseEntity.Entity
    {
        public string RecipientEmail { get; set; }

        [DataType(DataType.Text)]
        public string EmailContent { get; set; }
        public string Status { get; set; }
    }
}
