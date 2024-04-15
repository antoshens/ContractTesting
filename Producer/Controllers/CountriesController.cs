using ContractTesting_Producer.Models;
using ContractTesting_Producer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractTesting_Producer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private const int POPULATION_MULTIPLIER = 1000000;

        private readonly IHttpClientService _httpClient;

        public CountriesController(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries(string name, int limit, string sortOrder, int population)
        {
            try
            {
                var countries = await _httpClient.SendGetAsync<List<Country>>("v3.1/all");

                var response = FilterByName(countries, name);
                response = FilterByPopulation(response, population);
                response = SortCountries(response, sortOrder);
                response = LimitRecords(response, limit);

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("Unable to load countries.");
            }
        }

        [HttpPost]
        [Route("currency/{countryName}")]
        public IActionResult AddNewCurrency(string countryName, Currency currency)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countryName))
                {
                    throw new ArgumentNullException(nameof(countryName));
                }

                if (countryName.Any(Char.IsDigit))
                {
                    throw new FormatException($"{nameof(countryName)} has the wrong format");
                }

                if (currency is null)
                {
                    throw new ArgumentNullException(nameof(currency));
                }

                return Ok("The currency has been added.");
            }
            catch (Exception)
            {
                return BadRequest("Unable to add new currency.");
            }
        }

        private IEnumerable<Country> FilterByName(IEnumerable<Country> countries, string filterStr)
        {
            if (string.IsNullOrWhiteSpace(filterStr))
            {
                return countries;
            }

            if (countries is null)
            {
                return null;
            }

            return countries.Where(c => c.Name.Common.Contains(filterStr));
        }

        private IEnumerable<Country> FilterByPopulation(IEnumerable<Country> countries, int maxPopulation)
        {
            if (maxPopulation <= 0)
            {
                return countries;
            }

            if (countries is null)
            {
                return null;
            }

            return countries.Where(c => c.Population < maxPopulation * POPULATION_MULTIPLIER);
        }

        private IEnumerable<Country> SortCountries(IEnumerable<Country> countries, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                return countries;
            }

            if (countries is null)
            {
                return null;
            }

            if (sortOrder.Equals("ascend", StringComparison.OrdinalIgnoreCase) || sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                return countries.OrderBy(c => c.Name.Common).AsEnumerable();
            }
            else if (sortOrder.Equals("descend", StringComparison.OrdinalIgnoreCase) || sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                return countries.OrderByDescending(c => c.Name.Common).AsEnumerable();
            }
            return countries;
        }

        private IEnumerable<Country> LimitRecords(IEnumerable<Country> countries, int recordsLimit)
        {
            if (recordsLimit <= 0)
            {
                return countries;
            }

            if (countries is null)
            {
                return null;
            }

            return countries.Take(recordsLimit);
        }
    }
}
