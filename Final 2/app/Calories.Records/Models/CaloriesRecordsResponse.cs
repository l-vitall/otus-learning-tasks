using Calories.Common.Models;

namespace Calories.Records.Models
{
    public class CaloriesRecordsResponse : PagedCollection<CaloriesRecord>
    {
        public Form CaloriesRecordsQuery { get; set; }
    }
}