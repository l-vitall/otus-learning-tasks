using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calories.Restaurant.Data;
using Calories.Restaurant.Model;
using Microsoft.EntityFrameworkCore;

namespace Calories.Restaurant.Service
{
    public class MenuItemService : IMenuItemService
    {
        private readonly RestaurantsDbContext _dbContext;

        public MenuItemService(RestaurantsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<RestaurantMenuItemEntity[]> GetAllMenuitems()
        {
            return await _dbContext.MenuItems.ToArrayAsync();
        }
        
        public async Task CreateMenuItem(RestaurantMenuItemEntity menuItem)
        {
            await _dbContext.MenuItems.AddAsync(menuItem);
           
            var created = await _dbContext.SaveChangesAsync();

            if (created < 1)
                throw new InvalidOperationException("Could not create a notification");
        }
    }
}