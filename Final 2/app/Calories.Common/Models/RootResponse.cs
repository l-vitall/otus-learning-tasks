namespace Calories.Common.Models
{
    public class RootResponse : Resource
    {
        public Link CaloriesRecords { get; set; }
        public Link Users { get; set; }

        public Form Token { get; set; }
    }
}