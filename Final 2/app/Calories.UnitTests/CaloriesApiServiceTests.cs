using Calories.Common.Models;
using Calories.Records.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Calories.UnitTests
{
    class TestOptions : IOptions<CaloriesApiSettings>
    {
        private readonly CaloriesApiSettings _value = new CaloriesApiSettings(){Uri = "https://api.nutritionix.com/v1_1/search/{0}?results=0%3A20&fields=nf_calories&appId=f1187872&appKey=9c87e077290c2ef813b5518613ce8962"};

        public CaloriesApiSettings Value => _value;
    }
    
    public class CaloriesApiServiceTests
    {
        private CaloriesApiService _apiService;

        public CaloriesApiServiceTests()
        {
            _apiService = new CaloriesApiService(new TestOptions());
        }
        
        [Theory]
        [InlineData("Bigmac")]
        [InlineData("Milk")]
        [InlineData("Hamburger")]
        public void TryGetCalories_ValidInput(string text)
        {
            var result = _apiService.TryGetCalories(text).GetAwaiter().GetResult();
            
            Assert.True(result > 0);
        }
        
        [Theory]
        [InlineData("Hamburger123321")]
        public void TryGetCalories_InvalidInput(string text)
        {
            var result = _apiService.TryGetCalories(text).GetAwaiter().GetResult();
            
            Assert.Equal(CaloriesApiService.DefaultValue, result);
        }
    }
}