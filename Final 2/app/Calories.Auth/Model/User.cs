using System;
using Calories.Common.Infrastructure;
using Calories.Common.Models;

namespace Calories.Auth.Model
{
    public class User : Resource
    {
        [Sortable(Default = true)]
        [Searchable]
        public string Email { get; set; }
        [Sortable]
        [Searchable]
        public string UserName { get; set; }
    }
}