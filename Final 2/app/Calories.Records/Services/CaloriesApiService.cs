using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Calories.Common.Models;
using Microsoft.Extensions.Options;

namespace Calories.Records.Services
{
    public class CaloriesApiService : ICaloriesApiService
    {
        public const int DefaultValue = 0;
        
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _requestUri;
        private string _pattern = "\"nf_calories\":(.*?),";

        public CaloriesApiService(IOptions<CaloriesApiSettings> apiSetingsWrapper)
        {
            _requestUri = apiSetingsWrapper.Value.Uri;
        }
        
        public async Task<int> TryGetCalories(string text)
        {
            try
            {
                string requestUri = string.Format(_requestUri, Uri.EscapeDataString(text));
                var resultJson = await _httpClient.GetStringAsync(requestUri);

                foreach (Match match in Regex.Matches(resultJson, _pattern))
                {
                    if (match.Success && match.Groups.Count > 0)
                    {
                        var caloriesStr = match.Groups[1].Value;

                        if (float.TryParse(caloriesStr, out var result))
                            return (int)result;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return DefaultValue;
        }
    }
}