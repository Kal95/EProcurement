using E_Procurement.Repository.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.Interface
{
    public interface IConvertViewToPDF : IDependencyRegister
    {
        Task<bool> CreateRFQPDF(RFQGenerationModel rfqGenerationModel, string emailMessage);
        Task<bool> CreatePOPDF(RFQGenerationModel rfqGenerationModel);
    }
}
