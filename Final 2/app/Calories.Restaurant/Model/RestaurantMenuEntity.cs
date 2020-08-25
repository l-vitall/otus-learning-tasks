using System;
using System.ComponentModel.DataAnnotations;

namespace Calories.Restaurant.Model
{
    public enum MenuItemKind
    {
        Soup,
        Hot,
        Salad,
        Desert,
        Poison,
        Drink
    }
    
    public class RestaurantMenuItemEntity
    {
        [Key]
        public long Id { get; set; }
        public long RestaurantId { get; set; }
        public string Name { get; set; }
        public int CaloriesCount { get; set; }
        public MenuItemKind Kind { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Comment { get; set; }
    }
}