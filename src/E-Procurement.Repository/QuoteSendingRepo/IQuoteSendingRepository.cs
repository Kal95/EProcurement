﻿using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RequisitionModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.QuoteSendingRepo
{
    public interface IQuoteSendingRepository : IDependencyRegister
    {
        Task<IEnumerable<RFQGenerationModel>> GetQuoteAsync();
        Task<RFQGenerationModel> GetQuoteDetailsAsync(int RFQId);
        List<RFQDetails> GetRfqDetails();
        bool UpdateQuote(int[] Id, decimal[] quotedPrice, decimal[] quotedAmount, RequisitionModel model, out string Message);
       // bool UpdateQuote(int[] Id, decimal[] quotedPrice, decimal[] AgreedAmount, out string Message);
    }
}
