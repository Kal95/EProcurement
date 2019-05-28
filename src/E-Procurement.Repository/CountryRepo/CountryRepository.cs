using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;

namespace E_Procurement.Repository.CountryRepo
{
    public class CountryRepository: ICountryRepository
    {
        private readonly EProcurementContext _context;


        public CountryRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateCountry(string CountryName, string UserId, out string Message)
        {
            var confirm = _context.Countries.Where(x => x.CountryName == CountryName).Count();

            Country country = new Country();

            if (confirm == 0)
            {

                country.CountryName = CountryName;

                country.IsActive = true;

                country.CreatedBy = UserId;

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

        public bool UpdateCountry(int Id, string CountryName, bool IsActive, string UserId, out string Message)
        {

            var confirm = _context.Countries.Where(x => x.CountryName == CountryName && x.IsActive == IsActive).Count();

            var oldEntry = _context.Countries.Where(u => u.Id == Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Country exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.CountryName = CountryName;

                oldEntry.IsActive = IsActive;

                oldEntry.UpdatedBy = UserId;

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

