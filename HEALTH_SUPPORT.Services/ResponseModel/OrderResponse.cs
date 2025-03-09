using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public static class OrderResponse
    {
        public record GetOrderDetailsModel(
            Guid Id,
            string SubscriptionName,
            string Description,
            float Price,
            int Quantity,
            string AccountName,
            string AccountEmail,
            DateTimeOffset CreateAt
        );
    }
}
