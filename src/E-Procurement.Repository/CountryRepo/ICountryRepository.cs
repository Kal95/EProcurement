using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.CountryRepo
{
    public interface ICountryRepository : IDependencyRegister
    {
        IEnumerable<Country> GetCountries();
        bool CreateCountry(string StateName, string UserId, out string Message);

        bool UpdateCountry(int Id, string StateName, bool IsActive, string UserId, out string Message);
    }
}
