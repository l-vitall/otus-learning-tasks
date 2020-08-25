using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Calories.Common;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Records.Models;
using Calories.Records.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Calories.Records.Controllers
{
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CaloriesRecordsController : ControllerBase
    {
        private readonly ICaloriesRecordService _caloriesRecordService;
        private readonly IAuthorizationService _authService;
        private readonly IMemoryCache _memoryCache;
        private readonly GrpcEnvironmentSettings _envSettings;
        private readonly PagingOptions _defaultPagingOptions;

        protected ILogger Logger { get; }
        
        private static ConcurrentDictionary<int, object> _caloriesRecordsLocks = new ConcurrentDictionary<int, object>();
        
        public CaloriesRecordsController(IOptions<PagingOptions> defaultPagingOptionsWrapper
            , ICaloriesRecordService caloriesRecordService
            ,IAuthorizationService authService
            ,IMemoryCache memoryCache
            ,GrpcEnvironmentSettings envSettings)
        {
            Logger = Log.ForContext(GetType());
            Logger.Information("Ctor called.");
            
            _caloriesRecordService = caloriesRecordService;
            _authService = authService;
            _memoryCache = memoryCache;
            _envSettings = envSettings;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }
        
        //GET /caloriesRecords
        [HttpGet(Name = nameof(GetAllCaloriesRecords))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<CaloriesRecord>>> GetAllCaloriesRecords([FromQuery] PagingOptions pagingOptions
            , [FromQuery] SortOptions<CaloriesRecord, CaloriesRecordEntity> sortOptions
            , [FromQuery] SearchOptions<CaloriesRecord, CaloriesRecordEntity> searchOptions)
        {
            var canRead = await _authService.AuthorizeAsync(User, AuthorizationPolicy.ReadCaloriesRecords);
            
            if(!canRead.Succeeded)
                return Unauthorized();
            
            pagingOptions.Offset ??= _defaultPagingOptions.Offset;
            pagingOptions.Limit ??= _defaultPagingOptions.Limit;
            
            var canSeeEveryone = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllCaloriesRecords);
            //var canSeeEveryone = new {Succeeded = true};
            
            PagedResults<CaloriesRecord> records = null;
            if (_envSettings.CacheEnabled)
            {
                var cacheKey = $"{pagingOptions.Offset}{pagingOptions.Limit}{sortOptions.OrderBy}{searchOptions.Search}".GetHashCode();
                records = _memoryCache.GetOrCreate(cacheKey,  entry =>
                {
                    lock (_caloriesRecordsLocks.GetOrAdd(cacheKey, new object()))
                    {
                        var value = _memoryCache.Get<PagedResults<CaloriesRecord>>(cacheKey);

                        if (value == null)
                        {
                            //Task.Delay(1000).GetAwaiter().GetResult();
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                            value = _caloriesRecordService.GetCaloriesRecordsAsync(pagingOptions,
                                sortOptions,
                                searchOptions,
                                canSeeEveryone.Succeeded ? null : User.Email()).GetAwaiter().GetResult();
                        }

                        _caloriesRecordsLocks.TryRemove(cacheKey, out _);
                        return value;
                    }
                });
            }
            else
                records = await _caloriesRecordService.GetCaloriesRecordsAsync(pagingOptions,
                    sortOptions,
                    searchOptions,
                    canSeeEveryone.Succeeded ? null : User.Email());

            var collection = PagedCollection<CaloriesRecord>.Create<CaloriesRecordsResponse>(
                Link.ToCollection(nameof(GetAllCaloriesRecords)),
                records.Items.ToArray(),
                records.TotalSize,
                pagingOptions);

            collection.CaloriesRecordsQuery = FormMetadata.FromResource<CaloriesRecord>(
                Link.ToForm(nameof(GetAllCaloriesRecords),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
        
        //GET /caloriesrecords/{caloriesRecordId}
        [HttpGet("{caloriesRecordId}", Name = nameof(GetCaloriesRecordById))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<CaloriesRecord>> GetCaloriesRecordById(Guid caloriesRecordId)
        {
            var canRead = await _authService.AuthorizeAsync(User, AuthorizationPolicy.ReadCaloriesRecords);
            
            if(!canRead.Succeeded)
                return Unauthorized();
            
            var canSeeEveryone = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllCaloriesRecords);
            //var canSeeEveryone = new {Succeeded = true};
            
            var record = await _caloriesRecordService.GetCaloriesRecordAsync(caloriesRecordId,
                canSeeEveryone.Succeeded ? null : User.Email());
            
            if (record == null)
                return NotFound();
            
            return record;
        }
        
        //POST /caloriesrecords
        [HttpPost(Name = nameof(CreateCaloriesRecord))]
        [ProducesResponseType(201)]
        public async Task<ActionResult> CreateCaloriesRecord([FromBody] CaloriesRecordForm form)
        {
            string ownerUserEmail = null;
            if (!string.IsNullOrEmpty(form.Email) && !User.Email().Equals(form.Email, StringComparison.OrdinalIgnoreCase))
            {
                var canCreateAll = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllCaloriesRecords);
                
                if(!canCreateAll.Succeeded)
                    return Unauthorized();

                ownerUserEmail = form.Email;
            }
            else
            {
                var canCreate = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CreateCaloriesRecords);
                if(!canCreate.Succeeded)
                    return Unauthorized();
                
                ownerUserEmail = User.Email();
            }

            var caloriesRecordId = await _caloriesRecordService.CreateCaloriesRecord(ownerUserEmail, form.Text, form.NumberOfCalories);

            return Created(
                Url.Link(nameof(GetCaloriesRecordById), new { caloriesRecordId }),
                null);
        }

        //DELETE /{caloriesRecordId}
        [HttpDelete("{caloriesRecordId}", Name = nameof(DeleteCaloriesRecordById))]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteCaloriesRecordById(Guid caloriesRecordId)
        {
            var canDelete = await _authService.AuthorizeAsync(User, AuthorizationPolicy.DeleteCaloriesRecords);
            if(!canDelete.Succeeded)
                return Unauthorized();
            
            var canDeleteAll = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllCaloriesRecords);
            
            await _caloriesRecordService.DeleteCaloriesRecordAsync(caloriesRecordId,
                canDeleteAll.Succeeded ? null : User.Email());
            
            return NoContent();
        }
        
        // PUT: /caloriesRecords
        [HttpPut(Name = nameof(UpdateCaloriesRecord))]
        public async Task<IActionResult> UpdateCaloriesRecord([FromBody]UpdateCaloriesRecordForm caloriesRecordModel)
        {
            var canUpdate = await _authService.AuthorizeAsync(User, AuthorizationPolicy.UpdateCaloriesRecords);
            if(!canUpdate.Succeeded)
                return Unauthorized();
            
            var canUpdateAll = await _authService.AuthorizeAsync(User, AuthorizationPolicy.CruidAllCaloriesRecords);
        
            await _caloriesRecordService.UpdateCaloriesRecord(caloriesRecordModel,
                canUpdateAll.Succeeded ? null : User.Email());

            return NoContent();
        }
    }
}