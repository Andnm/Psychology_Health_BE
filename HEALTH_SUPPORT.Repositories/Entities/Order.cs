using HEALTH_SUPPORT.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class Order : Entity<Guid>, IAuditable
    {
        [Required]
        public int Quantity { get; set; }

        [Required]
        public Guid SubscriptionDataId { get; set; }
        [ForeignKey("SubscriptionDataId")]
        public SubscriptionData SubscriptionData { get; set; }

        [Required]
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account Accounts { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Transaction> Transaction { get; set; }
        public ICollection<SubscriptionProgress> SubscriptionProgresses { get; set; }
    }
}
