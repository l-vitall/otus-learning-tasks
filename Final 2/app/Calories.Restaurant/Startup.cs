using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Calories.Common;
using Calories.Common.Filters;
using Calories.Common.Models;
using Calories.Common.Services;
using Calories.Restaurant.Data;
using Calories.Restaurant.Service;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Calories.Restaurant
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.Configure<AppSettings>(appSettingsSection);
            
            //services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));
            //services.Configure<EmailSendingSettings>(Configuration.GetSection("EmailSendingSettings"));

            services.AddSingleton<IRestaurantOrderService, RestaurantOrderService>();
            services.AddSingleton<IMenuItemService, MenuItemService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            
            var settings = new EnvironmentSettings();
            services.AddSingleton(settings);
            
            //// start kafka consumer    
            var consumerConfig = new ConsumerConfig
            { 
                GroupId = Guid.NewGuid().ToString(),
                BootstrapServers = settings.KafkaConsumerUri ?? "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Latest
            };  
            
            services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = settings.KafkaProducerUri ?? "localhost:9092",
            });
            
            services.AddSingleton(consumerConfig);  

            services.AddHostedService<KafkaRestaurantService>(); //important
            
            services.AddDbContext<RestaurantsDbContext>(options =>
            {
                if (!appSettings.UseInMemoryDatabase)
                {
                    if(!string.IsNullOrWhiteSpace(settings.DatabaseUri))
                        options.UseNpgsql(settings.DatabaseUri, o => o.SetPostgresVersion(8,10));
                    else
                        options.UseNpgsql(Configuration.GetConnectionString("UserDbContext"));
                }
                else
                {
                    options.UseInMemoryDatabase("caloriesdb");
                }

                options.UseOpenIddict<Guid>();
            }, contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Transient);
            
            services.AddResponseCaching();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            
            //this must come after registering ASP.NET Core Identity
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = OpenIdConnectConstants.Claims.Subject,
                        RoleClaimType = OpenIdConnectConstants.Claims.Role,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret))
                    };
                });
            
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<JsonExceptionFilter>();
                //options.Filters.Add<RequireHttpsOrCloseAttribute>();
                options.Filters.Add<LinkRewritingFilter>();
            });
            
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerDocument();

            //services.AddAutoMapper(options => options.AddProfile<MappingProfile>());
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiError(context.ModelState);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicy.GetAllRestaurantOrders,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseHsts();
            }

            app.UseResponseCaching();
            
            //Note: Used filter instead
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}