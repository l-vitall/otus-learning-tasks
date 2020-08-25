using System;
using Calories.Common.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Calories.Common.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EtagAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new EtagHeaderFilter();
        }

        public bool IsReusable => true;
    }
}