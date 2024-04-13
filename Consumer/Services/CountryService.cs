using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Consumer.Data
{
    public class CountryService
    {
        private readonly HttpClient _httpClient;

        public CountryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("countriesconsumer");
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync(CountryFilter filter)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "name", filter.Name },
                { "limit", filter.Limit.ToString() },
                { "sortOrder", filter.SortOrder.ToString() },
                { "population", filter.Population.ToString() }
            };

            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("api/countries", queryParams));

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<Country>>(jsonResponse);

                return countries;
            }

            throw new BadHttpRequestException("The request has been failed", 400);
        }

        public async Task AddCurrencyAsync(string countryName, Currency currency)
        {
            var payload = JsonContent.Create(currency);
            var response = await _httpClient.PostAsync($"countries/currency/{countryName}", payload);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadHttpRequestException("The request has been failed", 400);
            }
        }
    }
}