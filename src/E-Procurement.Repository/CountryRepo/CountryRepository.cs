using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.CountryModel;

namespace E_Procurement.Repository.CountryRepo
{
    public class CountryRepository: ICountryRepository
    {
        private readonly EProcurementContext _context;


        public CountryRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateCountry(CountryModel model, out string Message)
        {
            var confirm = _context.Countries.Where(x => x.CountryName == model.CountryName).Count();

            Country country = new Country();

            if (confirm == 0)
            {

                country.CountryName = model.CountryName;

                country.IsActive = true;

                country.CreatedBy = model.CreatedBy;

                country.DateCreated = DateTime.Now;

                _context.Add(country);

                _context.SaveChanges();

                Message = "Country created successfully";

                return true;
            }
            else
            {
                Message = "Country already exist";

                return false;
            }

        }

        public bool UpdateCountry(CountryModel model, out string Message)
        {

            var confirm = _context.Countries.Where(x => x.CountryName == model.CountryName && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.Countries.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Country exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.CountryName = model.CountryName;

                oldEntry.IsActive = model.IsActive;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                _context.SaveChanges();

                Message = "Country updated successfully";

                return true;
            }
            else
            {
                Message = "Country already exist";

                return false;
            }

        }

        public IEnumerable<Country> GetCountries()
        {
            return _context.Countries.OrderByDescending(u => u.Id).ToList();
        }
    }
}

