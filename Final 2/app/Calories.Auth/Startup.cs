using System;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Calories.Auth.Data;
using Calories.Auth.Infrastructure;
using Calories.Auth.Services;
using Calories.Common;
using Calories.Common.Filters;
using Calories.Common.Models;
using Calories.Common.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation;
using UserEntity = Calories.Auth.Model.UserEntity;
using UserRoleEntity = Calories.Auth.Model.UserRoleEntity;

namespace Calories.Auth
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
            var appSettings = appSettingsSection.Get<AppSettingsAuth>();
            services.Configure<AppSettingsAuth>(appSettingsSection);
            
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            
            var settings = new AuthEnvironmentSettings();
            services.AddSingleton(settings);
            
            services.AddSingleton(new ProducerConfig
            {
                BootstrapServers = settings.KafkaProducerUri ?? "localhost:9092",
            });
            
            services.AddDbContext<AuthDbContext>(options =>
            {
                if (!appSettings.UseInMemoryDatabase)
                {
                    if(!string.IsNullOrWhiteSpace(settings.DatabaseUri))
                        options.UseNpgsql(settings.DatabaseUri);
                    else
                        options.UseNpgsql(Configuration.GetConnectionString("CaloriesApiDbContext"));
                }
                else
                {
                    options.UseInMemoryDatabase("caloriesdb");
                }

                options.UseOpenIddict<Guid>();
            });
            
            // Add OpenIddict services
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<AuthDbContext>()
                        .ReplaceDefaultEntities<Guid>();
                })
                .AddServer(options =>
                {
                    options.UseMvc();

                    options.EnableTokenEndpoint("/token");

                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();
                    
                    options.AddSigningKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret)));
                    options.UseJsonWebTokens();

                    options.DisableHttpsRequirement();
                })
                .AddValidation();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(36135);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
            
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = OpenIddictValidationDefaults.AuthenticationScheme;
            });
            
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            AddIdentityCoreServices(services);
            
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Static", new CacheProfile(){Duration = 86400});
                options.Filters.Add<JsonExceptionFilter>();
                //options.Filters.Add<RequireHttpsOrCloseAttribute>();
                options.Filters.Add<LinkRewritingFilter>();
            });
            
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerDocument();

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());
            
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });
            
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiError(context.ModelState);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddAuthorization();
        }

        private static void AddIdentityCoreServices(IServiceCollection services)
        {
            var builder = services.AddDefaultIdentity<UserEntity>();
            builder = new IdentityBuilder(builder.UserType, typeof(UserRoleEntity), builder.Services);

            builder.AddRoles<UserRoleEntity>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<UserEntity>>();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
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

            //Note: Used filter instead
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}