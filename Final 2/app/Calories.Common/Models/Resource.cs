using System.Text.Json.Serialization;

namespace Calories.Common.Models
{
    public abstract class Resource : Link
    {
        [JsonIgnore]
        public Link Self { get; set; } 
    }
}