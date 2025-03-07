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

        [HttpPost("register", Name = "RegisterSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterSubscription([FromBody] SubscriptionRequest.RegisterSubscriptionModel model)
        {
            if (model == null || model.SubscriptionId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid subscription data" });
            }
            var accountId = GetAccountIdFromClaims();
            await _subscriptionService.RegisterSubscription(accountId, model);
            return Ok(new { message = "Subscription registered successfully" });
        }

        [HttpGet("user-subscriptions", Name = "GetUserSubscriptions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetUserSubscriptions()
        {
            var accountId = GetAccountIdFromClaims();
            var subscriptions = await _subscriptionService.GetUserSubscriptions(accountId);
            return Ok(subscriptions);
        }

        [HttpDelete("cancel/{orderId}", Name = "CancelSubscription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelSubscription(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid order ID" });
            }
            await _subscriptionService.CancelSubscription(orderId);
            return Ok(new { message = "Subscription canceled successfully" });
        }
        private Guid GetAccountIdFromClaims()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdClaim, out var accountId) ? accountId : throw new UnauthorizedAccessException("Invalid account ID");
        }
    }
}
