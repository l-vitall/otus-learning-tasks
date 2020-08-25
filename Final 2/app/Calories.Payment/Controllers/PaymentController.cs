using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calories.Common;
using Calories.Common.Kafka;
using Calories.Common.Notifications;
using Calories.Common.Services;
using Calories.Payment.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Calories.Payment.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IKafkaProducerService _service;
        private static List<Guid> _paidOrders = new List<Guid>();

        public PaymentController(IKafkaProducerService service)
        {
            _service = service;
        }

        [HttpPost(Name = nameof(ProcessPayment))]
        [ProducesResponseType(201)]
        public async Task<ActionResult> ProcessPayment([FromBody] OrderPaymentForm form)
        {
            if(_paidOrders.Contains(form.OrderId))
                return BadRequest($"Order with Id = {form.OrderId} is already paid.");
            
            //TODO: Connect external service to process payment here 
            _paidOrders.Add(form.OrderId);
            
            var notification = JsonConvert.SerializeObject(new OrderStatusUpdateNotification()
            {
                OrderId = form.OrderId,
                NewStatus = OrderStatus.Paid
            });
            await _service.SendMessage(MessageTopic.Order, MessageKind.OrderStatusUpdate, notification);

            return Ok();
            /*return Created(
                Url.Link(nameof(GetCaloriesRecordById), new { id }),
                null);*/
        }
    }
}