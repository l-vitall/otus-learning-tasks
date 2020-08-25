namespace Calories.Common
{
    public class AppSettings
    {
        public bool UseInMemoryDatabase { get; set; }
        public string Secret { get; set; }
        
        public string CaloriesRecordsDbContext { get; set; }
    }
}