using Calories.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Calories.Common.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        public JsonExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
            _logger = Log.ForContext(typeof(JsonExceptionFilter));
        }
        
        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();

            if (_env.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.StackTrace;                
            }
            else
            {
                error.Message = "A server error occured";
                error.Detail = context.Exception.Message;
            }

            string controller = context.RouteData.Values["controller"]?.ToString();
            string action = context.RouteData.Values["action"]?.ToString();
            _logger.Error(context.Exception, $"{controller}Controller.{action} failed.");
            
            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}