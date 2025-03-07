using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public static class SubscriptionResponse
    {
        public record GetSubscriptionsModel(
            Guid Id,
            string SubscriptionName,
            string Description,
            double Price,
            int Duration,
            Guid CategoryId,
            string CategoryName
        );
    }
}
