using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.CountryModel;

namespace E_Procurement.Repository.CountryRepo
{
    public interface ICountryRepository : IDependencyRegister
    {
        IEnumerable<Country> GetCountries();
        bool CreateCountry(CountryModel model, out string Message);

        bool UpdateCountry(CountryModel model, out string Message);
    }
}
