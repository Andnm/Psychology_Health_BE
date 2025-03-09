using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionResponse.GetSubscriptionsModel>> GetSubscriptions();
        Task<SubscriptionResponse.GetSubscriptionsModel?> GetSubscriptionById(Guid id);
        Task AddSubscription(SubscriptionRequest.CreateSubscriptionModel model);
        Task UpdateSubscription(Guid id, SubscriptionRequest.UpdateSubscriptionModel model);
        Task RemoveSubscription(Guid id);
        Task<Guid> CreateOrder(Guid subscriptionDataId, Guid accountId, int quantity);
        Task<OrderResponse.GetOrderDetailsModel?> ConfirmOrder(Guid orderId);
        Task<OrderResponse.GetOrderDetailsModel?> GetOrderDetails(Guid orderId);
    }
}
