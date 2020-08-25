using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Calories.Common;
using Calories.Common.Filters;
using Calories.Common.Models;
using Calories.Records.Data;
using Calories.Records.Infrastructure;
using Calories.Records.Models;
using Calories.Records.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Prometheus;

namespace Calories.Records
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
            services.Configure<CaloriesApiSettings>(Configuration.GetSection("CaloriesApiSettings"));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.Configure<AppSettings>(appSettingsSection);

            services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));

            services.AddScoped<ICaloriesRecordService, CaloriesRecordService>();
            services.AddSingleton<ICaloriesApiService, CaloriesApiService>();
            services.AddSingleton<IGrpcUserProfileClientService, GrpcUserProfileClientService>();

            var settings = new GrpcEnvironmentSettings();
            services.AddSingleton(settings);

            services.AddTransient<UserResolverService>();
            
            services.AddDbContext<CaloriesRecordsDbContext>();

            services.AddResponseCaching();

            // Add OpenIddict services
            // services.AddOpenIddict()
            //     .AddCore(options =>
            //     {
            //         options.UseEntityFrameworkCore()
            //             .UseDbContext<CaloriesRecordsDbContext>()
            //             .ReplaceDefaultEntities<Guid>();
            //     })
            //     .AddServer(options =>
            //     {
            //         options.UseMvc();
            //
            //         options.EnableTokenEndpoint("/token");
            //
            //         options.AllowPasswordFlow();
            //         options.AcceptAnonymousClients();
            //     });
            // .AddValidation(options => 
            // {
            //     //options.UseReferenceTokens();
            // });

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

            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });

            //AddAuthentication(services, oauthSection);
            services.AddHttpContextAccessor();
            services.AddMvc(options =>
            {
                options.Filters.Add<JsonExceptionFilter>();
                //options.Filters.Add<RequireHttpsOrCloseAttribute>();
                options.Filters.Add<LinkRewritingFilter>();
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerDocument();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());
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
                options.AddPolicy(AuthorizationPolicy.CruidAllCaloriesRecords,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator, AuthorizationRole.Manager));
                options.AddPolicy(AuthorizationPolicy.ReadCaloriesRecords,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator, AuthorizationRole.User, AuthorizationRole.Manager));
                options.AddPolicy(AuthorizationPolicy.CreateCaloriesRecords,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator, AuthorizationRole.User));
                options.AddPolicy(AuthorizationPolicy.UpdateCaloriesRecords,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator, AuthorizationRole.User));
                options.AddPolicy(AuthorizationPolicy.DeleteCaloriesRecords,
                    p => p.RequireAuthenticatedUser().RequireRole(AuthorizationRole.Administrator, AuthorizationRole.User));
            });
            
            //services.AddMultitenancy<AppTenant, CachingTenantResolver>();
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

            // app.UseMultitenancy<AppTenant>();
            // app.UsePerTenant<AppTenant>((ctx, builder) =>
            // {
            //     JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //     builder.UseOpenApi();
            // });
            app.UseResponseCaching();

            //Note: Used filter instead
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
        }
    }
}