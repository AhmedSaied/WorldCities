using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data.Models;
using Microsoft.AspNetCore.Hosting;
using System.Security;
using System.IO;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace WorldCities.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<ActionResult> Import()
        {
            if (!_env.IsDevelopment())
                throw new SecurityException("Not Allowed");

            var path = Path.Combine(_env.ContentRootPath, "Data\\Source\\worldcities.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);
            

            //get first worksheet
            var worksheet = excelPackage.Workbook.Worksheets[0];

            //define how many rows we want to process
            var nEndRow = worksheet.Dimension.End.Row;

            //initialize the record counters
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            var countriesByName = _context.Countries.AsNoTracking().ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);

            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();

                //skip this country if it already exists in the database
                if (countriesByName.ContainsKey(countryName))
                    continue;

                //create the country entity and fill it with xlsx data
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                await _context.Countries.AddAsync(country);

                countriesByName.Add(countryName, country);

                //increment the counter
                numberOfCountriesAdded++;
            }
            if (numberOfCountriesAdded > 0)
                await _context.SaveChangesAsync();

            var cities = _context.Cities.AsNoTracking().ToDictionary(a => (a.Name,a.Lat,a.Lon,a.CountryId));

            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                var countryId = countriesByName[countryName].Id;

                //skip this city if it already exists in the database
                if (cities.ContainsKey((name,lat,lon,countryId)))
                    continue;

                //create the country entity and fill it with xlsx data
                var city = new City
                {
                    Name = name,
                    Name_ASCII = nameAscii,
                    Lat = lat,
                    Lon=lon,
                    CountryId=countryId
                };

                await _context.Cities.AddAsync(city);

                cities.Add((name, lat, lon, countryId),city);

                //increment the counter
                numberOfCitiesAdded++;
            }
            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countires = numberOfCountriesAdded
            });
        }
    }
}
