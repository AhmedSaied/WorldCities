﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities.Data;
using WorldCities.Data.Models;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
//using System.Reflection;

namespace WorldCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        //public ILogger<CitiesController> _logger;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        [Route("{pageIndex?}/{pageSize?}/{sortColumn?}/{sortOrder?}/{filterColumn?}/{filterQuery?}")]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
            int pageIndex=0, int pageSize=10, string sortColumn=null,
            string sortOrder=null, string filterColumn=null, string filterQuery=null)
        {

            return await ApiResult<CityDTO>.CreateAsync(_context.Cities
                .Include(a=>a.Country)
                .Select(c=>new CityDTO
            {
                Id=c.Id,
                Name=c.Name,
                Lat=c.Lat,
                Lon=c.Lon,
                CountryId= c.CountryId,
                CountryName=c.Country.Name
            }), pageIndex, pageSize, 
                sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        // DELETE: api/Cities/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<bool> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return false;
            }

            _context.Cities.Remove(city);
            var executed=await _context.SaveChangesAsync();

            return executed > 0;
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("IsDupeCity")]
        public bool IsDupeCity(City city)
        {
            return _context.Cities.Any(c => c.Name == city.Name
                    && c.Lat == city.Lat && c.Lon == city.Lon
                    && c.CountryId == city.CountryId && c.Id != city.Id);
        }
    }
}
