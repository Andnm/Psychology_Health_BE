using HEALTH_SUPPORT.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class Category : Entity<Guid>
    {
        [Required]
        [MaxLength(255)]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public ICollection<SubscriptionData> SubscriptionDatas { get; set; } = new HashSet<SubscriptionData>();
    }
}
