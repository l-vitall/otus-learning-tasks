using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Calories.Common.Models
{
    public class Link
    {
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";

        public static Link To(string routeName, object routeValues = null)
            => new Link()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };
        
        public static Link ToCollection(string routeName, object routeValues = null)
            => new Link()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = new []{"collection"}
            };
        
        public static Link ToForm(
            string routeName,
            object routeValues = null,
            string method = PostMethod,
            params string[] relations)
            => new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = method,
                Relations = relations
            };
        
        public string Href { get; set; }
        
        public string[] Relations { get; set; }
        
        [DefaultValue(GetMethod)]
        public string Method { get; set; }
        
        [JsonIgnore]
        public string RouteName { get; set; }
        
        [JsonIgnore]
        public object RouteValues { get; set; }
    }
}