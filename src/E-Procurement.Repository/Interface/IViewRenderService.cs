using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.Repository.Interface
{
    public interface IViewRenderService : IDependencyRegister
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
