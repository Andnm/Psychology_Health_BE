using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public static class SubscriptionRequest
    {
        public class CreateSubscriptionModel
        {
            [Required(ErrorMessage = "Subscription Name is required")]
            public string SubscriptionName { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            public double Price { get; set; }

            [Required(ErrorMessage = "Duration is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
            public int Duration { get; set; }

            [Required(ErrorMessage = "CategoryId is required")]
            public Guid CategoryId { get; set; }
        }

        public class UpdateSubscriptionModel
        {
            [Required(ErrorMessage = "Subscription ID is required")]
            public Guid SubscriptionId { get; set; }

            public string? SubscriptionName { get; set; }
            public string? Description { get; set; }
            public double? Price { get; set; }
            public int? Duration { get; set; }
            public Guid? CategoryId { get; set; }
        }
        public class RegisterSubscriptionModel
        {
            public Guid AccountId { get; set; }
            public Guid SubscriptionId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
