using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Gatways.Models;
using Newtonsoft.Json;

namespace Gatway
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";
        private string ApiKey = "5ffdbffc198af87917f4604052b5b138"; 

        public WeatherService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BaseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<WeatherData> GetWeatherAsync(string city)
        {
            try
            {
                string encodedCityName = WebUtility.UrlEncode(city);
                string endpoint = $"?q={encodedCityName}&appid={ApiKey}&units=metric";

                HttpResponseMessage response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse);

                    WeatherData weatherData = new WeatherData
                    {
                        City = jsonData.name,
                        Temperature = jsonData.main.temp,
                        Description = jsonData.weather[0].main,
                        Humidity = jsonData.main.humidity,
                        WindSpeed = jsonData.wind.speed
                        // Map other properties as needed
                    };

                    return weatherData;
                }
                else
                {
                    Console.WriteLine($"Unsuccessful API response. Status code: {response.StatusCode}");

                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response content: {errorResponse}");

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during API call: {ex.Message}");
                return null;
            }
        }
    }
}
