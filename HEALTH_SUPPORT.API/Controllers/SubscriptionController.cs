using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace HEALTH_SUPPORT.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet(Name = "GetSubscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSubscriptions()
        {
            var result = await _subscriptionService.GetSubscriptions();
            return Ok(result);
        }

        [HttpGet("{subscriptionId}", Name = "GetSubscriptionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSubscriptionById(Guid subscriptionId)
        {
            var result = await _subscriptionService.GetSubscriptionById(subscriptionId);
            if (result == null)
            {
                return NotFound(new { message = "Subscription not found" });
            }
            return Ok(result);
        }

        [HttpPost(Name = "CreateSubscription")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateSubscription([FromBody] SubscriptionRequest.CreateSubscriptionModel model)
        {
            await _subscriptionService.AddSubscription(model);
            return CreatedAtRoute("GetSubscriptionById", new { subscriptionId = /* newly created id */ Guid.NewGuid() }, new { message = "Subscription created successfully" });
        }

        [HttpPut("{subscriptionId}", Name = "UpdateSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSubscription(Guid subscriptionId, [FromBody] SubscriptionRequest.UpdateSubscriptionModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid update data" });
            }
            // Kiểm tra xem subscription có tồn tại không
            var existingSubscription = await _subscriptionService.GetSubscriptionById(subscriptionId);
            if (existingSubscription == null)
            {
                return NotFound(new { message = "Subscription not found" });
            }

            await _subscriptionService.UpdateSubscription(subscriptionId, model);
            return Ok(new { message = "Subscription updated successfully" });
        }

        [HttpDelete("{subscriptionId}", Name = "DeleteSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSubscription(Guid subscriptionId)
        {
            var existingSubscription = await _subscriptionService.GetSubscriptionById(subscriptionId);
            if (existingSubscription == null)
            {
                return NotFound(new { message = "Subscription not found" });
            }
            await _subscriptionService.RemoveSubscription(subscriptionId);
            return Ok(new { message = "Subscription deleted successfully" });
        }

        //[HttpPost(Name = "CreateOrder")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult> CreateOrder(Guid subscriptionDataId, Guid accountId, int quantity)
        //{
        //    if (subscriptionDataId == Guid.Empty || accountId == Guid.Empty || quantity <= 0)
        //    {
        //        return BadRequest(new { message = "Invalid order data. Please provide valid SubscriptionDataId, AccountId, and Quantity." });
        //    }

        //    var orderId = await _subscriptionService.CreateOrder(subscriptionDataId, accountId, quantity);

        //    if (orderId == Guid.Empty)
        //    {
        //        return BadRequest(new { message = "Failed to create order. Please check the provided data." });
        //    }

        //    var orderDetails = await _subscriptionService.GetOrderDetails(orderId);

        //    if (orderDetails == null)
        //    {
        //        return BadRequest(new { message = "Order created but details could not be retrieved." });
        //    }

        //    return CreatedAtRoute("GetOrderDetails", new { orderId = orderId }, new { message = "Order created successfully" });
        //}

        [HttpGet("{orderId}", Name = "GetOrderDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetOrderDetails(Guid orderId)
        {
            var result = await _subscriptionService.GetOrderDetails(orderId);

            if (result == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(result);
        }


    }
}
