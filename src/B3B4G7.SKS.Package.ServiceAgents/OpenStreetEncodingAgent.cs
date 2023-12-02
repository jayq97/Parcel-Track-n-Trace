using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.ServiceAgents.Interfaces;
using Microsoft.Extensions.Logging;
using B3B4G7.SKS.Package.ServiceAgents.Exceptions;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace B3B4G7.SKS.Package.ServiceAgents
{
    public class OpenStreetEncodingAgent : IGeoEncodingAgent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OpenStreetEncodingAgent> _logger;

        public OpenStreetEncodingAgent(IHttpClientFactory httpClientFactory, ILogger<OpenStreetEncodingAgent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<GeoCoordinate> EncodeAddress(Recipient recipient)
        {
            var httpClient = _httpClientFactory.CreateClient();
            AddUserAgent(httpClient);

            string url = $"https://nominatim.openstreetmap.org/search?" +
                $"street={recipient.Street}&" +
                $"city={recipient.City}&" +
                $"country={recipient.Country}&" +
                $"postalcode={recipient.PostalCode}&" +
                $"format=json";

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            JArray array = JArray.Parse(result);

            if (array.Count <= 0)
                throw new PersonAddressNotFoundInAPIException("Address does not exist");

            JObject first = (JObject)array.First();
            string lat = first["lat"].ToString();
            string lon = first["lon"].ToString();
            return new GeoCoordinate { Lat = double.Parse(lat, CultureInfo.InvariantCulture), Lon = double.Parse(lon, CultureInfo.InvariantCulture) };
        }
        private static void AddUserAgent(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("f1ana.Nominatim.API", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
        }
    }
}