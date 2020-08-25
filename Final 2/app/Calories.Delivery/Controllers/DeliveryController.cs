using System.Threading.Tasks;
using Calories.Common.Infrastructure;
using Calories.Common.Models;
using Calories.Delivery.Data;
using Calories.Delivery.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calories.Delivery.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly DeliveryDbContext _context;

        public DeliveryController(IAuthorizationService authService,
            DeliveryDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet(Name = nameof(GetAllDeliveries))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<DeliveryEntity>>> GetAllDeliveries()
        {
            var canReadAll = await _authService.AuthorizeAsync(User, AuthorizationPolicy.GetAllDeliveries );

            DeliveryEntity[] records = null;
            if(!canReadAll.Succeeded)
                return Unauthorized();
            
            records = await _context.Deliveries.ToArrayAsync();
            
            var collection = PagedCollection<DeliveryEntity>.Create<DeliveriesResponse>(
                Link.ToCollection(nameof(GetAllDeliveries)),
                records,
                records.Length,
                new PagingOptions());

            collection.DeliveriesQuery = FormMetadata.FromResource<DeliveryEntity>(
                Link.ToForm(nameof(GetAllDeliveries),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));
            
            return collection;
        }
    }
}