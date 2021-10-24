using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities.Data;
using WorldCities.Data.Models;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/countries
        [HttpGet]
        [Route("{pageIndex?}/{pageSize?}/{sortColumn?}/{sortOrder?}/{filterColumn?}/{filterQuery?}")]
        public async Task<ActionResult<ApiResult<Country>>> GetCountries(
            int pageIndex = 0, int pageSize = 10, string sortColumn = null,
            string sortOrder = null, string filterColumn = null, string filterQuery = null)
        {
            return await ApiResult<Country>.CreateAsync(_context.Countries.Select(c => new Country
            {
                Id = c.Id,
                Name = c.Name,
                ISO2 = c.ISO2,
                ISO3 = c.ISO3,
                TotCities = c.Cities.Count
            }), pageIndex, pageSize,
                sortColumn, sortOrder, filterColumn, filterQuery);
            /*return await ApiResult<CountryDTO>.CreateAsync(_context.Countries.Select(c=>new CountryDTO
            {
                Id=c.Id,
                Name=c.Name,
                ISO2=c.ISO2,
                ISO3=c.ISO3,
                TotCities = c.Cities.Count
            }), pageIndex, pageSize,
                sortColumn, sortOrder, filterColumn, filterQuery);*/
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        /*[HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(int countryId, string fieldName, string fieldValue)
        {
            /*return fieldName switch
            {
                "name" => _context.Countries.Any(
                              c => c.Name == fieldValue && c.Id != countryId),
                "iso2" => _context.Countries.Any(
                              c => c.ISO2 == fieldValue && c.Id != countryId),
                "iso3" => _context.Countries.Any(
                              c => c.ISO3 == fieldValue && c.Id != countryId),
                _ => false,
            };/

            return ApiResult<Country>.IsValidProperty(fieldName, true) ?
                    _context.Countries.Any(string.Format("{0} == @0 && Id != @1",
                    fieldName), fieldValue, countryId) : false;
        }*/

        [HttpPost]
        [Route("IsDupeField/{countryId}/{fieldName}/{fieldValue}")]
        public bool IsDupeField(int countryId,string fieldName,
            string fieldValue)
        {

            // Dynamic approach (using System.Linq.Dynamic.Core)
            return (ApiResult<Country>.IsValidProperty(fieldName, true))
                ? _context.Countries.Any(
                    string.Format("{0} == @0 && Id != @1", fieldName),
                    fieldValue,
                    countryId)
                : false;
        }
    }
}
