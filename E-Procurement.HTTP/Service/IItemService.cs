using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.HTTP.Models;

namespace E_Procurement.HTTP.Service
{
    public interface IItemService
    {
        Item GetItems(int Id);
    }
}
