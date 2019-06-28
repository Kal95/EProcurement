
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.Repository.Interface
{
    public interface ISMTPService : IDependencyRegister
    {
        Task SendEmailAsync(string email, string subject, string message, string attachedfiles);
        Task SendEmailTransactionAsync(string email, string subject, string message, string attachedfiles);
    }
}
