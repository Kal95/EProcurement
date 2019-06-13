using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.EmailLogRepo
{
   public interface IEmailSentLog : IDependencyRegister
    {
        Task<bool> LogEmailAsync(EmailSentLog emailLog);

        Task<bool> UpdateEmailAsync(EmailSentLog emailLog);

        Task<EmailSentLog> GetEmailSentByIdAsync(int Id);

        Task<IEnumerable<EmailSentLog>> GetEmailSentLogAsync();
    }
}
